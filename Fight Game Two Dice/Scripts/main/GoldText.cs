using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldText : MonoBehaviour
{
    private Text _text;
    private Gold _gold;

    private void Start()
    {
        _text = GetComponent<Text>();
        _gold = FindObjectOfType<Gold>();
        _gold.OnAddGoldEvent += OnGoldValueChanded;
        _text.text = _gold.GetGold().ToString();
    }

    private void OnGoldValueChanded()
    {
        _text.text = _gold.GetGold().ToString();
    }

    private void OnDestroy()
    {
        _gold.OnAddGoldEvent -= OnGoldValueChanded;
    }

}
