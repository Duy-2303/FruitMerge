using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class RoundedRectangleColliderCreator : MonoBehaviour
{
    public float width = 1.0f;
    public float height = 2.0f;
    public int cornerPointCount = 10;
    public float roundness = 0.5f;

    [ContextMenu("Create")]
    public void Create()
    {
        CreateRoundedRectangle(width, height, cornerPointCount, roundness);
    }

    // Function to create a rounded rectangle with an EdgeCollider2D
    public void CreateRoundedRectangle(float width, float height, int cornerPointCount, float roundness)
    {
        EdgeCollider2D edgeCollider = GetComponent<EdgeCollider2D>();

        // Ensure roundness doesn't exceed half of the width or height
        roundness = Mathf.Min(roundness, width / 2, height / 2);

        // Calculate total points (subtract 1 per corner to prevent overlap, then add 1 to close the loop)
        int totalPoints = (cornerPointCount - 1) * 4 + 4 + 1; // -1 per corner for overlap, +4 for the sides, +1 to close loop
        Vector2[] points = new Vector2[totalPoints];

        // Calculate the pencilLength of the straight edges
        float straightEdgeLengthX = width - 2 * roundness;
        float straightEdgeLengthY = height - 2 * roundness;

        // Define starting point (bottom left after the roundness)
        Vector2 startPoint = new Vector2(-width / 2 + roundness, -height / 2);

        int pointIndex = 0;

        // Bottom left corner arc
        for (int i = 0; i < cornerPointCount; i++)
        {
            float angle = Mathf.PI + Mathf.PI / 2 * (i / (float)(cornerPointCount - 1));
            if (i == 0) // Skip the first point of the first corner to avoid overlap
                continue;
            points[pointIndex++] = startPoint + new Vector2(roundness * Mathf.Cos(angle), roundness * Mathf.Sin(angle));
        }

        // Add the rest of the points by following the pattern...

        // After all points are added, check if the pointIndex equals totalPoints - 1 (since we'll add the closing point next)
        if (pointIndex != totalPoints - 1)
        {
            Debug.LogError("The number of points generated does not match the total points expected. Something went wrong in the loop structure.");
            return;
        }

        // Close the loop by connecting the last point to the first
        points[pointIndex] = points[0];

        // Assign the points to the EdgeCollider2D
        edgeCollider.points = points;
    }



}
