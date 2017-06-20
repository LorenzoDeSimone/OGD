using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class StarRotator : MonoBehaviour
{
    private List<GameObject> starLayers;
    public float minSpeed = 0.1f;
    public float maxDelta = 0.3f;
    
    int rotationDirection = 1;
    List<float> starLayerSpeeds;

    void Start()
    {
        starLayers = new List<GameObject>();

        foreach (Transform currStarLayer in transform)
        {
            if(!currStarLayer.Equals(transform))
                starLayers.Add(currStarLayer.gameObject);
        }

        starLayerSpeeds = new List<float>();
        int[] randoms = {1, -1};

        rotationDirection = randoms[UnityEngine.Random.Range(0,randoms.Length)];

        for(int i = 0; i < starLayers.Count; i++)
        {
            starLayerSpeeds.Add(UnityEngine.Random.Range(minSpeed, minSpeed+maxDelta));
        }

        InitStarRotatino();
        StartCoroutine(RotateStars());
    }

    private void InitStarRotatino()
    {
        foreach (GameObject go in starLayers)
        {
            go.transform.Rotate(transform.forward, UnityEngine.Random.Range(0.0f,360.0f));
        }
    }

    private IEnumerator RotateStars()
    {
        while (true)
        {
            int i = 0;
            foreach (GameObject go in starLayers)
            {
                go.transform.Rotate(go.transform.forward, rotationDirection * starLayerSpeeds[i]);
                i++;
            }
            yield return new WaitForEndOfFrame(); 
        }
    }
}
