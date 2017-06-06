using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class PlayerScore : MonoBehaviour
    {
        public Image playerImage;
        public Image scoreBarImage;
        int playerPoints;
        int totalPoints;

        private void Start()
        {
            Vector2 temp = Vector2.zero;
            temp.Set(playerPoints / (float)totalPoints, 1);
            scoreBarImage.rectTransform.anchorMax = temp;
            temp.Set(0, 0);
            scoreBarImage.rectTransform.anchorMin = temp;
        }

        public void SetPoints(int pp, int tp)
        {
            playerPoints = pp;
            totalPoints = tp;
        }

        public void SetSprites(Sprite ps, Sprite bs)
        {
            scoreBarImage.sprite = bs;
            playerImage.sprite = ps;
        }
    }

}