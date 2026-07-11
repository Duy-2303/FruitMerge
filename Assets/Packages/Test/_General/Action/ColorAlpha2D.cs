using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link
{
    public class ColorAlpha2D : MonoBehaviour
    {
        //doi mau cho bep lua
        [SerializeField] AnimationCurve curveAlpha;
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] float speed = 0.5f;
        Color color;

        void Awake()
        {
            color = spriteRenderer.color;
        }

        void LateUpdate()
        {
            color.a = curveAlpha.Evaluate(Time.time * speed);
            spriteRenderer.color = color;
        }
    }
}