using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeDesigner : MonoBehaviour
{

    private readonly List<Color> _colors = new List<Color>
    {
        new Color(0.7735849f, 0.3021359f, 0.3021359f, 1),
        new Color(0.7169812f, 0.2259166f, 0.6382701f, 1),
        new Color(0.227451f, 0.5413867f, 0.7176471f, 1),
        new Color(0.2274509f, 0.7176471f, 0.270036f, 1),
        new Color(0.7176471f, 0.7003939f, 0.2274509f, 1)
    };

    void Start()
    {
        GetComponent<MeshRenderer>().material.color = _colors[Random.Range(0, _colors.Count)];
    }

}
