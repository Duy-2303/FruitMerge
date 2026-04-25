using UnityEngine;

public class StickToScreenEdge : MonoBehaviour
{
    public Camera mainCamera;
    public float padding = 0.1f;
    public EdgePosition stickToEdge = EdgePosition.Left;

    private SpriteRenderer spriteRenderer;

    public enum EdgePosition
    {
        Left,
        Right,
        Top,
        Bottom
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void LateUpdate()
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);
        Vector3 newPosition = transform.position;

        switch (stickToEdge)
        {
            case EdgePosition.Left:
                newPosition.x = mainCamera.ViewportToWorldPoint(new Vector3(0f + padding, 0f, 0f)).x;
                break;
            case EdgePosition.Right:
                newPosition.x = mainCamera.ViewportToWorldPoint(new Vector3(1f - padding, 0f, 0f)).x;
                break;
            case EdgePosition.Top:
                newPosition.y = mainCamera.ViewportToWorldPoint(new Vector3(0f, 1f - padding, 0f)).y;
                break;
            case EdgePosition.Bottom:
                newPosition.y = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f + padding, 0f)).y;
                break;
        }

        transform.position = newPosition;
    }
}