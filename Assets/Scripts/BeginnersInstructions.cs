using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginnersInstructions : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject TutoSpacebar;
    public GameObject TutoPause;
    public GameObject TutoBonus;
    void Start()
    {
        StartCoroutine(DisableInstructions(TutoSpacebar, 4));
        StartCoroutine(EnableInstructions(TutoBonus, 4, 5));
        StartCoroutine(EnableInstructions(TutoPause, 9, 3));
    }

    IEnumerator DisableInstructions(GameObject Instructions, int delay)
    {
        yield return new WaitForSeconds(delay);
        Instructions.SetActive(false);
    }

    IEnumerator EnableInstructions(GameObject Instructions, int delayEnable, int delayDisable)
    {
        yield return new WaitForSeconds(delayEnable);
        Instructions.SetActive(true);
        StartCoroutine(DisableInstructions(Instructions, delayDisable));
    }
}
