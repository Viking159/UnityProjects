using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    public void OnClick()
    {
        PlayerPrefs.SetInt("MoneyCount", MoneyCount.GetMoney(false));
        if (gameObject.name == "nextButton")
        {
            SceneManager.LoadScene(1);
        }
        if (gameObject.name == "menuButton") 
        {
            SceneManager.LoadScene(0);
        }
    }

    public void ToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
