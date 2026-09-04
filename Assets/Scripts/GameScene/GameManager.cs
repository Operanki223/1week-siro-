using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Item
{
    public Vector3 spawnPoint;
    public Items item;
}

public enum Items
{
    Beer,
    Water,
}

public class GameManager : MonoBehaviour
{
    [SerializeField] List<GameObject> itemList = new List<GameObject>();
    [SerializeField] List<Item> itemPointList = new List<Item>();
    [SerializeField] GameObject itemParent;
    [SerializeField] GameObject carParent;
    [SerializeField] GameObject playerObjecet;
    [SerializeField] GameObject camera_player;
    [SerializeField] private Image gaugeImage;
    [SerializeField] List<GameObject> savePoints = new List<GameObject>();
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject finishPanel;
    [SerializeField] float alcoholGaugeSpeed = 5;
    [SerializeField] int alcoholGauge = 0;
    [SerializeField] float time = 0;
    [SerializeField] bool resetPosition = true;

    public int savePointNum = 0;
    public static GameManager instance;
    public int gaugeLimit = 100;
    public bool playGame = true;
    private Vector3 savePosition;
    private float nextTime;
    private bool isPlay = true;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextTime = alcoholGaugeSpeed;
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) || alcoholGauge > gaugeLimit - 1)
        {
            isPlay = false;
        }

        if (!isPlay)
        {
            GameOver();
        }

        Timer();
        Alcoholgauge();
    }

    void Reset()
    {
        CarSpawnPoint.instance.StartCarSpawn();

        PlayerController playerController =
        playerObjecet.GetComponent<PlayerController>();

        CharacterController controller =
            playerObjecet.GetComponent<CharacterController>();

        controller.enabled = false;

        // プレイヤーを初期位置に移動
        if (!resetPosition)
        {
            playerObjecet.transform.position = new Vector3(0, 1.25f, 0);
            playerObjecet.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        controller.enabled = true;

        playerController.ResetVelocity();
        playerController.ResetCamera();

        gameOverPanel.SetActive(false);
        FinishPanelSetActive(false);
        playGame = true;
        time = 0;
        alcoholGauge = 0;
        nextTime = alcoholGaugeSpeed;
        isPlay = true;
        Time.timeScale = 1;
        ClearAllItem();
        ItemSpawn();
        PlayerController.instance.StartPlayer();
    }

    public void ContinueGame()
    {
        time = 0;
        alcoholGauge = 0;
        nextTime = alcoholGaugeSpeed;
        CarSpawnPoint.instance.StartCarSpawn();

        if (savePointNum == 0)
        {
            Reset();
            return;
        }

        PlayerController playerController =
            playerObjecet.GetComponent<PlayerController>();

        CharacterController controller =
            playerObjecet.GetComponent<CharacterController>();

        controller.enabled = false;

        playerObjecet.transform.position =
            savePoints[savePointNum].transform.position;

        playerObjecet.transform.rotation =
            Quaternion.Euler(0, 0, 0);

        controller.enabled = true;

        playerController.ResetVelocity();
        playerController.ResetCamera();

        gameOverPanel.SetActive(false);
        FinishPanelSetActive(false);
        playGame = true;
        isPlay = true;
        Time.timeScale = 1;
        PlayerController.instance.StartPlayer();
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        playGame = false;
        CarSpawnPoint.instance.StopCarSpawn();
        ClearAllCar();
        StopGame();
    }

    public void StopGame()
    {
        playGame = false;
        Time.timeScale = 0;
    }

    void Alcoholgauge()
    {
        gaugeImage.fillAmount = (float)alcoholGauge / gaugeLimit;

        //alcoholgaugeSpeed秒で1ゲージ貯まる
        if (time >= nextTime)
        {
            nextTime += alcoholGaugeSpeed;
            AddGauge(1);
        }
    }

    void Timer()
    {
        time += Time.deltaTime;
    }

    public void AddGauge(int amount)
    {
        //Debug.Log(amount + "ゲージ追加");
        alcoholGauge += amount;
        alcoholGauge = Mathf.Clamp(alcoholGauge, 0, gaugeLimit);
    }

    public void RemoveGauge(int amount)
    {
        alcoholGauge -= amount;
        alcoholGauge = Mathf.Clamp(alcoholGauge, 0, gaugeLimit);
    }

    void ItemSpawn()
    {
        foreach (Item item in itemPointList)
        {
            Instantiate(itemList[(int)item.item], item.spawnPoint, Quaternion.identity, itemParent.transform);
        }
    }

    void ClearAllItem()
    {
        for (int i = itemParent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(itemParent.transform.GetChild(i).gameObject);
        }
    }

    void ClearAllCar()
    {
        for (int i = carParent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(carParent.transform.GetChild(i).gameObject);
        }
    }

    public void FinishPanelSetActive(bool isPanel)
    {
        finishPanel.SetActive(isPanel);
    }
}
