using UnityEngine;
using System.Collections;

public class MGAttachExistance : MonoBehaviour
{
	public GameObject target;

    [Tooltip("If set, use enabled property instead of active, ignores target game object")]
    public MonoBehaviour targetMono;

	public bool enable = true;
	public bool disable = true;
	public bool destroy = true;

	[Tooltip("If true, deactivate on activation and activate on deactivation.")]
	public bool inverted = false;

	private void OnEnable()
	{
		if (enable)
		{
			if (target != null && targetMono == null)
				target.SetActive(!inverted);
			else if (targetMono != null) 
				targetMono.enabled = !inverted;
		}
	}

	private void OnDisable()
	{
        if (disable)
        {
            if (target != null && targetMono == null)
                target.SetActive(inverted);
            else if (targetMono != null)
                targetMono.enabled = inverted;
        }
    }

	private void OnDestroy()
	{
		if (destroy && target != null)
		{
			Destroy(target);
		}
	}
}
