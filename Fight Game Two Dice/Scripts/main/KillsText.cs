using UnityEngine;
using UnityEngine.UI;

public class KillsText : MonoBehaviour
{
    private Text _text;
    private EnemySpawner _enemySpawner;

    private void Start()
    {
        _text = GetComponent<Text>();
        _enemySpawner = FindObjectOfType<EnemySpawner>();
        _enemySpawner.OnEnemyInstantientAction += UpdateText;
        UpdateText();
    }

    private void UpdateText()
    {
        _text.text = EnemySpawner.enemyCount.ToString();
    }
}
