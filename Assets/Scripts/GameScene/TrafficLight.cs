using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    [SerializeField] float changeTime = 5f;
    [SerializeField] GameObject light;

    public bool isRed = true;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= changeTime)
        {
            timer = 0;
            isRed = !isRed;

            Debug.Log(isRed ? "赤信号" : "青信号");
        }

        if (isRed)
        {
            light.SetActive(true);
        }
        else
        {
            light.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("stop");
        if (!other.CompareTag("Car")) return;

        Car car = other.GetComponent<Car>();

        if (car != null)
        {
            car.StopByTrafficLight(isRed);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("exit");
        if (!other.CompareTag("Car")) return;

        Car car = other.GetComponent<Car>();

        if (car != null)
        {
            car.StopByTrafficLight(false);
        }
    }
}