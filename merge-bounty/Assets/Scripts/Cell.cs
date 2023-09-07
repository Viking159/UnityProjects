using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Cell
{
    private Tilemap tilemap;
    public Vector3Int Vector3Int { get; private set; }

    public Wood OccupiedWith { get; set; }

    public Cell(Vector3Int vector3Int, Tilemap tilemap)
    {
        Vector3Int = vector3Int;
        this.tilemap = tilemap;
        OccupiedWith = null;
    }

    public Vector3 GetPosition()
    {
        Vector3 cellPos = tilemap.CellToLocal(Vector3Int);
        return new Vector3(cellPos.x, cellPos.y + 5, cellPos.z);
    }

}
