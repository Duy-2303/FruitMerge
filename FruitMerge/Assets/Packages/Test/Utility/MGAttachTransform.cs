using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class MGAttachTransform : MonoBehaviour
{
    public static int screenHalfWidth = Screen.width / 2;
    public static int screenHalfHeight = Screen.height / 2;

    public bool disableOnTargetGone; // auto disable when target is disabled or destroyed
    public bool stayInScreen; // stay inside screen even when target moves outside

    //public bool useLateUpdate; // to override animation

    [Tooltip("the object to be followed")]
	public Transform target;
    [Tooltip("the object to be moved, if not set, it will be using this.transform")]
	public Transform attacher;

	public float lerpSpeedPos; // 0 means no lerp
	public float lerpSpeedRot; // 0 means no lerp
	public float lerpSpeedScale; // 0 means no lerp
	
    public Vector3 offsetPos;
    public bool useLocal;
    [Tooltip("Only set position if you don't set posXonly and posYonly")]
    public bool position;
    public bool posXonly;
    public bool posYonly;
    public bool rotation;
	public bool scale;

	//// Use this for initialization
	//void Start()
	//{

	//}

	//// Update is called once per frame
	//void Update()
	//{
	//	if (target == null) return;

	//	SyncPos(lerpSpeedPos);

	//	SyncRot(lerpSpeedRot);
	//}

	private void LateUpdate()
	{
        if (disableOnTargetGone && (target == null || !target.gameObject.activeInHierarchy))
        {
            if (attacher != null) attacher.gameObject.SetActive(false);
            return;
        }
		if (target == null) return;
		if (attacher == null) attacher = transform;

		if (position) SyncPos(lerpSpeedPos);

		if (rotation) SyncRot(lerpSpeedRot);

		if (scale) SyncScale(lerpSpeedScale);
	}

    private const float inScreenOffsetX = 0.8f;
    private const float inScreenOffsetY = 0.8f;
    [ContextMenu("Sync Pos")]
    public void _SyncPos()
    {
        SyncPos();
    }
	public void SyncPos(float lerpSpeedPos = 0)
	{

        if (useLocal)
        {
            if (target.localPosition + offsetPos == attacher.localPosition) return;
            if (!posXonly && !posYonly)
            {
                if (lerpSpeedPos == 0)
                {
                    attacher.localPosition = target.localPosition + offsetPos;
                }
                else
                {
                    attacher.localPosition = Vector3.Lerp(attacher.localPosition, target.localPosition + offsetPos, Time.deltaTime * lerpSpeedPos);
                }
            }
            else if (posYonly && target.localPosition.y + offsetPos.y != attacher.localPosition.y)
            {
                if (lerpSpeedPos == 0)
                {
                    attacher.localPosition = new Vector3(attacher.localPosition.x, target.localPosition.y + offsetPos.y, attacher.localPosition.z);
                }
                else
                {
                    attacher.localPosition = Vector3.Lerp(attacher.localPosition, new Vector3(attacher.localPosition.x, target.localPosition.y + offsetPos.y, attacher.localPosition.z), Time.deltaTime * lerpSpeedPos);
                }
            }
            else if (posXonly && target.localPosition.x + offsetPos.x != attacher.localPosition.x)
            {
                if (lerpSpeedPos == 0)
                {
                    attacher.localPosition = new Vector3(target.localPosition.x + offsetPos.x, attacher.localPosition.y, attacher.localPosition.z);
                }
                else
                {
                    attacher.localPosition = Vector3.Lerp(attacher.localPosition, new Vector3(attacher.localPosition.y, target.localPosition.x + offsetPos.x, attacher.localPosition.z), Time.deltaTime * lerpSpeedPos);
                }
            }
        }
        else
        {
            Vector3 targetPos = stayInScreen ? 
                new Vector3(Mathf.Clamp(target.position.x, -screenHalfWidth + inScreenOffsetX, screenHalfWidth - inScreenOffsetX), 
                Mathf.Clamp(target.position.y, -screenHalfHeight + inScreenOffsetY, screenHalfHeight - inScreenOffsetY)) 
                : target.position;
            if (targetPos + offsetPos == attacher.position) return;
            if (!posXonly && !posYonly)
            {
                if (lerpSpeedPos == 0)
                {
                    attacher.position = targetPos + offsetPos;
                }
                else
                {
                    attacher.position = Vector3.Lerp(attacher.position, targetPos + offsetPos, Time.deltaTime * lerpSpeedPos);
                }
            }
            else if (posYonly && targetPos.y + offsetPos.y != attacher.position.y)
            {
                if (lerpSpeedPos == 0)
                {
                    attacher.position = new Vector3(attacher.position.x, targetPos.y + offsetPos.y, attacher.position.z);
                }
                else
                {
                    attacher.position = Vector3.Lerp(attacher.position, new Vector3(attacher.position.x, targetPos.y + offsetPos.y, attacher.position.z), Time.deltaTime * lerpSpeedPos);
                }
            }
            else if (posXonly && targetPos.x + offsetPos.x != attacher.position.x)
            {
                if (lerpSpeedPos == 0)
                {
                    attacher.position = new Vector3(targetPos.x + offsetPos.x, attacher.position.y, attacher.position.z);
                }
                else
                {
                    attacher.position = Vector3.Lerp(attacher.position, new Vector3(targetPos.x + offsetPos.x, attacher.position.y, attacher.position.z), Time.deltaTime * lerpSpeedPos);
                }
            }
        }
		
    }

	public void SyncRot(float lerpSpeedRot = 0)
	{
		if (target.rotation != attacher.rotation)
		{
            if (useLocal)
            {
                if (lerpSpeedRot == 0)
                {
                    attacher.localRotation = target.localRotation;
                }
                else
                {
                    attacher.localRotation = Quaternion.Lerp(attacher.localRotation, target.localRotation, Time.deltaTime * lerpSpeedRot);
                }
            }
            else
            {
                if (lerpSpeedRot == 0)
                {
                    attacher.rotation = target.rotation;
                }
                else
                {
                    attacher.rotation = Quaternion.Lerp(attacher.rotation, target.rotation, Time.deltaTime * lerpSpeedRot);
                }
            }
			
		}
	}

	public void SyncScale(float lerpSpeedScale = 0)
	{
		if (target.localScale != attacher.localScale)
		{
			if (lerpSpeedScale == 0)
			{
				attacher.localScale = target.localScale;
			}
			else
			{
				attacher.localScale = Vector3.Lerp(attacher.localScale, target.localScale, Time.deltaTime * lerpSpeedScale);
			}
		}
	}
}
