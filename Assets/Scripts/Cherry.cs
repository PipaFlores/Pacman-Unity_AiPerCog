using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using TMPro;

public class Cherry : MonoBehaviour
{
    public int points;
    public Sprite[] sprites;

    public GameObject FloatingPoint;

    public int cherryIndex; // 1 first and 2 second cherry
    
    void OnEnable()
    {
        Invoke(nameof(Disable), 10.0f);
    }

    void Disable()
    {
        gameObject.SetActive(false);
        if (cherryIndex == 1)
        {
            FindObjectOfType<GameManager>().fruitState_1 = 0;
        }
        else
        {
            FindObjectOfType<GameManager>().fruitState_2 = 0;
        }
    }

    public void SetSprite(int index)
    {
        GetComponent<SpriteRenderer>().sprite = sprites[index];
    }
    
    public void Eat()
    {
        CancelInvoke();
        FindObjectOfType<GameManager>().CherryEaten(this);
    }

    public void InstantiateFloatingPoint(int points)
    {
        GameObject floatingPoint = Instantiate(FloatingPoint, transform.position, Quaternion.identity);
        floatingPoint.GetComponentInChildren<TMP_Text>().text = points.ToString();
        Destroy(floatingPoint, 1.5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Pacman")){
            Eat();
        }
    }
    

}
