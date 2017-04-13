using UnityEngine;
using UnityEngine.SceneManagement;
namespace Assets.Scripts.Utility
{
    public class Quitter : MonoBehaviour
    {
        public void QuitApplication()
        {
            Application.Quit();
        }       
    }
}