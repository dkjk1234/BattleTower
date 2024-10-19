using System.Collections;
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


    [SerializeField]
    private GameObject projectilePrefab;

    [SerializeField]
    private bool shootsMultiProjectiles = false;





    private void Start() {
        sr = GetComponent<SpriteRenderer>();
        canAttack = true;
        monsterList = new List<Monster>();


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


        Projectile = projectileType;
        towerPrice = GameManager.Instance.towerPrices[towerIndex];
        GameManager.Instance.Money = GameManager.Instance.Money -  towerPrice;

        range = transform.GetChild(0).GetComponent<TowerRange>();
        towerRange.enabled = false;
        range.enabled = true;
        range.GridPosition = GridPosition;
        towerRange.transform.localScale = new Vector3(4, 4.5f, 1);
        LevelManager.Instance.SpawnPoints[GridPosition].HasTower = true;
        
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


        if(level.Equals(GameManager.Instance.TowerLevelMax)) {
            LevelManager.Instance.SpawnPoints[GridPosition].TowerLevelMax = true;
            return;
        }       


        level = ++LevelManager.Instance.SpawnPoints[GridPosition].TowerLevel;
        damage++;
        DetermineProjectile();    


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
   
        float distance = float.MaxValue;  

        foreach (Transform pos in LevelManager.Instance.Tile.SoldierSpawnPos)
        {

            float _distance = Vector3.Distance(transform.position, pos.position);
      
            if (distance > _distance)
            {
                distance = _distance; 
                soldierStandardPos = pos.position; 
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
