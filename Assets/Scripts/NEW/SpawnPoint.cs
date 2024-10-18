using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint
{
    public bool HasTower { get; set; }
    public int TowerLevel { get; set; }
    public bool TowerLevelMax { get; set; }

    // Other properties and methods...

    public Vector3 WorldLocation { get; set; }

    public Vector3[] SetSoldierPos(Vector3 soldierStandardPos)
    {
        // Implement logic to set soldier positions
        return new Vector3[3]; // Placeholder
    }
}

