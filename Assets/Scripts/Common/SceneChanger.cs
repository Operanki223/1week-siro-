using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public void startScene()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void openingScene()
    {
        SceneManager.LoadScene("OpeningScene");
    }

    public void demoMapScene()
    {
        SceneManager.LoadScene("DemoMapScene");
    }
}
