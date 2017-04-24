using UnityEngine;

namespace Assets.Scripts.UI
{
    class VictoryScreenController : MonoBehaviour
    {
        public GameObject scoreBoard;

        public void ShowScoreboard()
        {
            scoreBoard.SetActive(true);
            // TO DO: Fill Vicory Screen With scores
        }
    }
}
