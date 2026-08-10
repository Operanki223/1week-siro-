using UnityEngine;

public class CarSpawnPoint : MonoBehaviour
{
    [SerializeField] float carSpawnSpeed = 3.0f;
    [SerializeField] GameObject carObj;
    [SerializeField] GameObject carParent;
    private float time;
    private bool isTime = true;
    public static CarSpawnPoint instance;

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

    }

    // Update is called once per frame
    void Update()
    {
        if (isTime)
        {
            time += Time.deltaTime;
            if (time > carSpawnSpeed)
            {
                CarSpawn();
                time = 0;
            }
        }
    }

    void CarSpawn()
    {
        Instantiate(carObj, transform.position, transform.rotation, carParent.transform);
    }

    public void StartCarSpawn()
    {
        isTime = true;
    }

    public void StopCarSpawn()
    {
        isTime = false;
    }
}
