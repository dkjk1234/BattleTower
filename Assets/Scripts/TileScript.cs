﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class WayPoints {
    public Transform[] CheckPoints;
}

public class TileScript : MonoBehaviour {
    //Tower
    public int TowerLevel { get; set; }
    public bool TowerLevelMax { get; set; }

    public Vector3[] SoldierPos { get; set; }
    public Vector2 WorldPostion { get { return WorldLocation + new Vector3(0.5f, 0.5f); } }

    public Point GridPosition { get; set; }

    public Vector3Int LocalPlace { get; set; }

    public Vector3 WorldLocation { get; set; }
    
    public bool HasTower { get; set; }
    
    public TileBase TileBase { get; set; }

    public Tilemap TilemapMember { get; set; }

    public Transform[] GreenPortalPos;       
    public Transform PurplePortalPos;

    public Transform[] SoldierSpawnPos;
    
    [SerializeField]
    List<WayPoints> monsterWay;
    public List<WayPoints> MonsterWay { get => monsterWay; }

    private void Awake()
    {
        
    }



    public Vector3[] SetSoldierPos(Vector3 standardPos) {
        SoldierPos = new Vector3[3];
        SoldierPos[0] = standardPos + new Vector3(-0.3f, 0.3f, 0);
        SoldierPos[1] = standardPos + new Vector3(0, -0.3f, 0);
        SoldierPos[2] = standardPos + new Vector3(0.3f, 0.3f, 0);
        return SoldierPos;
    }


}
