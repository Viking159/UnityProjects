using UnityEngine;
using UnityEngine.SceneManagement;

public class EndButtons : MonoBehaviour
{

    public void ToMenu()
    {
        PlayerPrefs.SetString("SceneName", "menu");
        SceneManager.LoadScene("loading");
    }
}
