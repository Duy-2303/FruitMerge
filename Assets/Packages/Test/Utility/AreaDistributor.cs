using System.Collections.Generic;
using UnityEngine;

public class AreaDistributor : MonoBehaviour
{
    public GameObject prefab;
    public PolygonCollider2D spawnArea;
    public int minObjects = 10;
    public int maxObjects = 100;
    public FloatRange rotationRange;
    public bool spawnOnStart = true;
    [Range(0f, 1f)]
    public float randomnessFactor = 0.5f; // New parameter for randomness

    public List<GameObject> spawnedObjects = new List<GameObject>();
    private float calculatedMinDistance;

    void Start()
    {
        if (spawnOnStart) SpawnObjects();
    }

    [ContextMenu("Spawn")]
    public void SpawnObjects()
    {
        ClearSpawnedObjects();

        int objectCount = Random.Range(minObjects, maxObjects + 1);
        calculatedMinDistance = Mathf.Sqrt(spawnArea.bounds.size.x * spawnArea.bounds.size.y / objectCount);

        List<Vector2> potentialPositions = GenerateGridPositions(objectCount);

        int attempts = 0;
        while (spawnedObjects.Count < objectCount && attempts < objectCount * 10)
        {
            if (potentialPositions.Count == 0)
                break; // Avoid infinite loop

            int index = Random.Range(0, potentialPositions.Count);
            Vector2 gridPosition = potentialPositions[index];
            Vector2 randomizedPosition = gridPosition + Random.insideUnitCircle * calculatedMinDistance * randomnessFactor;
            potentialPositions.RemoveAt(index);

            if (spawnArea.OverlapPoint(randomizedPosition))
            {
                GameObject newObject = Instantiate(prefab, randomizedPosition, Quaternion.Euler(0, 0, rotationRange.GetRandomWithin()), transform);
                spawnedObjects.Add(newObject);
            }

            attempts++;
        }
    }

    private List<Vector2> GenerateGridPositions(int objectCount)
    {
        List<Vector2> positions = new List<Vector2>();

        float cols = Mathf.Ceil(spawnArea.bounds.size.x / calculatedMinDistance);
        float rows = Mathf.Ceil(spawnArea.bounds.size.y / calculatedMinDistance);

        for (float x = 0; x < cols; x++)
        {
            for (float y = 0; y < rows; y++)
            {
                Vector2 potentialPosition = new Vector2(
                    spawnArea.bounds.min.x + (x * calculatedMinDistance) + calculatedMinDistance / 2,
                    spawnArea.bounds.min.y + (y * calculatedMinDistance) + calculatedMinDistance / 2
                );

                positions.Add(potentialPosition);
            }
        }

        return positions;
    }

    private void ClearSpawnedObjects()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }
        spawnedObjects.Clear();
    }

    // show rotationRange angle in editor
    private void OnDrawGizmosSelected()
    {
        // draw the corner created by 2 vectors, using min and max angle in the rotationRange
        Vector3 center = spawnArea.bounds.center;
        Vector3 minVector = center + Quaternion.Euler(0, 0, rotationRange.min) * Vector3.right * 2;
        Vector3 maxVector = center + Quaternion.Euler(0, 0, rotationRange.max) * Vector3.right * 2;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(center, minVector);
        Gizmos.DrawLine(center, maxVector);

    }
}