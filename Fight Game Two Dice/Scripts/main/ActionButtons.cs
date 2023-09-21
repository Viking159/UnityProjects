using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Helper;

public class ActionButtons : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _atcionButtons;

    private Player _player;
    private Actions _actions;
    private DiceRoller _diceRoller;

    private readonly Dictionary<EAction, int> _actionEnergy = new Dictionary<EAction, int>()
    {
        {EAction.ATTACK, 10 },
        {EAction.BLOCK, 0 },
        {EAction.DODGE, 10 },
        {EAction.REST, 0 }
    };

    public static bool Activated { get; private set; }

    private void Start()
    {
        Activated = false;
        _player = FindObjectOfType<Player>();
        _actions = FindObjectOfType<Actions>();
        _diceRoller = FindObjectOfType<DiceRoller>();
    }

    public void Activate(Dictionary<EAction, int> results)
    {
        Activated = true;
        int[] xPositions = new int[results.Count];
        int i = 0, j = 0;

        switch (results.Count)
        {
            case 1:
                xPositions[0] = 0;
                break;
            case 2:
                xPositions[0] = -300; xPositions[1] = 300;
                break;
            case 3:
                xPositions[0] = -400; xPositions[1] = 0; xPositions[2] = 400;
                break;
            case 4:
                xPositions[0] = -520; xPositions[1] = -173; xPositions[2] = 173; xPositions[3] = 520;
                break;
        }
        foreach (var act in _actionEnergy)
        {
            if (results.ContainsKey(act.Key))
            {
                _atcionButtons[i].transform.localPosition = new Vector3(xPositions[j], 0, 0);
                _atcionButtons[i].GetComponentInChildren<Text>().text = results[act.Key].ToString();
                _atcionButtons[i].SetActive(true);
                if (_player.Energy < act.Value)
                {
                    _atcionButtons[i].GetComponent<Button>().enabled = false;
                    _atcionButtons[i].transform.GetChild(2).gameObject.SetActive(true);
                }
                j++;
            }
            i++;
        }
        CheckAnyAvailableAction(results);
    }

    public void DeactivateAll()
    {
        foreach (GameObject btn in _atcionButtons)
        {
            if (btn.activeInHierarchy) btn.SetActive(false);
            btn.transform.GetChild(2).gameObject.SetActive(false);
            btn.GetComponent<Button>().enabled = true;
        }
        Activated = false;
    }

    private void CheckAnyAvailableAction(Dictionary<EAction, int> results)
    {
        foreach (var result in results)
        {
            if (_actionEnergy[result.Key] <= _player.Energy)
                return;
        }
        if (_diceRoller.RollCount == 0)
            StartCoroutine(SkipAction());
        
    }

    private IEnumerator SkipAction()
    {
        yield return new WaitForSeconds(1f);
        DeactivateAll();
        _actions.NotEnoughEnergy();
    }

}
