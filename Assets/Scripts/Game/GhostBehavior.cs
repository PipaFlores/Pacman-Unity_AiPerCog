using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Ghost))]
public abstract class GhostBehavior : MonoBehaviour
{
    public Ghost ghost { get; private set;}
    public float duration;
    private void Awake()
    {
        this.ghost = GetComponent<Ghost>();
        this.enabled = false;
    }

    public void Enable()
    {
        Enable(this.duration);
    }

    public virtual void Enable(float duration)
    {
        this.enabled = true;
        //Debug.Log(this.ghost.gameObject.name + "--Enabled " + this.GetType().Name + " Behavior");
        CancelInvoke(); // Fixing bug: ghosts leaving home before time on Roundreset
        Invoke(nameof(Disable), duration);
    }

    public virtual void Disable()
    {
        this.enabled = false;
        //Debug.Log(this.ghost.gameObject.name + "--Disabled " + this.GetType().Name + " Behavior");
        CancelInvoke();
    }
}
