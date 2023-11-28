using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectInteractable : MonoBehaviour
{
    public UnityEvent InvokeOnInteract;

    public void triggerInteraction()
    {
        InvokeOnInteract.Invoke();
    }
}
