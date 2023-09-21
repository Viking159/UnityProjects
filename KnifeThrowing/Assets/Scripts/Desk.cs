using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Desk : MonoBehaviour
{
    [SerializeField]
    private Vector3 _rotation;

    public event Action OnKnifeEnterEvent;

    private void Start()
    {
        gameObject.transform.localScale *= LevelDesign.DeskSkale;
        _rotation = new Vector3(0, 0, 50 * LevelDesign.DeskRotationSpeed);
    }

    void Update()
    {
        transform.Rotate(_rotation * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "knife") 
        {
            OnKnifeEnterEvent?.Invoke(); 
        }
    }

}
