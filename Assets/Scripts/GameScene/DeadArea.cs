using UnityEngine;

public class DeadArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("デッドエリアに侵入");
            GameManager.instance.GameOver();
        }
    }
}
