using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatButtons : MonoBehaviour
{
    public Text[] _texts = new Text[4];

    private Gold _gold;
    private Player _player;
    private CharacterStats playerStats;

    private void Start()
    {
        _gold = FindObjectOfType<Gold>();
        _player = FindObjectOfType<Player>();
        playerStats = new CharacterStats(_player.Damage, _player.CrititcalDamage, _player.MaxHealth, _player.BlockedDamage);
    }

    public void OnButtonClick(int index)
    {
        int price = playerStats.GetPrice(index);
        if (_gold.GetGold() < price) return;
        _gold.AddGold(-price);

        playerStats.BuyStat(index);
        _texts[index].text = playerStats.GetName(index) + "\n" + playerStats.GetPrice(index).ToString();

        if (index == 2) _player.Health += 20;

        _player.UpdateStat(playerStats.GetStats());
    }
}
