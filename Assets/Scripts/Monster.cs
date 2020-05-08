﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour {   
    [SerializeField]
    private float speed;
    private bool canAttack = true;
    public bool moveStop = false;

    [SerializeField]
    private int price;

    [SerializeField]
    private Element elementType;

    private Stack<Node> path;
    private Vector3 destination;
    private Animator anim;

    [SerializeField]
    private float health, damage, attackCoolDown;
    private float currentHealth;

    public GameObject healthBar;
    public Soldier targetSoldier;
    public Image gaugeBar;

    public bool isDie {
        get { return currentHealth <= 0; }
    }

    public Point GridPosition { get; set; }




    /// <summary>
    /// 
    /// </summary>
    /// 
    private void Awake() {
        anim = GetComponent<Animator>();

    }

  
    //Spawns the monster in our world
    public void Spawn() {
        transform.position = LevelManager.Instance.greenPortal.transform.position;

        //Initialization for Health Information
        currentHealth = health;
        gaugeBar.fillAmount = 1;


        //Starts to scale the monsters
        StartCoroutine(Scale(new Vector3(0.1f, 0.1f), new Vector3(1, 1), true));
        
        //Sets the monsters path
        SetPath(LevelManager.Instance.Path);
        
        StartCoroutine(MonsterMove());
    }

    //Sclaes a monster up or down
    public IEnumerator Scale(Vector3 from, Vector3 to, bool isActive) {
        float progress = 0;

        //As long as the progress is less than 1, than we need to keep scaling
        while (progress <= 1) {
            transform.localScale = Vector3.Lerp(from, to, progress);
            progress += Time.deltaTime;
            yield return null;
        }
        //Make sure that is has the correct scale after scaling
        transform.localScale = to;


        if (!isActive)
            Release();
        else
            gameObject.SetActive(true);
    }


    //Gives the monster a path to walk on
    void SetPath(Stack<Node> newPath) {
        if (newPath != null) {
            path = newPath;
            Animate(GridPosition, path.Peek().GridPosition);
            GridPosition = path.Peek().GridPosition;
            destination = path.Pop().WorldPosition;
        }
    }


    //Makes the monster move along the given path
    IEnumerator MonsterMove() {
        while (!isDie && !moveStop) {
            transform.position = Vector2.MoveTowards(transform.position, destination, speed);
            healthBar.transform.position = transform.position + new Vector3(0, 0.43f, 0);

            //Checks if monster arrived at the destination
            if (transform.position == destination) {
                if (path != null & path.Count > 0) {
                    Animate(GridPosition, path.Peek().GridPosition);

                    //Sets the new GirdPosition and destination
                    GridPosition = path.Peek().GridPosition;
                    destination = path.Pop().WorldPosition;
                }
                else
                    break;
            }
            yield return new WaitForSeconds(0.03f);
        }
        if(moveStop) {
            Animate(GridPosition, GridPosition);
        }

        //Monsters arrive at the edge of the map without dying
        if (!isDie && GridPosition == LevelManager.Instance.purpleSpawn) {
            StartCoroutine(Scale(new Vector3(1, 1), new Vector3(0.1f, 0.1f), false));
            GameManager.Instance.Lives--;
        }
    }

    public void StartAttack(Soldier soldier) {
        if(canAttack) {
            StartCoroutine(Attack(soldier));
            canAttack = false;
        }
    }

    IEnumerator Attack(Soldier soldier) {
        while(!soldier.IsDie && soldier.gameObject.activeSelf && moveStop) {
            yield return new WaitForSeconds(attackCoolDown);
            anim.SetTrigger("MonsterAttack");
            soldier.TakeDamage(damage);
        }
        moveStop = false;
        StartCoroutine(MonsterMove());
    }


    void Animate(Point currentPos, Point newPos) {
        if(moveStop) {
            anim.SetBool("MonsterIdle", true);
            anim.SetBool("MonsterDown", false);
            return;
        }

        if (currentPos.y < newPos.y) { //Moving down
            anim.SetBool("MonsterDown", true);
        }
        else if (currentPos.y > newPos.y) {  //Moving up
            anim.SetBool("MonsterDown", false);
        }

        if (currentPos.y == newPos.y) {
            anim.SetBool("MonsterDown", true);
            if (currentPos.x > newPos.x) {  //Move to left
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else if (currentPos.x < newPos.x) {  //Move to right
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
       
    }

    public void TakeDamage(float damage, Element dmgSource) {
        if (dmgSource.Equals(elementType)) 
            damage /= 2;
        currentHealth -= damage;

        //Monster Die
        if(currentHealth <= 0) {  
            currentHealth = 0;
            GameManager.Instance.Money = price;
            GameManager.Instance.objectManager.ReleaseObject(healthBar);
            anim.SetTrigger("MonsterDie");
        }
        gaugeBar.fillAmount = currentHealth / health;
    }

    public void Release() {
        GameManager.Instance.objectManager.ReleaseObject(gameObject);
        GridPosition = LevelManager.Instance.greenSpawn;
        GameManager.Instance.RemoveMonster(this);
        canAttack = true;
        moveStop = false;
    }
}
