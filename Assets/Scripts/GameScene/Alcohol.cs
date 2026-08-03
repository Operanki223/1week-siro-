using UnityEngine;

public class Alcohol : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 100f;
    [SerializeField] private int addGauge = 10;

    private void Update()
    {
        // Y軸方向に回転し続ける
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("お酒を取った");
            GameManager.instance.AddGauge(addGauge);
            Destroy(this.gameObject);
        }
    }
}