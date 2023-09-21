using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnifeIcon : MonoBehaviour
{

    private Knife _knife;

    private void Start()
    {
        GetKnife();
    }

    private void GetKnife()
    {
        _knife = FindObjectOfType<Knife>();
        _knife.OnGameEndAction += OnGameEnd;
        _knife.OnKnifeCreating += GetKnife;
    }

    private void OnGameEnd(bool obj)
    {
        GetComponent<Image>().enabled = false;
        _knife.OnGameEndAction -= OnGameEnd;
    }
}
