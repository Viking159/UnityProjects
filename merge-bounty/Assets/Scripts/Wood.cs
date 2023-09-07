using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public enum WoodType
{
    None = -1, Stick, Branch, Log
}

public class Wood : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    private SpriteRenderer backlight;
    private Camera mainCam;
    private Tween tween;

    private Cell currentCell;
    private Merger merger;
    private GridGenerator gridGenerator;

    public Cell CurrCell { get { return currentCell; } protected set { currentCell = value; } }

    private bool selected;
    public WoodType WoodType { get; set; }

    protected void Init()
    {
        gameObject.transform.localScale = Vector3.zero;
        gameObject.transform.DOScale(1f, 1.5f);
        mainCam = Camera.main;
        selected = false;
        backlight = transform.GetChild(0).GetComponent<SpriteRenderer>();
        gridGenerator = FindObjectOfType<GridGenerator>();
        merger = FindObjectOfType<Merger>();
        Invoke("FindSelfCell", 0.1f);
    }

    private void FindSelfCell()
    {
        for (int k = 0; k < 15; k++)
        {
            for (int i = 0; i < gridGenerator.Height; i++)
            {
                for (int j = 0; j < gridGenerator.Width; j++)
                {
                    float distance = Vector3.Distance(transform.position, gridGenerator.Cells[i, j].GetPosition());

                    if (distance < 5f + 4 * k && gridGenerator.Cells[i, j].OccupiedWith == null)
                    {
                        currentCell = gridGenerator.Cells[i, j];
                        currentCell.OccupiedWith = this;
                        transform.position = currentCell.GetPosition();
                        return;
                    }
                }
            }
        }
        Debug.LogWarning("FindSelfCell null error! Wood Destroyed");
        Destroy(gameObject);
    }

    private void OnMouseDrag()
    {
        if (selected)
        {
            var plane = new Plane(Vector3.forward, Vector3.zero);
            Vector3 pointPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float pos))
            {
                
                Vector3 worldsPos = ray.GetPoint(pos);
                Vector3Int cellPos = gridGenerator.Tilemap.LocalToCell(worldsPos);
                if (cellPos.x < 0 || cellPos.x >= gridGenerator.Height || cellPos.y < 0 || cellPos.y >= gridGenerator.Width)
                    return;
                transform.position = worldsPos;
                if ((int)WoodType < Enum.GetValues(typeof(WoodType)).Length - 2)
                    merger.LightUpMerge(worldsPos.x, worldsPos.y, WoodType);
            }
        }
    }

    public void LightOn()
    {
        tween.Kill();
        tween = backlight.DOFade(0.7f, 0.3f);
    }

    public void LightOff()
    {
        tween.Kill();
        tween = backlight.DOFade(0, 0.6f);
    }

    private void OnMouseDown()
    {
        currentCell.OccupiedWith = null;
        selected = true;
        LightOn();
    }

    private void OnMouseUp()
    {
        selected = false;
        LightOff();
        Vector3 pos = gridGenerator.SetOnCell(transform.position, this, ref currentCell);
        if (pos != Vector3.zero)
            transform.DOMove(pos, 0.5f);
        currentCell.OccupiedWith = this;
    }

    public void DestroySelf(Vector3 pos)
    {
        currentCell.OccupiedWith = null;
        float time = 0.7f;
        gameObject.transform.DOMove(pos, time / 2f);
        gameObject.transform.DOScale(0, time);
        Destroy(gameObject, time * 1.1f);
    }

}
