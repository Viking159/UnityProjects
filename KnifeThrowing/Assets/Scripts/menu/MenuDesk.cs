using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuDesk : MonoBehaviour
{
    private Vector3 _rotation;

    private void Start()
    {
        _rotation = new Vector3(0, 0, -30);
    }

    void Update()
    {
        transform.Rotate(_rotation * Time.deltaTime);
    }
}
