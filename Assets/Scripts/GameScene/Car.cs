using UnityEngine;
using System.Collections.Generic;

public class Car : MonoBehaviour
{
    [SerializeField] float carSpeed = 10f;
    [SerializeField] float turnSpeed = 5f;
    [SerializeField] float stopDistance = 10f;
    [SerializeField] LayerMask carLayer;

    private bool stopByCar = false;
    private bool stopBySignal = false;

    private List<Transform> route = new List<Transform>();

    private int routeIndex = 0;
    private bool isTurning = false;


    private Quaternion targetRotation;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        StopByCar();

        if (stopByCar || stopBySignal)
            return;

        if (isTurning)
        {
            MoveRoute();
        }
        else
        {
            //transform.position += transform.forward * carSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + transform.forward * carSpeed * Time.deltaTime);
        }

        if (transform.position.x < -200 || transform.position.x > 300 ||
            transform.position.z < -200 || transform.position.z > 300)
        {
            Destroy(gameObject);
        }
    }


    // 曲がる種類を受け取る
    // turnAngle = 0(直進), 90(右折), -90(左折)
    public void SetRoute(Transform[] points, float turnAngle)
    {
        route.Clear();

        foreach (Transform point in points)
        {
            route.Add(point);
        }


        routeIndex = 0;


        // 現在の向きから回転角度を決める
        targetRotation =
            transform.rotation * Quaternion.Euler(0, turnAngle, 0);


        isTurning = true;
    }


    void MoveRoute()
    {
        if (routeIndex >= route.Count)
        {
            // 最後の向きに固定
            transform.rotation = targetRotation;

            isTurning = false;
            return;
        }


        Transform target = route[routeIndex];

        // ポイントへの方向
        Vector3 direction = target.position - transform.position;


        // ポイントを通過したか判定
        float dot = Vector3.Dot(transform.forward, direction);


        // 到達または通過した場合、次のポイントへ
        if (direction.magnitude < 2.0f || dot < 0)
        {
            routeIndex++;
            return;
        }


        // ポイント方向へ回転
        Quaternion targetRot = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRot,
            Time.deltaTime * turnSpeed
        );


        // 前進
        //transform.position += transform.forward * carSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + transform.forward * carSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("a");
            GameManager.instance.GameOver();
        }
    }

    void StopByCar()
    {
        stopByCar = false;

        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, stopDistance, carLayer))
        {
            if (hit.collider.gameObject != gameObject)
            {
                stopByCar = true;
            }
        }

        Debug.DrawRay(transform.position + Vector3.up * 0.5f,
                      transform.forward * stopDistance,
                      stopByCar ? Color.red : Color.green);
    }

    public void StopByTrafficLight(bool stop)
    {
        stopBySignal = stop;
        Debug.Log("信号：" + stop);
    }
}