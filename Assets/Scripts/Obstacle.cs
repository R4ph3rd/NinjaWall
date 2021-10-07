using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public AudioClip DestroyObstacle;
    public AudioClip ProtectionOut;

    public ParticleSystem Explosion;

    private bool isColliding = false;

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

        if (isColliding) isColliding = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isColliding) return;
        isColliding = true;

        if (collision.gameObject.CompareTag("shield"))
        {
            switch (GM.currentBonus)
            {
                case GameManager.BonusType.Protection:
                    NinjaController.PlayHitSound("ProtectionOut");

                    Explode();
                    Spawner.DestroyObj(gameObject);
                    Spawner.DestroyObj(collision.gameObject);
                    GM.Score += 50;
                    break;
                case GameManager.BonusType.Destruction:
                    NinjaController.PlayHitSound("DestroyObstacle");

                    Explode();
                    Spawner.DestroyObj(gameObject);
                    GM.Score += 30; // add points to the score
                    break;
                default:
                    break;
            }
        }
        else if (collision.gameObject.CompareTag("ninja"))
        {
            NinjaController.PlayHitSound("HitObstacle");
            GM.ObstacleHit();
        }
        else if (collision.gameObject.CompareTag("destroyer"))
        {
            Spawner.DestroyObj(gameObject);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isColliding = false;
    }

    void Explode()
    {
        Vector3 offsetPos = transform.position;
        
        if (offsetPos.x > 0) offsetPos.x -= .7f;
        else offsetPos.x += .7f;

        ParticleSystem explosion = Instantiate(Explosion, offsetPos, transform.rotation);
        StartCoroutine(RemoveExplosion(explosion));
    }

    static IEnumerator RemoveExplosion(ParticleSystem explosion)
    {
        yield return new WaitForSeconds(10);
        Destroy(explosion);
    }

}
