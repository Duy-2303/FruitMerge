using System.Collections;
using UnityEngine;

/// <summary>
/// Fit the camera to the screen to ensure all the content are shown. Screen ratio ranges from 3:4 to 9:20.
/// </summary>
public class MGCameraFitter : MonoBehaviour
{
    public FloatRange sizeRange = new FloatRange(7.2f, 9f); // for ratio from 3:4 to 9:20
    public FloatRange ratioRange = new FloatRange(9f / 20f, 3f / 4f); // for ratio from 9:20 to 3:4

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        FitCamera();
    }


    public void FitCamera()
    {
        float ratio = (float)Screen.width / Screen.height;
        float size = sizeRange.min + (sizeRange.max - sizeRange.min) * (1 - (ratio - ratioRange.min) / (ratioRange.max - ratioRange.min));
        cam.orthographicSize = size;
    }
}