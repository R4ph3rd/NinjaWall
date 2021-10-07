using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour
{
    public int IndexShieldType;
    GameManager GM;

    // Start is called before the first frame update
    void Start()
    {
        GM = GameManager.getGMInstance();
    }

    // Update is called once per frame
    void Update()
    {
        Utils.Fall(transform);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ninja"))
        {
            GM.currentBonus = (GameManager.BonusType)IndexShieldType;
            Destroy(gameObject);
        } 
        else if (collision.gameObject.CompareTag("destroyer"))
        {
            Destroy(gameObject);
        }
    }
}
