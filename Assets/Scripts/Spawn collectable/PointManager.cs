using UnityEngine;
using Assets.Scripts.Player;
using System.Collections.Generic;

public class PointManager : MonoBehaviour
{
    public GameObject pointBarSegmentPrefab;
    public GameObject pointBarSpaceRoot;

    Dictionary<int, RectTransform> ofPlayersAndBars;
    Dictionary<int, int> ofPlayersAndPoints;

    int pointsTotal = 0;

    private void Start()
    {
        ofPlayersAndBars = new Dictionary<int, RectTransform>();
        ofPlayersAndPoints = new Dictionary<int, int>();

        PlayerDataHolder.PointSyncEvent += UpdateBar;
    }

    private void OnDestroy()
    {
        PlayerDataHolder.PointSyncEvent -= UpdateBar;
    }

    private void UpdateBar(int playerNetID, int playerPoints)
    {
        pointsTotal += playerPoints;

        if (!ofPlayersAndBars.ContainsKey(playerNetID))
        {
            AddNewBar(playerNetID);
        }

        ofPlayersAndPoints[playerNetID] += playerPoints;

        ScalePointsBars();
    }

    private void AddNewBar(int playerNetID)
    {
        GameObject go = Instantiate(pointBarSegmentPrefab, pointBarSpaceRoot.transform, false);
        ofPlayersAndBars[playerNetID] = go.GetComponent<RectTransform>();
        ofPlayersAndPoints[playerNetID] = 0;
    }

    private void ScalePointsBars()
    {
        float offSet = 0.0f;
        RectTransform rect;
        Vector3 newAnchorMin;
        Vector3 newAnchorMax;

        foreach(int k in ofPlayersAndBars.Keys)
        {
            rect = ofPlayersAndBars[k];

            newAnchorMin = rect.anchorMin;
            newAnchorMax = rect.anchorMax;

            newAnchorMin.x = offSet;

            if (ofPlayersAndPoints[k]>0)
            {
                offSet = (pointsTotal / ofPlayersAndPoints[k]); 
            }
            else
            {
                offSet = 0;
            }

            newAnchorMax.x = offSet + newAnchorMin.x;

            rect.anchorMin = newAnchorMin;
            rect.anchorMax = newAnchorMax;
        }
    }
}
