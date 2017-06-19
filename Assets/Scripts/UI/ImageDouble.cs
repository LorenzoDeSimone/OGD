using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts.Player;

public class ImageDouble : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Image myImage;
    public bool isPlayerWalker = false;
    public PlayerDresser playerDresser; 

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        myImage = GetComponent<Image>();
        Animator myAnimator = GetComponent<Animator>();

        if(myAnimator && isPlayerWalker)
        {
            myAnimator.SetBool("moving", true);
            myAnimator.runtimeAnimatorController = playerDresser.GetAnimator(Random.Range(0,4));
        }
    }

    void Update()
    {
        myImage.sprite = spriteRenderer.sprite;
        myImage.preserveAspect = true;
    }
}
