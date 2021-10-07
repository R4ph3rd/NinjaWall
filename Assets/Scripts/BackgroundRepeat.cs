using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundRepeat : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject Ground, Sky_01, Sky_02;
    public Transform Destroyer;

    GameManager GM;

    private Vector3 SkySizes;
    void Start()
    {
        GM = GameManager.getGMInstance();
        Ground = gameObject.transform.GetChild(0).gameObject;
        Sky_01 = gameObject.transform.GetChild(1).gameObject;
        Sky_02 = gameObject.transform.GetChild(2).gameObject;

        SkySizes = Sky_01.transform.GetChild(0)
                                .gameObject.GetComponent<MeshCollider>().bounds.size;
    }

    // Update is called once per frame
    void Update()
    {
        if (Ground != null)
        {
            Utils.Fall(Ground.transform);

            if (IsItTheEnd(Ground))
            {
                Destroy(Ground);
            }
        }

        Utils.Fall(Sky_01.transform);
        Utils.Fall(Sky_02.transform);

        if (IsItTheEnd(Sky_01))
        {
            Vector3 pos = Sky_02.transform.position;
            pos.y += SkySizes.y;
            Sky_01.transform.position = pos;
        }

        if (IsItTheEnd(Sky_02))
        {
            Vector3 pos = Sky_01.transform.position;
            pos.y += SkySizes.y;
            Sky_02.transform.position = pos;
        }
    }

    bool IsItTheEnd(GameObject bg)
    {
        return bg.transform.position.y <= Destroyer.position.y;
    }
}
