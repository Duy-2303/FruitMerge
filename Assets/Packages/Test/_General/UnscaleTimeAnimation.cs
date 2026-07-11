using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Link.Cooking
{
    public class UnscaleTimeAnimation : MonoBehaviour
    {
        [SerializeField] Animation anim;
        AnimationState state;

        void Awake()
        {
            state = anim[anim.clip.name];
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (Time.timeScale <= 0.1f)
            {
                state.time += Time.unscaledDeltaTime;
                anim.Sample();
            }
        }

        [Button]
        private void Setup()
        {
            if (anim == null)
            {
                anim = GetComponent<Animation>();
            }
            if (anim != null && anim.clip != null)
            {
                state = anim[anim.clip.name];
            }
        }
    }
}