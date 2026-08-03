using System;
using System.Collections.Generic;
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
}

public class GameManager : MonoBehaviour
{
    [SerializeField] List<GameObject> itemList = new List<GameObject>();
    [SerializeField] List<Item> itemPointList = new List<Item>();
    [SerializeField] GameObject itemParent;
    [SerializeField] GameObject playerObjecet;
    [SerializeField] GameObject camera_player;
    [SerializeField] private Image gaugeImage;
    [SerializeField] List<GameObject> savePoints = new List<GameObject>();
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] float alcoholGaugeSpeed = 5;
    [SerializeField] int alcoholGauge = 0;
    [SerializeField] float time = 0;

    public int savePointNum = 0;
    public static GameManager instance;
    public int gaugeLimit = 100;
    public bool playGame = true;
    private Vector3 savePosition;
    private float nextTime;

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
            gameOverPanel.SetActive(true);
            playGame = false;
        }

        Timer();
        Alcoholgauge();
    }

    void Reset()
    {
        PlayerController playerController =
        playerObjecet.GetComponent<PlayerController>();

        CharacterController controller =
            playerObjecet.GetComponent<CharacterController>();

        controller.enabled = false;

        // プレイヤーを初期位置に移動
        playerObjecet.transform.position = new Vector3(0, 1.25f, 0);
        playerObjecet.transform.rotation = Quaternion.Euler(0, 0, 0);

        controller.enabled = true;

        playerController.ResetVelocity();
        playerController.ResetCamera();

        gameOverPanel.SetActive(false);
        playGame = true;
        time = 0;
        alcoholGauge = 0;
        nextTime = alcoholGaugeSpeed;
        ClearAllItem();
        ItemSpawn();
    }

    public void ContinueGame()
    {
        time = 0;
        alcoholGauge = 0;
        nextTime = alcoholGaugeSpeed;

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
        playGame = true;
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
        Debug.Log(amount + "ゲージ追加");
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
}
