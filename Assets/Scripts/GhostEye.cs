using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEye : MonoBehaviour
{
    public SpriteRenderer spriteRenderer { get; private set; }
    public Movement movement { get; private set; }
    public Sprite up;
    public Sprite down;
    public Sprite right;
    public Sprite left;

    private void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.movement = GetComponentInParent<Movement>();
    }

    private void Update()
    {
        if (this.movement.direction == Vector2.up){
            this.spriteRenderer.sprite = up;
        }
        else if (this.movement.direction == Vector2.down){
            this.spriteRenderer.sprite = down;
        }
        else if (this.movement.direction == Vector2.left){
            this.spriteRenderer.sprite = left;
        }
        else if (this.movement.direction == Vector2.right){
            this.spriteRenderer.sprite = right;
        }
    }
}
