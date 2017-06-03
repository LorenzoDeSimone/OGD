using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerDresser : MonoBehaviour
    {
        public static PlayerDresser instance;
       
        public Sprite[] playerSprites;
        public List<Animation[]> animations;

        void Start()
        {
            if (instance == null)
                instance = this;
        }

        public void DressPlayer(SpriteRenderer rend, int playerId)
        {
            rend.sprite = playerSprites[playerId % playerSprites.Length];
        }

        public void AnimatePlayer(AnimatorController contr, int playerId)
        {
        }
    }

}