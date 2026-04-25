using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class OnClickAction : MonoBehaviour
{
    public UnityEvent onClick;

    private void OnMouseUpAsButton()
    {
        onClick?.Invoke();
    }
}