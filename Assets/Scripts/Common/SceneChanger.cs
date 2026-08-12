using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public void StartScene()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void OpeningScene()
    {
        SceneManager.LoadScene("OpeningScene");
    }

    public void DemoMapScene()
    {
        SceneManager.LoadScene("DemoMapScene");
    }
}
