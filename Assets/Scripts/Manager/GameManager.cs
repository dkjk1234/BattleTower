using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : Singleton<GameManager>
{
    public bool isTest = false;
    [Range(0,10)]
    public float tiemScale = 1f;
    int wave, lives, money, waveMax;
    bool gameOver = false;
    public int TowerLevelMax { get; set; }

    public int[] towerPrices = new int[4] { 70, 80, 90, 80 };

    [SerializeField]
    private GameObject waveBtn, gameCompleteUI, gameOverUI, pauseUI, panel;

    [SerializeField]
    private Text waveText, moneyText, livesText;

    private List<Monster> activeMonsters = new List<Monster>();

    public void Update()
    {
        if (isTest)
        {
            Time.timeScale = tiemScale;
        }   
    }

    public bool WaveActive
    {
        get { return activeMonsters.Count > 0; }
    }

    public int Lives
    {
        get { return lives; }
        set
        {
            lives = value;

            if (lives <= 0)
            {
                lives = 0;
                GameOver();
            }
            livesText.text = value.ToString();
        }
    }

    public int Money
    {
        get { return money; }
        set
        {
            money += value;
            moneyText.text = money.ToString();
        }
    }

    public ObjectManager objectManager;
    public DataManager dataManager;

    public Dictionary<Point, Tower> Towers { get; private set; }

    private void Start()
    {
        Time.timeScale = 1;
        Towers = new Dictionary<Point, Tower>();
    }

    /// <summary>
    /// The following is a function related to Monster Spawn
    /// </summary>

    public void StartWave()
    {
        wave++;
        waveText.text = "공격 <color=yellow>" + wave.ToString() + "</color>/" + waveMax.ToString();
        StartCoroutine(SpawnWave());
        waveBtn.SetActive(false);
    }

    IEnumerator SpawnWave()
    {
        Queue<int> monsterData = dataManager.waveData.Dequeue();
        for (int i = 0; i < LevelManager.Instance.Tile.MonsterWay.Count; i++)
            LevelManager.Instance.GeneratePath(i);

        int count = monsterData.Count;
        int wayIndex = 0;
        for (int i = 0; i < count; i++)
        {
            if (i == 0 || i % 2 == 0)
            {
                wayIndex = monsterData.Dequeue();
            }
            else
            {
                int monsterIndex = monsterData.Dequeue();
                string type = dataManager.MonsterType(monsterIndex);

                // Create monsters and health bars
                Monster monster = objectManager.GetObject(type).GetComponent<Monster>();
                HealthBar bar = objectManager.GetObject("HealthBar").GetComponent<HealthBar>();
                bar.ParentObj = monster.gameObject;
                monster.healthBar = bar;

                monster.Spawn(wayIndex);
                activeMonsters.Add(monster);
            }

            yield return new WaitForSeconds(Random.Range(1f, 2.5f));
        }
    }

    public void RemoveMonster(Monster monster)
    {
        activeMonsters.Remove(monster);
        if (!WaveActive && !gameOver)
        {
            if (wave.Equals(waveMax) && activeMonsters.Count.Equals(0))
            {
                GameComplete();
                return;
            }
            waveBtn.SetActive(true);
        }
    }

    /// <summary>
    /// The following is a function related to Game State
    /// </summary>

    public void GameStateInitial(int money, int waveMax, int towerLevelMax)
    {
        Lives = 20;
        Money = money;
        this.waveMax = waveMax;
        waveText.text = "공격 <color=yellow>0</color>/" + waveMax.ToString();
        TowerLevelMax = towerLevelMax;
    }

    public void ShowGameMenu()
    {
        pauseUI.SetActive(!pauseUI.activeSelf);
        panel.SetActive(!panel.activeSelf);
        if (!pauseUI.activeSelf)
        {
            if (!WaveActive && !waveBtn.activeSelf)
                waveBtn.SetActive(true);
            Time.timeScale = 1;
        }
        else
        {
            if (!WaveActive && !gameOver && waveBtn.activeSelf)
                waveBtn.SetActive(false);
            Time.timeScale = 0;
        }
    }

    void GameComplete()
    {
        panel.SetActive(!panel.activeSelf);
        gameCompleteUI.SetActive(true);
        if (PlayerPrefs.GetInt("AchieveLevel").Equals(LevelManager.Instance.GameLevel))
        {
            PlayerPrefs.SetInt("AchieveLevel", LevelManager.Instance.GameLevel + 1);
        }
    }

    public void GameOver()
    {
        if (!gameOver)
        {
            gameOver = true;
            Time.timeScale = 0;
            panel.SetActive(!panel.activeSelf);
            gameOverUI.SetActive(true);
            waveBtn.SetActive(false);
        }
    }

    public void GameRetry()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameQuit()
    {
        Destroy(MainMenuManager.Instance.dontDestory);
        SceneManager.LoadScene(1);
    }
}
