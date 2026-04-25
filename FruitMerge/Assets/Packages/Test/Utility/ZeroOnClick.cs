using System.Collections;
using UnityEngine;

public class ZeroOnClick : MonoBehaviour
{
    public AudioClip[] acClick;
    private void OnMouseUpAsButton()
    {
        if (enabled)
        {
            transform.localPosition = Vector3.zero;
            AudioManager.PlaySFX(acClick);
        }
    }
}