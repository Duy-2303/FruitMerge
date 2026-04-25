using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link
{
    [System.Serializable]
    public class Ellipse2D
    {
        public float angleBase;
        private float radius;
        public float xAxis;
        public float yAxis;

        public Ellipse2D(float xAxis, float yAxis)
        {
            this.xAxis = xAxis;
            this.yAxis = yAxis;
            this.angleBase = 0;
            this.radius = 1;
        }

        public Ellipse2D(float xAxis, float yAxis, Vector3 point)
        {
            this.xAxis = xAxis;
            this.yAxis = yAxis;
            this.angleBase = GetAngle(point);
            this.radius = GetRadius(point);
        }

        public Vector2 Evaluate(float t, float radius)
        {
            float angle = Mathf.Deg2Rad * 360f * t + this.angleBase;
            float x = Mathf.Sin(angle) * xAxis * radius;
            float y = Mathf.Cos(angle) * yAxis * radius;
            return new Vector2(x, y);
        }

        public Vector2 Evaluate(float t)
        {
            return Evaluate(t, radius);
        }

        public float GetAngle(Vector2 point)
        {
            return Mathf.Atan2(point.x / xAxis, point.y / yAxis);
        }

        public float GetRadius(Vector3 point)
        {
            return Vector2.Distance(point, Vector2.zero) / Vector2.Distance(Evaluate(0, 1), Vector2.zero);
        }

        public void OnDrawGizmos(Vector2 center)
        {
            Gizmos.DrawSphere(Evaluate(0) + center, 0.1f);

            for (int i = 0; i < 20; i++)
            {
                Gizmos.DrawLine(Evaluate(i * 0.05f) + center, Evaluate((i + 1) * 0.05f )+ center);
            }
        }
    }
}