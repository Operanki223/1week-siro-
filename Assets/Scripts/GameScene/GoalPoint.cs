using UnityEngine;

public class GoalPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("クリア");
            GameManager.instance.StopGame();
            GameManager.instance.FinishPanelSetActive(true);
        }
    }
}
