using UnityEngine;

public class Events : MonoBehaviour
{
    public void GameOver()
    {
        GameManager.instance.GameOver();
    }
}
