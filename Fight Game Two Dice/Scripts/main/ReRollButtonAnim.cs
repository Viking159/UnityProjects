using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReRollButtonAnim : MonoBehaviour
{

    private Image _self;

    private void Start()
    {
        _self = GetComponent<Image>();
    }

    public void AddReRollAnimStart()
    {
        StartCoroutine(AddReRollAnim());
    }

    private IEnumerator AddReRollAnim()
    {
        Color color = Color.white;
        while (color.r > 0)
        {
            color.r -= 0.05f;
            color.b -= 0.05f;
            _self.color = color;
            yield return null;
        }
        color = Color.green;
        _self.color = color;
        yield return new WaitForSeconds(0.2f);
        while (color.r < 1)
        {
            color.r += 0.05f;
            color.b += 0.05f;
            _self.color = color;
            yield return null;
        }
        color = Color.white;
        _self.color = color;
    }

}
