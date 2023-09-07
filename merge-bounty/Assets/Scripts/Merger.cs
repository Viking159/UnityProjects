using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Merger : MonoBehaviour
{
    [SerializeField] private GameObject[] woodsPrefab;
    [SerializeField] private Transform parentWood;
    private GridGenerator gridGenerator;

    private List<Vector2Int> checkedPoints;
    private Vector3Int lastVector;

    private void Start()
    {
        gridGenerator = FindObjectOfType<GridGenerator>();
        checkedPoints = new List<Vector2Int>();
        lastVector = new Vector3Int(-1, 0, 0);
    }
    
    public void LightUpMerge(float posX, float posY, WoodType woodType)
    {
        Vector3Int nearCellVec = gridGenerator.Tilemap.LocalToCell(new Vector3(posX, posY, 0));
        if (nearCellVec == lastVector) return;
        for (int i = 0; i < checkedPoints.Count; i++)
            if (gridGenerator.Cells[checkedPoints[i].x, checkedPoints[i].y].OccupiedWith != null)
                gridGenerator.Cells[checkedPoints[i].x, checkedPoints[i].y].OccupiedWith.LightOff();
        lastVector = nearCellVec;
        if (gridGenerator.Cells[nearCellVec.x, nearCellVec.y].OccupiedWith == null) 
            return;
        if (gridGenerator.Cells[nearCellVec.x, nearCellVec.y].OccupiedWith.WoodType != woodType)
            return;
        checkedPoints = new List<Vector2Int>();
        if (CountSequence(new Vector2Int(nearCellVec.x, nearCellVec.y), woodType) >= 2)
            for(int i = 0; i < checkedPoints.Count; i++)
                gridGenerator.Cells[checkedPoints[i].x, checkedPoints[i].y].OccupiedWith.LightOn();
    }

    public int CheckMerge(int i, int j, Wood wood)
    {
        if ((int)wood.WoodType >= Enum.GetValues(typeof(WoodType)).Length - 2) return 0;
        checkedPoints = new List<Vector2Int>();
        int k = CountSequence(new Vector2Int(i, j), wood.WoodType) + 1;
        if (k >= 3)
        {
            Vector3 clickCellPos = gridGenerator.Cells[i, j].GetPosition();
            wood.DestroySelf(clickCellPos);
            DestroyWood(clickCellPos);
            checkedPoints = new List<Vector2Int>();
            SpawnWood(k, wood.WoodType, i , j);
        }
        return k;
    }

    private int CountSequence(Vector2Int point, WoodType woodType)
    {
        int k = 0;
        for (int i = point.x - 1; i <= point.x + 1 && i < gridGenerator.Height; i++)
        {
            if (i < 0) continue;
            for (int j = point.y - 1; j <= point.y + 1 && j < gridGenerator.Width; j++)
            {
                if (j < 0 || (((i - (point.x - 1)) + (j - (point.y - 1))) % 2 == 0)) continue;
                if (gridGenerator.Cells[i, j].OccupiedWith == null) continue;
                if (gridGenerator.Cells[i, j].OccupiedWith.WoodType == woodType && checkedPoints.Contains(new Vector2Int(i, j)) == false)
                {
                    checkedPoints.Add(new Vector2Int(i, j));
                    k += 1 + CountSequence(new Vector2Int(i, j), woodType);
                }
            }
        }
        return k;
    }

    private void DestroyWood(Vector3 pos)
    {
        for(int i = 0; i < checkedPoints.Count; i++)
            gridGenerator.Cells[checkedPoints[i].x, checkedPoints[i].y].OccupiedWith.DestroySelf(pos);
    }

    private void GetCounts(int k, out int a, out int b)
    {
        if (k < 5)
        {
            a = k / 3;
            b = k % 3;
        }
        else
        {
            if ((k - 5) % 6 != 0 && (k - 5) % 3 == 0)
                a = (k - 5) / 3 + 1;
            else
                a = (k - 5) / 3 + 2;
            if ((k - 5) % 6 == 0)
                b = 0;
            else if (a % 2 == 0)
                b = (k - 5) % 6 - 1;
            else
                b = (k - 5) % 6 - 4;
        }
    }

    private void SpawnWood(int k, WoodType woodType, int x, int y)
    {
        GetCounts(k, out int a, out int b);
        WoodType newWoodType = woodType + 1;
        for (int i = 0; i < a; i++)
            Instantiate(woodsPrefab[(int)newWoodType], parentWood).transform.position = gridGenerator.Cells[x, y].GetPosition();
        for (int i = 0; i < b; i++)
            Instantiate(woodsPrefab[(int)woodType], parentWood).transform.position = gridGenerator.Cells[x, y].GetPosition();
    }
}
