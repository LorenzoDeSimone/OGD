using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Player
{
    class PaintThePlayer : NetworkBehaviour
    {
        void Start()
        {
            int color_num = 0;
            var netIdent = GetComponent<NetworkIdentity>();

            color_num = (int)netIdent.netId.Value;

            GetComponent<SpriteRenderer>().color = PlayerColor.GetColor(color_num);
        }
    }
}
