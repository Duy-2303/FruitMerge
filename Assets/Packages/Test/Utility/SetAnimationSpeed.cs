using System.Collections;
using UnityEngine;

public class SetAnimationSpeed : MonoBehaviour
{
    public Animation anim;
    public float speed = 1;
    private void OnEnable()
    {
        anim[anim.clip.name].speed = speed;
    }
}