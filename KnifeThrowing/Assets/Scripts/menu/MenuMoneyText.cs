using UnityEngine;
using UnityEngine.UI;

public class MenuMoneyText : MonoBehaviour
{
    private int _moneyCount;
    private void Start()
    {
        if (PlayerPrefs.HasKey("MoneyCount")) _moneyCount = PlayerPrefs.GetInt("MoneyCount");
        else _moneyCount = 0;
        GetComponent<Text>().text = _moneyCount.ToString();
    }
}
