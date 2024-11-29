using UnityEngine;

public class OnCloseListener : MonoBehaviour
{
    public void OnClose()
    {
        Debug.Log("Browser is being closed during game scene!");
        // Add any specific warning logic here
        // Note: The warning message is already handled by the JavaScript code
        // in the template we modified earlier
    }
}