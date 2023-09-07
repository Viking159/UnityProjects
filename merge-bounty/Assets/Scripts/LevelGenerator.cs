using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Transform parentWood;
    [Space]
    [Header("Префабы")]
    [SerializeField] private GameObject[] woodPrefabs;
    [Space]
    [Header("Поле")]
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [Space]
    [Header("Количество деревяшек")]
    [SerializeField] private int branchCount = 6;
    [SerializeField] private int stickCount = 4;
    [SerializeField] private int logCount = 1;
    [Header("Удалить уже существующие")]
    [SerializeField] private bool delete = true;

    private GridGenerator gridGenerator;

    private void Awake()
    {
        gridGenerator = FindObjectOfType<GridGenerator>();
        gridGenerator.Init(width, height);
        if (delete)
        {
            var woods = FindObjectsOfType<Wood>();
            foreach (var item in woods)
            {
                Destroy(item.gameObject);
            }
        }
        Invoke("SpawnWoods", 0.15f);
    }

    private void SpawnWoods()
    {
        int[] counts = new int[3] { branchCount, stickCount, logCount };
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < counts[i]; j++)
            {
                Instantiate(woodPrefabs[i], parentWood).transform.position = gridGenerator.Cells[Random.Range(0, height), Random.Range(0, width)].GetPosition();
            }
        }
    }

}
