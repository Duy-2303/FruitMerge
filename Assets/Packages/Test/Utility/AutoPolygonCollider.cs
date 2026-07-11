using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PolygonCollider2D))]
public class AutoPolygonCollider : MonoBehaviour
{
    [Tooltip("Higher values produce more detailed colliders, at the cost of performance. Lower values simplify the collider.")]
    [Range(1, 10)] // Adjust the range as needed
    public int detailLevel = 1;

    [ContextMenu("Detect")]
    public void UpdateCollider()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return;

        PolygonCollider2D polyCollider = GetComponent<PolygonCollider2D>();
        if (polyCollider == null) return;

        List<Vector2> spritePoints = new List<Vector2>();
        spriteRenderer.sprite.GetPhysicsShape(0, spritePoints); // Retrieve the physics shape

        // Simplify the list of points based on the detail level
        List<Vector2> simplifiedPoints = new List<Vector2>();
        for (int i = 0; i < spritePoints.Count; i += 11 - detailLevel)
        {
            simplifiedPoints.Add(spritePoints[i]);
        }

        // Here, instead of closing the loop as with EdgeCollider, we set the path for PolygonCollider
        polyCollider.SetPath(0, simplifiedPoints);
    }
}
