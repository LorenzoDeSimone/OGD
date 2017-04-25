using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI
{
    class VictoryScreenController : MonoBehaviour
    {
        public GameObject scoreBoard;
        public GameObject controlsHolder;

        public void ShowScoreboard()
        {
            scoreBoard.SetActive(true);
            controlsHolder.SetActive(false);
            // TO DO: Fill Vicory Screen With scores
        }
    }
}
