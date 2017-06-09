using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class PlayerScore : MonoBehaviour
    {
        public Image playerImage;
        public Image mask;
        public Image scoreBarImage;
        int playerPoints;
        int totalPoints;

        private void Start()
        {
            Vector2 temp = Vector2.zero;
            scoreBarImage.rectTransform.anchorMin = temp;
            temp.Set(playerPoints / (float)totalPoints, 1);
            mask.rectTransform.anchorMax = temp;
        }

        public void SetPoints(int pp, int tp)
        {
            playerPoints = pp;
            totalPoints = tp;
        }

        public void SetSprites(Sprite ps, Sprite bs)
        {
            scoreBarImage.sprite = bs;
            scoreBarImage.preserveAspect = false;
            playerImage.sprite = ps;
            playerImage.preserveAspect = true;
        }
    }

}