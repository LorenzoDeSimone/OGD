using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Assets.Scripts.Networking
{
    public class OnlineSceneSelector : MonoBehaviour
    {
        public List<string> sceneNames;

        public string GetSceneFor(int val)
        {
            return sceneNames[val % sceneNames.Count];
        }
    }

}