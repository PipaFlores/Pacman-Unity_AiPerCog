using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimatedSprite : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    public float animationTime = 0.25f;
    public int animationFrame { get; private set;}
    public bool loop = true;


    private void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        this.spriteRenderer.enabled = true;
    }

    private void OnDisable()
    {
        this.spriteRenderer.enabled = false;
    }
    private void Start()
    {
        InvokeRepeating(nameof(Advance), this.animationTime, this.animationTime);
    }

    private void Advance()
    {
        if (!this.spriteRenderer.enabled) {
            return;
        }
        this.animationFrame++;
        if (this.animationFrame >= this.sprites.Length && this.loop){
            this.animationFrame = 0;
        }
        if (this.animationFrame >= 0 && this.animationFrame < this.sprites.Length){
            this.spriteRenderer.sprite = this.sprites[this.animationFrame];
        }

    }

    // This method calls the animation for the death of pacman, showed in the lives indicator instead of the pacman itself
    public void PacmanDeathAnimation(){
        StartCoroutine(DeathAnimation());
    }

    private IEnumerator DeathAnimation()
    {
        float duration = this.animationTime * this.sprites.Length;
        float elapsedTime = 0f;
        int spriteIndex = 0;
        this.GetComponentInParent<UnityEngine.UI.Image>().enabled = false;
        this.spriteRenderer.enabled = true;

        while (elapsedTime < duration)
        {
            this.spriteRenderer.sprite = this.sprites[spriteIndex];
            spriteIndex = (spriteIndex + 1) % this.sprites.Length;
            //this.spriteRenderer.enabled = !this.spriteRenderer.enabled;

            yield return new WaitForSecondsRealtime(this.animationTime);
            elapsedTime += this.animationTime;
        }

        this.spriteRenderer.enabled = false;
        this.GetComponentInParent<UnityEngine.UI.Image>().enabled = true;
    }

    public void Restart()
    {
        this.animationFrame = -1;
        Advance();
    }
}
