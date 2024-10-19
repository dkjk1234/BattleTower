﻿using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Element { ARCHER, WIZARD, BOMB, BARRACKS, NONE }

public class Tower : MonoBehaviour {
    float attackTimer;
    bool canAttack;

    public int level;

    public int towerIndex, towerPrice;

    public float projectileSpeed, attackCooldown;
    public float ProjectileSpeed { get { return projectileSpeed; } }
    public float AttackCoolDown { get => attackCooldown; }

    [SerializeField]
    private string projectileType;
    public string Projectile { get; set; }

    [SerializeField]
    private Sprite[] towerSprites;

    private SpriteRenderer sr;
    private TowerRange range;

    private Monster target;
    public Monster Target { get { return target; } }
    public List<Monster> monsterList;

    //Soldier
    public Dictionary<int, Soldier> Soldiers { get; set; }
    private Vector3 soldierStandardPos;

   

    public float damage, distance;
    public float Damage { get => damage; }

    [SerializeField]
    private Element elementType;
    public Element ElementType { get => elementType; }

    public SpriteRenderer towerRange;

    public Point GridPosition { get; set; }




    /// <summary>
    /// 
    /// </summary>

    private void Start() {
        sr = GetComponent<SpriteRenderer>();
        canAttack = true;
        monsterList = new List<Monster>();

        for (int i = 0; level > i; i++)
        {
            LevelUp();
        }
    }

    private void Update() {
        if(!ElementType.Equals(Element.BARRACKS))
            Attack();
    }

    public void Setup(Vector3 worldPos) {
        GridPosition = LevelManager.Instance.WorldToGridPosition(worldPos);
        transform.position = LevelManager.Instance.GridToWorldPosition(GridPosition) + new Vector3(0, 0.13f, 0); ;
        GameManager.Instance.Towers.Add(GridPosition, this);

        LevelManager.Instance.SpawnPoints[GridPosition].TowerLevel++;
        /*transform.position = worldPos + new Vector3(0, 0.13f, 0);
        GameManager.Instance.Towers.Add(gridPos, this);

        
        if(LevelManager.Instance.SpawnPoints[gridPos].TowerLevel.Equals(GameManager.Instance.TowerLevelMax)) 
            LevelManager.Instance.SpawnPoints[GridPosition].TowerLevelMax = true;*/

        Projectile = projectileType;
        towerPrice = GameManager.Instance.towerPrices[towerIndex];
        //GameManager.Instance.dataManager.Initialize(towerIndex, ref damage, ref projectileSpeed, ref attackCooldown);

        range = transform.GetChild(0).GetComponent<TowerRange>();
        towerRange.enabled = false;
        range.enabled = true;
        range.GridPosition = GridPosition;
        towerRange.transform.localScale = new Vector3(4, 4.5f, 1);
        LevelManager.Instance.SpawnPoints[GridPosition].HasTower = true;
        
        //Create Soldiers if it is the BARRACKS
        if(ElementType.Equals(Element.BARRACKS)) {
            Soldiers = new Dictionary<int, Soldier>();
            distance = 10000;
            CheckNeighborTiles();
            Vector3[] posArray = LevelManager.Instance.SpawnPoints[GridPosition].SetSoldierPos(soldierStandardPos);
            for(int i = 0; i < 3; i++)
                StartCoroutine(CreateSoldier(i, posArray[i], false, false, null));
        }
    }

    public void LevelUp() {
        int level = LevelManager.Instance.SpawnPoints[GridPosition].TowerLevel;

        //Stop upgrading when the tower level is at its maximum
        if(level.Equals(GameManager.Instance.TowerLevelMax)) {
            LevelManager.Instance.SpawnPoints[GridPosition].TowerLevelMax = true;
            return;
        }       
        //if(level.Equals(towerSprites.Length - 1)) {
            
        //}

        level = ++LevelManager.Instance.SpawnPoints[GridPosition].TowerLevel;
        damage++;
        DetermineProjectile();    

        //Replace with next level sprite
        sr.sprite = towerSprites[level - 1];

        if(ElementType.Equals(Element.BARRACKS)) {
            int count = Soldiers.Count;
            for(int i = 0; i < count; i++) {
                if(Soldiers[i].gameObject.activeSelf)
                    Soldiers[i].Release(false, true);
            }
        }
    }


    void Attack() {
        if(!canAttack) {
            attackTimer += Time.deltaTime;
            if(attackTimer >= attackCooldown) {
                canAttack = true;
                attackTimer = 0;

            }
        }

        if(target == null && monsterList.Count > 0) {
            target = monsterList[0];
            monsterList.Remove(target);
        }

        if(target != null && target.gameObject.activeSelf) {
            if(canAttack) {
                Shoot();
                canAttack = false;
            }
        }
        else if(monsterList.Count > 0) {
            target = monsterList[0];
            monsterList.Remove(target);
        }

        if((target != null && target.isDie) || (target != null && !target.gameObject.activeSelf)) {
            target = null;
        }
    }

