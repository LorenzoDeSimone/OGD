using UnityEngine;
using Assets.Scripts.Player;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System;

public class PointManager : MonoBehaviour
{
    public static PointManager instance;

    public GameObject pointBarSegmentPrefab;
    public Sprite[] barSprites;

    Dictionary<int, RectTransform> ofPlayersAndBars;
    Dictionary<int, int> ofPlayersAndPoints;

    int pointsTotal = 0;

    private void Start()
    {
        if(instance == null)
            instance = this;

        ofPlayersAndBars = new Dictionary<int, RectTransform>();
        ofPlayersAndPoints = new Dictionary<int, int>();
    }

    //player uses -1 as points for bar init
    public void UpdateBar(int playerNetID, int playerPoints)
    {
        if (!ofPlayersAndBars.ContainsKey(playerNetID))
        {
            AddNewBar(playerNetID);
        }

        if (playerPoints > 0)
        {
            //Debug.Log("Updating points " + playerNetID + " " + playerPoints);
            ofPlayersAndPoints[playerNetID] = playerPoints; 
            pointsTotal = CalculateTotalPoints();
        }

        ScalePointsBars();
    }

    internal int GetMatchSize()
    {
        return ofPlayersAndPoints.Keys.Count;
    }

    private void AddNewBar(int playerNetID)
    {
        GameObject go;
        Sprite newSprite = GetPointBar(playerNetID);
        
        try
        {
            go = Instantiate(pointBarSegmentPrefab, transform, false);
            go.GetComponent<Image>().sprite = newSprite;
        }
        catch (System.Exception)
        {
            //Something unity won't the find prefab i put i the editor WTF?!!
            go = Instantiate((GameObject)Resources.Load("Prefabs/UI/PointBarSegment"), transform, false);
            go.GetComponent<Image>().sprite = newSprite;
        }

        go.GetComponent<Image>().color = PlayerColor.GetColor(playerNetID);

        ofPlayersAndBars[playerNetID] = go.GetComponent<RectTransform>();
        ofPlayersAndPoints[playerNetID] = 0;
    }

    internal Sprite GetPointBar(int pID)
    {
        return barSprites[pID % barSprites.Length];
    }

    float offSet;
    int numZeroPointPlayer;
    RectTransform rect;
    Vector2 newAnchorMin;
    Vector2 newAnchorMax;

    private void ScalePointsBars()
    {
        offSet = 0.0f;
        numZeroPointPlayer = 0;

        foreach (int k in ofPlayersAndBars.Keys)
            if (ofPlayersAndPoints[k] == 0)
                numZeroPointPlayer++;
        foreach (int k in ofPlayersAndBars.Keys)
        {
            rect = ofPlayersAndBars[k];

            newAnchorMin = rect.anchorMin;
            newAnchorMax = rect.anchorMax;

            newAnchorMin.x = offSet;

            if (pointsTotal <= 0)
            {
                offSet += 1f / (float)ofPlayersAndBars.Count;
            }
            else
            {
                if (ofPlayersAndPoints[k] > 0)
                {
                    offSet += ((float)ofPlayersAndPoints[k] / (float)pointsTotal) * (1f - (0.05f * (float)numZeroPointPlayer));
                }
                else
                {
                    offSet += 0.05f;
                }
            }
            
            newAnchorMax.x = offSet;

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

    public int GetPlayerRankPosition(int playerId)
    {
        int count = ofPlayersAndPoints.Keys.Count;
        foreach(int id in ofPlayersAndPoints.Keys)
        {
            if(id != playerId && ofPlayersAndPoints[playerId] > ofPlayersAndPoints[id])
            {
                count--;
            }
        }
        return count;
    }

    public int GetTotalPoints()
    {
        return pointsTotal;
    }

    public Dictionary<int,int> GetPointsForPlayers()
    {
        return ofPlayersAndPoints;
    }
}
