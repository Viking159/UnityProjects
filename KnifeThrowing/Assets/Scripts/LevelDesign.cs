using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class LevelDesign : MonoBehaviour
{
    [SerializeField]
    private GameObject _acornPrefab;
    [SerializeField]
    private GameObject _knifeIconPrefab;

    private ClickDetection _clickDetection;
    private GameObject _acorn;

    private Color _darkIconColor = new Color(0.4f, 0.4f, 0.4f);


    private int _maxKnifeCount;
    
    private static int _knifeCount;
    public static int KnifeCount{ get; private set; }
    public static float KnifeSpeed { get; private set; }
    public static float DeskSkale { get; private set; }
    public static int DeskRotationSpeed { get; private set; }
    
    private Image[] _allKnifeIcons;

    private void Awake()
    {
        if (Random.Range(0, 5) >= 1) _acorn = Instantiate(_acornPrefab);
        _maxKnifeCount = Random.Range(5, 15);
        KnifeSpeed = Random.Range(12f, 25f);
        DeskSkale = Random.Range(0.65f, 1.1f);
        DeskRotationSpeed = Random.Range(1, 5);
        DeskRotationSpeed *= (Random.Range(1, 3) == 2) ? -1 : 1;
    }

    private void Start()
    {
        _clickDetection = FindObjectOfType<ClickDetection>();
        _clickDetection.OnClickEvent += UpdateKnifesIcon;
        _clickDetection.OnClickEvent += CheckAcornCreating;
        _knifeCount = _maxKnifeCount;
        _allKnifeIcons = new Image[_knifeCount];
        CreateKnifeIcons(_knifeCount);
    }

    private void CreateKnifeIcons(int knifeCount)
    {
        for (int i = 0; i < knifeCount; i++)
        {
            GameObject obj = Instantiate(_knifeIconPrefab, FindObjectOfType<Canvas>().transform);
            _allKnifeIcons[i] = obj.GetComponent<Image>();
            obj.transform.localPosition += new Vector3(0, -70f * i, 0);
            
        }
    }

    private void CheckAcornCreating()
    {
        if (_acorn != null) 
            return;
        if (Random.Range(0, 5) <= 1) _acorn = Instantiate(_acornPrefab);
    }

    private void UpdateKnifesIcon()
    {
        _allKnifeIcons[_maxKnifeCount - (_knifeCount--)].color = _darkIconColor;
        KnifeCount = _knifeCount;
    }

    private void OnDestroy()
    {
        _clickDetection.OnClickEvent -= UpdateKnifesIcon;
        _clickDetection.OnClickEvent -= CheckAcornCreating;
    }
}
