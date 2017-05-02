using UnityEngine;
using Assets.Scripts.Player;
using System.Collections.Generic;
using UnityEngine.UI;

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

    //player uses -1 as points for bar init
    private void UpdateBar(int playerNetID, int playerPoints)
    {
        if (!ofPlayersAndBars.ContainsKey(playerNetID))
        {
            AddNewBar(playerNetID);
        }

        if (playerPoints > 0)
        {
            ofPlayersAndPoints[playerNetID] = playerPoints; 
            pointsTotal = CalculateTotalPoints();
        }

        ScalePointsBars();
    }

    private void AddNewBar(int playerNetID)
    {
        GameObject go = Instantiate(pointBarSegmentPrefab, pointBarSpaceRoot.transform, false);
        go.GetComponent<Image>().color = PlayerColor.GetColor(playerNetID);

        ofPlayersAndBars[playerNetID] = go.GetComponent<RectTransform>();
        ofPlayersAndPoints[playerNetID] = 0;
    }

    float offSet;
    RectTransform rect;
    Vector3 newAnchorMin;
    Vector3 newAnchorMax;

    private void ScalePointsBars()
    {
        offSet = 0.0f;
        foreach (int k in ofPlayersAndBars.Keys)
        {
            rect = ofPlayersAndBars[k];

            newAnchorMin = rect.anchorMin;
            newAnchorMax = rect.anchorMax;

            newAnchorMin.x = offSet;

            if (ofPlayersAndPoints[k] > 0)
            {
                Debug.LogWarning("Bar update: " + k + " " + ofPlayersAndPoints[k] / (float)pointsTotal);
                offSet = ofPlayersAndPoints[k] / (float)pointsTotal;
            }
            else
            {
                offSet = 1 / (float) (pointsTotal + UnityEngine.Networking.NetworkManager.singleton.matchSize) ;
            }

            newAnchorMax.x = offSet + newAnchorMin.x;

            rect.anchorMin = newAnchorMin;
            rect.anchorMax = newAnchorMax;
        }
    }

    private int CalculateTotalPoints()
    {
        int sum = 0;

        foreach(int n in ofPlayersAndPoints.Values)
        {
            sum += n;
        }

        return sum;
    }
}
