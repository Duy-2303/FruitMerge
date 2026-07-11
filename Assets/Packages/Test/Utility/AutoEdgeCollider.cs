using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(EdgeCollider2D))]
public class AutoEdgeCollider : MonoBehaviour
{
    [Tooltip("Higher values produce more detailed colliders, at the cost of performance. Lower values simplify the collider.")]
    [Range(1, 10)] // Adjust the range as needed
    public int detailLevel = 1;

    [ContextMenu("Detect")]
    public void UpdateCollider()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return;

        EdgeCollider2D edgeCollider = GetComponent<EdgeCollider2D>();
        if (edgeCollider == null) return;

        List<Vector2> spritePoints = new List<Vector2>();
        spriteRenderer.sprite.GetPhysicsShape(0, spritePoints); // Retrieve the physics shape

        // Simplify the list of points based on the detail level
        List<Vector2> simplifiedPoints = new List<Vector2>();
        for (int i = 0; i < spritePoints.Count; i += 11 - detailLevel)
        {
            simplifiedPoints.Add(spritePoints[i]);
        }

        // Ensure the collider is a loop by adding the first point at the end if it's not already there
        if (simplifiedPoints[0] != simplifiedPoints[simplifiedPoints.Count - 1])
        {
            simplifiedPoints.Add(simplifiedPoints[0]);
        }

        edgeCollider.SetPoints(simplifiedPoints); // Apply the simplified points to the collider
    }
}
