using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] private Tile lightTile;
    [SerializeField] private Tile darkTile;
    [SerializeField] private Tilemap tileMap;

    private Merger merger;

    private int gridHeight;
    private int gridWidth;

    public int Height => gridHeight;
    public int Width => gridWidth;
    public Tilemap Tilemap => tileMap;
    public Cell[,] Cells { get; private set; }

    //private void Awake()
    //{
    //    gridHeight = 10;
    //    gridWidth = 10;
    //    GenerateGrid();
    //    CalculateCam();
    //    merger = FindObjectOfType<Merger>();
    //}

    public void Init(int width, int height)
    {
        gridHeight = height;
        gridWidth = width;
        GenerateGrid();
        CalculateCam();
        merger = FindObjectOfType<Merger>();
    }

    private void CalculateCam()
    {
        Vector3 pos = Vector3.back;
        pos.x = (Cells[gridHeight - 1, 0].GetPosition().x + Cells[0, gridWidth - 1].GetPosition().x) / 2f;
        pos.y = (Cells[0, 0].GetPosition().y + Cells[gridHeight - 1, gridWidth - 1].GetPosition().y) / 2f;
        Camera.main.transform.position = pos;
        Camera.main.orthographicSize = pos.y;
    }

    private void GenerateGrid()
    {
        Cells = new Cell[gridHeight, gridWidth];
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                Tile tile = Instantiate(((i + j) % 2 == 0) ? lightTile : darkTile);
                Cells[i, j] = new Cell(new Vector3Int(i, j, 0), tileMap);
                tile.name = $"tile{i}_{j}";
                tileMap.SetTile(Cells[i, j].Vector3Int, tile);
            }
        }
    }

    public Vector3 SetOnCell(Vector3 pos, Wood wood, ref Cell currCell)
    {
        Vector3Int nearCellVec = tileMap.LocalToCell(pos);
        if (Cells[nearCellVec.x, nearCellVec.y].OccupiedWith == null)
            currCell = Cells[nearCellVec.x, nearCellVec.y];
        else if (Cells[nearCellVec.x, nearCellVec.y].OccupiedWith.WoodType == wood.WoodType && Cells[nearCellVec.x, nearCellVec.y].Vector3Int != currCell.Vector3Int)
            if (merger.CheckMerge(nearCellVec.x, nearCellVec.y, wood) >= 3)
                return Vector3.zero;
        return currCell.GetPosition();
    }

}
