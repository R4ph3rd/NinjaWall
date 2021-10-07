using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaController : MonoBehaviour
{
    static public float speed = 8f;
    static public bool inAir = false;
    private enum Sides { Left, Center, Right };
    private Sides sideOnWhichItIsStucked = Sides.Left ;
    private Vector3 deltaPos;
    private float t = 0;

    public Transform T_LeftWall;
    public Transform T_RightWall;

    public AudioClip Swoosh;
    public AudioClip ObstacleHit;
    public AudioClip DestroyObstacle;
    public AudioClip ProtectionOut;

    static public AudioClip s_DestroyObstacle;
    static public AudioClip s_ProtectionOut;
    static public AudioClip s_ObstacleHit;
    static private AudioSource NinjaSoundSource;

    public Material[] BonusIconsMaterials = new Material[3];
    private ParticleSystem BonusIcons;

    private GameObject NinjaBlock;

    Animator SlashAnimator;

    private static NinjaController NinjaInstance = null;
    private NinjaController() { }

    public static NinjaController getNinjaInstance()
    {
        if (NinjaInstance == null)
        {
            //_this = new GameManager();
            NinjaInstance = FindObjectOfType<NinjaController>();
        }
        return NinjaInstance;
    }

    static GameManager GM ;

    void Start()
    {
        GM = GameManager.getGMInstance();
        SlashAnimator = gameObject.GetComponentInChildren<Animator>();
        NinjaSoundSource = GetComponent<AudioSource>();

        s_DestroyObstacle = DestroyObstacle;
        s_ProtectionOut = ProtectionOut;
        s_ObstacleHit = ObstacleHit;
        BonusIcons = transform.GetComponentInChildren<ParticleSystem>();
        NinjaBlock = transform.GetChild(0).gameObject;

        if (NinjaInstance == null)
        {
            NinjaInstance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp("space") && !inAir)
        {
            inAir = !inAir;
            NinjaSoundSource.clip = Swoosh;
            Utils.PlaySound(NinjaSoundSource);
            SlashAnimator.ResetTrigger("Slash");
            SlashAnimator.SetTrigger("Slash");
        }

        if (Input.GetKeyUp("e"))
        {
            Debug.Log("side stucked : " + sideOnWhichItIsStucked);
            Debug.Log("in air : " + inAir);
        }

        if (inAir)
        {
            int k = sideOnWhichItIsStucked == Sides.Right ? -1 : 1;
            t += k * .7f * Time.deltaTime * speed;
            deltaPos = Vector3.Lerp(T_LeftWall.position, T_RightWall.position, t);

            transform.position = deltaPos;
        }

        if (DidItReachedOtherSide())
        {
            inAir = false;
            sideOnWhichItIsStucked = sideOnWhichItIsStucked == Sides.Left ? Sides.Right : Sides.Left;

            transform.position = sideOnWhichItIsStucked == Sides.Right ? T_RightWall.position : T_LeftWall.position;

            if (sideOnWhichItIsStucked == Sides.Right)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    private bool DidItReachedOtherSide()
    {
        if (sideOnWhichItIsStucked == Sides.Right)
        {
            return Vector3.Distance(transform.position, T_LeftWall.position) < .1;
        } else
        {
            return Vector3.Distance(transform.position, T_RightWall.position) < .1;
        }
    }

    public void DisableShields()
    {
        // remove potential shield already there and replace it (or not)
        foreach (Transform child in transform.GetChild(0).transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void SetShield(GameManager.BonusType bonus)
    {
        NinjaBlock.transform.localScale = new Vector3(.5f, .5f, .5f);

        switch (bonus)
        {
            case GameManager.BonusType.Protection:
                NinjaBlock.transform.GetChild(0).gameObject.SetActive(true);
                BonusIcons.GetComponent<ParticleSystemRenderer>().material = BonusIconsMaterials[0];
                break;
            case GameManager.BonusType.Destruction:
                NinjaBlock.transform.GetChild(1).gameObject.SetActive(true);
                BonusIcons.GetComponent<ParticleSystemRenderer>().material = BonusIconsMaterials[1];
                break;
            case GameManager.BonusType.Speed:
                NinjaBlock.transform.GetChild(2).gameObject.SetActive(true);
                BonusIcons.GetComponent<ParticleSystemRenderer>().material = BonusIconsMaterials[2];
                ToggleSpeed();
                break;
            default:
                break;
        }

        BonusIcons.Play();
    }

    static public void PlayHitSound(string soundName)
    {
        switch (soundName)
        {
            case "HitObstacle":
                NinjaSoundSource.clip = s_ObstacleHit;
                break;
            case "ProtectionOut":
                NinjaSoundSource.clip = s_ProtectionOut;
                break;
            case "DestroyObstacle":
                NinjaSoundSource.clip = s_DestroyObstacle;
                break;
            default:
                break;
        }

        Utils.PlaySound(NinjaSoundSource);
    }

    public void ToggleSpeed()
    {
        speed = speed == 8f ? 30f : 8f;
    }

}
