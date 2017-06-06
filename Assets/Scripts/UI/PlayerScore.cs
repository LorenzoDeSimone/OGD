using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class PlayerScore : MonoBehaviour
    {
        public Image playerImage;
        public Image scoreBarImage;
        Sprite playerSprite;
        int playerPoints;
        int totalPoints;

        private void Start()
        {
            Vector2 temp = scoreBarImage.rectTransform.anchorMax;
            scoreBarImage.rectTransform.anchorMax.Set(temp.x, playerPoints / (float)totalPoints);
            playerImage.sprite = playerSprite;
        }

        public void SetPoints(int pp, int tp)
        {
            playerPoints = pp;
            totalPoints = tp;
        }

        public void SetSprite(Sprite sprite)
        {
            playerSprite = sprite;
        }
    }

}