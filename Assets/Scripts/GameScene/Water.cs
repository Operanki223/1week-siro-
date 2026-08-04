using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 100f;
    [SerializeField] private int minusGauge = 10;

    private void Update()
    {
        // Y軸方向に回転し続ける
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("回復を取った");
            GameManager.instance.RemoveGauge(minusGauge);
            Destroy(this.gameObject);
        }
    }
}
