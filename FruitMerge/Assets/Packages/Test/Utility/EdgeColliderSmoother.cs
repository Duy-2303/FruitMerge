using UnityEngine;

public class EdgeColliderSmoother : MonoBehaviour
{
    [Range(1, 10)]
    public int subdivisions = 2;

    private EdgeCollider2D edgeCollider;
    private Vector2[] originalPoints;

    [ContextMenu("Smooth")]
    private void Smooth()
    {
        edgeCollider = GetComponent<EdgeCollider2D>();
        originalPoints = edgeCollider.points;
        SmoothEdgeCollider();
    }

    private void SmoothEdgeCollider()
    {
        Vector2[] smoothedPoints = new Vector2[(originalPoints.Length - 1) * subdivisions + 1];
        int smoothedIndex = 0;

        for (int i = 0; i < originalPoints.Length - 1; i++)
        {
            Vector2 p0, p1, p2, p3;

            if (i == 0)
            {
                p0 = originalPoints[0];
                p1 = originalPoints[0];
                p2 = originalPoints[1];
                p3 = originalPoints[2];
            }
            else if (i == originalPoints.Length - 2)
            {
                p0 = originalPoints[i - 1];
                p1 = originalPoints[i];
                p2 = originalPoints[i + 1];
                p3 = originalPoints[i + 1];
            }
            else
            {
                p0 = originalPoints[i - 1];
                p1 = originalPoints[i];
                p2 = originalPoints[i + 1];
                p3 = originalPoints[i + 2];
            }

            smoothedPoints[smoothedIndex++] = p1;

            for (int j = 1; j < subdivisions; j++)
            {
                float t = (float)j / subdivisions;
                Vector2 smoothedPoint = CatmullRomSpline(p0, p1, p2, p3, t);
                smoothedPoints[smoothedIndex++] = smoothedPoint;
            }
        }

        smoothedPoints[smoothedIndex] = originalPoints[originalPoints.Length - 1];

        edgeCollider.points = smoothedPoints;
    }

    private Vector2 CatmullRomSpline(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        float b1 = 0.5f * (-t3 + 2f * t2 - t);
        float b2 = 0.5f * (3f * t3 - 5f * t2 + 2f);
        float b3 = 0.5f * (-3f * t3 + 4f * t2 + t);
        float b4 = 0.5f * (t3 - t2);

        return b1 * p0 + b2 * p1 + b3 * p2 + b4 * p3;
    }

    //private void OnDrawGizmosSelected()
    //{
    //    if (edgeCollider != null)
    //    {
    //        Gizmos.color = Color.green;
    //        Vector2[] points = edgeCollider.points;

    //        for (int i = 0; i < points.Length - 1; i++)
    //        {
    //            Gizmos.DrawLine(points[i], points[i + 1]);
    //        }
    //    }
    //}
}