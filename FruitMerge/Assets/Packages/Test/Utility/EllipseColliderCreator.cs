using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class EllipseColliderCreator : MonoBehaviour
{
    public float width = 1.0f;
    public float height = 2.0f;
    public int pointCount = 32;

    [ContextMenu("Create")]
    public void Create()
    {
        CreateEllipse(width, height, pointCount);
    }

    // Function to create an ellipse with an EdgeCollider2D
    public void CreateEllipse(float width, float height, int pointCount)
    {
        EdgeCollider2D edgeCollider = GetComponent<EdgeCollider2D>();
        Vector2[] points = new Vector2[pointCount + 1];

        float a = width / 2.0f; // Semi-major axis
        float b = height / 2.0f; // Semi-minor axis

        for (int i = 0; i <= pointCount; i++)
        {
            // Angle around the ellipse
            float angle = (float)i / pointCount * 2 * Mathf.PI;

            // Calculate x and ySpeed using the parametric equations of an ellipse
            float x = a * Mathf.Cos(angle);
            float y = b * Mathf.Sin(angle);

            points[i] = new Vector2(x, y);
        }

        // Assign the points to the EdgeCollider2D
        edgeCollider.points = points;
    }

}
