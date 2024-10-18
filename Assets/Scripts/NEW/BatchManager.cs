using System.Collections.Generic;
using UnityEngine;

public class BatchManager : MonoBehaviour
{
    public static BatchManager Instance { get; private set; }

    public Dictionary<Point, SpawnPoint> SpawnPoints { get; private set; }

    private void Awake()
    {
        Instance = this;
        // Initialize your SpawnPoints dictionary here
    }

    public Point WorldToGridPosition(Vector3 worldPosition)
    {
        // Convert world position to grid coordinates
        int x = Mathf.FloorToInt(worldPosition.x);
        int y = Mathf.FloorToInt(worldPosition.y);
        return new Point(x, y);
    }

    public Vector3 GridToWorldPosition(Point gridPosition)
    {
        // Convert grid coordinates back to world position
        float x = gridPosition.x + 0.5f; // Adjust as per your grid size
        float y = gridPosition.y + 0.5f;
        return new Vector3(x, y, 0f);
    }

    public bool IsValidTowerPosition(Point gridPosition)
    {
        // Check if the grid position is within bounds and not occupied
        return SpawnPoints.ContainsKey(gridPosition) && !SpawnPoints[gridPosition].HasTower;
    }
}
