using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneName="Lobby";
    public float loadTime = 2.0f;

	private void Start ()
    {
        StartCoroutine(OpenSceneAfterTime());
	}

    private IEnumerator OpenSceneAfterTime()
    {
        yield return new WaitForSeconds(loadTime);
        SceneManager.LoadScene(sceneName);
    }
}
