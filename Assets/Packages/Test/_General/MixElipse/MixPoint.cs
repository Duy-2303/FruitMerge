using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking
{
    public class MixPoint : MonoBehaviour
    {
        [SerializeField] Transform pourPoint;
        Vector3 checkPoint;
        IMixEllipse2D pan;

        // Update is called once per frame
        void LateUpdate()
        {
            if (checkPoint != pourPoint.position && Vector2.Distance(checkPoint, pourPoint.position) > 0.1f && LevelControl.Ins.IsHave(pourPoint.position, ref pan))
            {
                checkPoint = pourPoint.position;
                pan.SetTime();
                // if (!SoundControl.Ins.IsPlaying(LevelStep_1.Fx.Mix))
                // {
                //     SoundControl.Ins.PlayFX(LevelStep_1.Fx.Mix);
                // }
            }
            // else
            // {
            //     SoundControl.Ins.StopFX(LevelStep_1.Fx.Mix);
            // }
        }
    }
}