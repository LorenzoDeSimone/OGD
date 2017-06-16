using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

namespace Assets.Scripts.Networking
{
    abstract public class PlayMenu : MonoBehaviour
    {
        protected const string CHARS_POOL = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789-";
        protected const int NAME_LENGHT = 16;

        public MaskableGraphic loadingSpinner;
        public MaskableGraphic loadingMessage;

        protected NetworkLobbyController lobbyController;

        protected abstract void InitMenu();
        protected abstract void TryInitMenu();
        protected abstract void TryJoinMatch();

        void OnEnable()
        {
            lobbyController = NetworkLobbyController.instance;
            TryInitMenu();
        }

        internal void StopMatchSearch()
        {
            StopAllCoroutines();
        }

        protected string RandomPublicName()
        {
            string randName = "";
            System.Random rInt = new System.Random();

            for (int i = 0; i < NAME_LENGHT; i++)
            {
                randName += CHARS_POOL[rInt.Next() % CHARS_POOL.Length];
            }

            return randName;
        }

        protected IEnumerator SpinLoadingSpinner()
        {
            while (true)
            {
                loadingSpinner.transform.Rotate(loadingSpinner.transform.forward, -2.0f);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
