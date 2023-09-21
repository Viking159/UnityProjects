using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeText : MonoBehaviour
{
    private string _name;
    private Text _text;
    private Player _player;

    private void Start()
    {
        _name = gameObject.name;
        _text = GetComponent<Text>();
        _player = FindObjectOfType<Player>();
        UpdateUpgradeText();
    }

    public void UpdateUpgradeText()
    {
        switch (_name)
        {
            case "attackUpgradeText":
                int damage = _player.Damage;
                int upgradeDamage = damage + 5;
                _text.text = damage.ToString() + "  >>  " + upgradeDamage.ToString();
                break;
            case "defenseUpgradeText":
                int defense = _player.BlockedDamage;
                int upgradeDefense = defense + 10;
                _text.text = defense.ToString() + "  >>  " + upgradeDefense.ToString();
                break;
            default:
                Debug.Log("!!!CAUTION!!! No stat named " + _name);
                break;

        }
    }

}
