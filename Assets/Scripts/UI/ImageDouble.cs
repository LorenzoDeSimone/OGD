using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ImageDouble : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Image myImage;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        myImage = GetComponent<Image>();
        Animator myAnimator = GetComponent<Animator>();
        if(myAnimator)
        {
            myAnimator.SetBool("moving", true);
        }
    }

    void Update()
    {
        myImage.sprite = spriteRenderer.sprite;
        myImage.preserveAspect = true;
    }
}
