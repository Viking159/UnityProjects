using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeDetector : MonoBehaviour
{
    private GameObject _parentObject;

    private void Start()
    {
        _parentObject = transform.parent.gameObject;
        StartCoroutine(CheckInside());
    }

    private IEnumerator CheckInside()
    {
        Collider2D collider = GetComponent<Collider2D>();
        collider.enabled = false;
        while (Mathf.Abs(_parentObject.transform.position.x) > 7f ||
            Mathf.Abs(_parentObject.transform.position.y) > 5f)
        {
            yield return new WaitForSeconds(0.5f);
        }
        collider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("UpD") || collision.CompareTag("DownD"))
        {
            VertialPortal();
        }
        if (collision.CompareTag("LeftD") || collision.CompareTag("RightD"))
        {
            HorizontalPortal();
        }
    }

    public void HorizontalPortal()
    {
        if (_parentObject.transform.position.x < 0)
            _parentObject.transform.position = new Vector3(-_parentObject.transform.position.x - 0.5f, _parentObject.transform.position.y, 0);
        else
            _parentObject.transform.position = new Vector3(-_parentObject.transform.position.x + 0.5f, _parentObject.transform.position.y, 0);
    }

    public void VertialPortal()
    {
        if (_parentObject.transform.position.y < 0)
            _parentObject.transform.position = new Vector3(_parentObject.transform.position.x, -_parentObject.transform.position.y - 0.5f, 0);
        else
            _parentObject.transform.position = new Vector3(_parentObject.transform.position.x, -_parentObject.transform.position.y + 0.5f, 0);
    }

}
