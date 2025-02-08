using System;
using UnityEngine;

public class DebugLogMessageText : MonoBehaviour
{

    public void EventDebugMessage(Component sender, object data) 
    {
        Debug.Log(data);
    }
}
