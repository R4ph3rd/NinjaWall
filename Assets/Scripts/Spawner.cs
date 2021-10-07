using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject[] Spawners = new GameObject[2]; // from left to right side

    public GameObject[] Obstacles;
    public GameObject[] Bonus = new GameObject[3]; // in order : protection, destruction, speed
    public static Stack<GameObject> StackObstacles = new Stack<GameObject>();
    public static Stack<GameObject> StackBonus = new Stack<GameObject>();
    //public static Stack[] StackObjs = new Stack[4]; // obstacle, protection, 

    public enum Sides { Left, Right};
    private Sides PreviousSide = Sides.Left;
    private int HowManyObstaclesAreOnSameSide = 0 ; // [0] => side, [1] => Numbers

    [Range(0, 100)]
    public int bonusFrequence = 20; // in percent

    [Range(0, 5)]
    public float minSpawnDelay = .7f;
    [Range(0, 5)]
    public float maxSpawnDelay = 3f;
    public bool SpawnIsActive = true;

    GameManager GM;

    private Spawner() { }
    private static Spawner SpawnerInstance = null;
    public static Spawner getSpawnerInstance()
    {
        if (SpawnerInstance == null)
        {
            SpawnerInstance = FindObjectOfType<Spawner>();
        }
        return SpawnerInstance;
    }

    void Start()
    {
        GM = GameManager.getGMInstance();
        StartCoroutine(SpawnerDelay(DefineSpawnDelay()));
    }

    // Update is called once per frame
    void Update()
    {
        if (maxSpawnDelay < minSpawnDelay) maxSpawnDelay = minSpawnDelay; // ensure min doesn't exceeds max
        if (minSpawnDelay > maxSpawnDelay) minSpawnDelay = maxSpawnDelay;
    }


    void Spawn()
    {
        Sides side = (Sides)Random.Range(0, 1); // index for let or right side
        float r = Random.Range(0, 100);
        bool bonusOrObstacle = r <= bonusFrequence ? true : false;

        // avoid too much obstacle on same side
        if (PreviousSide == side)
        {
            if (HowManyObstaclesAreOnSameSide >= 4)
            {
                //side = side == Sides.Right ?
                //    Sides.Left :
                //    side == Sides.Left ?
                //    Sides.Right :
                //    Random.Range(0, 2) >= 1 ? Sides.Left : Sides.Right; // case it is center
                side = side == Sides.Right ? 
                        Sides.Left : Sides.Right; // no more center now, inchalla next version
                HowManyObstaclesAreOnSameSide = 0;
            } else
            {
                HowManyObstaclesAreOnSameSide++;
            }
        }

        if (bonusOrObstacle)
        {
            int chooseBonus = Random.Range(0, Bonus.Length);
            Instantiate(Bonus[chooseBonus], Spawners[(int)side].transform.position, Spawners[(int)side].transform.rotation);
        } else
        {
            if (StackObstacles.Count > 0)
            {
                GameObject obstacle = StackObstacles.Pop();
                obstacle.transform.position = Spawners[(int)side].transform.position;
                obstacle.transform.rotation = Spawners[(int)side].transform.rotation;
                obstacle.SetActive(true);
            }
            else
            {
                int chooseObstacle = Random.Range(0, Obstacles.Length);
                Instantiate(Obstacles[chooseObstacle], Spawners[(int)side].transform.position, Spawners[(int)side].transform.rotation);
            }
        }       
    }
    
    static public void DestroyObj(GameObject obj)
    {
        obj.SetActive(false);

        if (obj.GetComponent<Obstacle>() != null)
        {
            StackObstacles.Push(obj);
        }
    }

    IEnumerator SpawnerDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Spawn();
        if (SpawnIsActive)  StartCoroutine(SpawnerDelay(DefineSpawnDelay()));
    }

    public float DefineSpawnDelay()
    {
        return Mathf.Clamp(Utils.map(GM.gameSpeed, 5f, 10f, maxSpawnDelay, minSpawnDelay), minSpawnDelay, maxSpawnDelay);
    }
}
