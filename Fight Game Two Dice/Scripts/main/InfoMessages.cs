using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoMessages : MonoBehaviour
{
    public static string Message;

    private Animation _animation;
    private Text _text;

    private void Awake()
    {
        _animation = GetComponent<Animation>();
        _text = GetComponent<Text>();
    }

    public bool IsEnabled() => _text.enabled;

    public void Enable()
    {
        _animation.Stop("infoMassagesAnim");
        StopCoroutine(Disable());
        _animation.Play("infoMassagesAnim");
        _text.text = Message;
        StartCoroutine(Disable());
    }

    private IEnumerator Disable()
    {
        yield return new WaitForSeconds(2.5f);
        transform.localScale = Vector3.zero;
    }
}
