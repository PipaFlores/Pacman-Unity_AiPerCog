using Unity.VisualScripting;
using UnityEngine;

public class Pellet : MonoBehaviour
{
    public int points = 10;
    protected virtual void Eat() // Protected so powerpellet can access it and virtual so powerpellet can override
    {
        FindObjectOfType<GameManager>().PelletEaten(this);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Pacman")){
            Eat();
        }
    }
    

}
