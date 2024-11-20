using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AnimatedButton : MonoBehaviour
{
    public Image image;
    public Sprite[] sprites;
    public float animationTime = 0.25f;
    public int animationFrame { get; private set;}
    public bool loop = true;


    private void Awake()
    {
        this.image = this.gameObject.GetComponent<Image>();
        this.enabled = false;
    }

    private void OnEnable()
    {
        this.image.enabled = true;
    }

    private void OnDisable()
    {
        this.image.sprite = sprites[0];
        CancelInvoke(nameof(Advance));
    }
    private void Start()
    {
        InvokeRepeating(nameof(Advance), this.animationTime, this.animationTime);
    }

    private void Advance()
    {
        if (!this.image.enabled) {
            return;
        }
        this.animationFrame++;
        if (this.animationFrame >= this.sprites.Length && this.loop){
            this.animationFrame = 0;
        }
        if (this.animationFrame >= 0 && this.animationFrame < this.sprites.Length){
            this.image.sprite = this.sprites[this.animationFrame];
        }

    }

}
