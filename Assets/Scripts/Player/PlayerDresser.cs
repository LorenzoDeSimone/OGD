﻿using UnityEngine;

namespace Assets.Scripts.Player
{

    [CreateAssetMenu(fileName = "PlayerDresser", menuName = "Dresser", order = 1)]
    public class PlayerDresser : ScriptableObject
    {     
        public Sprite[] playerSprites;
        public AnimatorOverrideController[] animators;

        public void DressPlayer(SpriteRenderer rend, int playerId)
        {
            rend.sprite = GetSprite(playerId);
        }

        public AnimatorOverrideController GetAnimator(int playerId)
        {
            return animators[playerId % playerSprites.Length];
        }

        public Sprite GetSprite(int playerId)
        {
            return playerSprites[playerId % playerSprites.Length];
        }
    }

}