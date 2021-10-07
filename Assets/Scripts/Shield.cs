using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public int IndexShieldType;

    GameManager GM;

    void Start()
    {
        GM = GameManager.getGMInstance();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        if (GM.currentBonus == (GameManager.BonusType)IndexShieldType && !GM.preventDestroySameBonus)
        {
            GM.currentBonus = GameManager.BonusType.None;
        }
    }
}