    void Shoot() {
        Projectile proj = GameManager.Instance.objectManager.GetObject(Projectile).GetComponent<Projectile>();
        proj.transform.position = GameManager.Instance.Towers[GridPosition].transform.position;
        proj.Initialize(this);
    }

    private void CheckNeighborTiles()
    {
        // distance 변수를 각 타워마다 초기화
        float distance = float.MaxValue;  // 가장 가까운 위치를 찾기 위한 초기값 설정

        foreach (Transform pos in LevelManager.Instance.Tile.SoldierSpawnPos)
        {
            // 현재 타워 위치와 병사 스폰 위치 간의 거리 계산
            //float _distance = Vector3.Distance(LevelManager.Instance.SpawnPoints[GridPosition].WorldLocation, pos.position);
            float _distance = Vector3.Distance(transform.position, pos.position);
            // 더 가까운 위치가 있으면 soldierStandardPos를 업데이트
            if (distance > _distance)
            {
                distance = _distance;  // 새로운 최소 거리를 저장
                soldierStandardPos = pos.position;  // 가까운 위치로 업데이트

                // Debug 로그로 타워의 월드 위치 출력
                Debug.Log(LevelManager.Instance.SpawnPoints[GridPosition].WorldLocation);
            }
        }
    }


    public void RecreateSoldier(int index, bool haveTimer, bool isLevelUp, Monster target) {
        StartCoroutine(CreateSoldier(index, LevelManager.Instance.SpawnPoints[GridPosition].SoldierPos[index], haveTimer, isLevelUp, target));
    }

    IEnumerator CreateSoldier(int index, Vector3 pos, bool haveTimer, bool isLevelUp, Monster target) {
        if(haveTimer && !isLevelUp)
            yield return new WaitForSeconds(attackCooldown);
        Soldier soldier = GameManager.Instance.objectManager.GetObject(Projectile).GetComponent<Soldier>();
        HealthBar bar = GameManager.Instance.objectManager.GetObject("HealthBar").GetComponent<HealthBar>();
        bar.ParentObj = soldier.gameObject;

        if(isLevelUp && target != null) {
            soldier.Target = target;
            soldier.isMoved = true;
        }
        soldier.transform.position = transform.position;
        soldier.healthBar = bar;
        soldier.SoldierIndex = index;
        soldier.SpawnPos = pos;

        Soldiers.Add(index, soldier);
        soldier.Initialize(this, !isLevelUp);
    }

    public void ChangeSoldierPos(Vector3 pos) {
        soldierStandardPos = pos;
        Vector3[] posArray = LevelManager.Instance.SpawnPoints[GridPosition].SetSoldierPos(soldierStandardPos);
        for(int i = 0; i < Soldiers.Count; i++) {
            if(Soldiers.ContainsKey(i) && Soldiers[i].gameObject.activeSelf) {
                Soldiers[i].SpawnPos = posArray[i];
                Soldiers[i].ChangePos();
            }
        }
    }

    void DetermineProjectile() {
        print(LevelManager.Instance.SpawnPoints[GridPosition].TowerLevel);
        Projectile = projectileType +
            (GameManager.Instance.dataManager.ProjectileType(towerIndex, LevelManager.Instance.SpawnPoints[GridPosition].TowerLevel)).ToString();
    }

    public void MonsterInRange(Monster monster) { monsterList.Add(monster); }

    public void MonsterOutRange(Monster monster, bool isTarget) {
        monsterList.Remove(monster);
        if(isTarget)
            target = null;
    }

    public bool IsMonsterInRange(Monster monster) { return monsterList.Contains(monster); }

    public void Release() {
        GameManager.Instance.objectManager.ReleaseObject(gameObject);

        if(ElementType.Equals(Element.BARRACKS)) {
            int count = Soldiers.Count;
            for(int i = 0; i < count; i++) {
                if(Soldiers[i].gameObject.activeSelf)
                    Soldiers[i].Release(false, false);
            }
            Soldiers.Clear();
        }
        monsterList.Clear();

        sr.sprite = towerSprites[0];
        LevelManager.Instance.SpawnPoints[GridPosition].TowerLevel = 0;
        LevelManager.Instance.SpawnPoints[GridPosition].TowerLevelMax = false;
        GameManager.Instance.Towers.Remove(GridPosition);  //Delete from dictionary   

    }
}
