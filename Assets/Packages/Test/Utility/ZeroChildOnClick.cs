using System.Collections;
using UnityEngine;

public class ZeroChildOnClick : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (transform.childCount > 0)
        {
            transform.GetChild(0).localPosition = Vector3.zero;
        }
    }
}