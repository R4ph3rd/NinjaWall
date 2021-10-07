using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject NinjaBlock;
    static private ParticleSystem BonusIcons;
    public GameObject PauseScreen;
    static public GameObject s_PauseScreen;
    public GameObject EndgameScreen;
    static public GameObject s_EndgameScreen;
    public GameObject InGameScreen;
    static public GameObject s_InGameScreen;

    [Range(5, 50)]
    public float gameSpeed = 5f;
    private float gameSpeedStamp = 5f;
    public int nextGameSpeedStep = 40;
    public bool currentlyPlaying = false;

    static private float score = 0f;
    static public float highScore;
    public TextMeshProUGUI scoreTextObject;

    public enum BonusType { Protection, Destruction, Speed, None};
    private BonusType p_currentBonus = BonusType.None; 
    public int bonusDuration = 10;
    public bool preventDestroySameBonus = false;

    AudioSource ambiantMusic;
    private float deltaSpeed = 1.0f;
    private float t = 0;

    NinjaController Ninja;
    private static GameManager _this = null;

    public static GameManager getGMInstance()
    {
        if (_this == null)
        {
            //_this = new GameManager();
            _this = FindObjectOfType<GameManager>();
        }
        return _this;
    }

    public float Score
    {
        get
        {
            return score;
        }
        set
        {
            if (value == 0) { score = 0; }
            else
            {
                score += (int)Mathf.Min(Utils.map(value, 5, 50, 1, 10));
            }
        }
    }

    public BonusType currentBonus
    {
        get => p_currentBonus;

        set
        {
            Ninja.DisableShields();

            // ensure that speed bonus doesn't stay after its removal
            if (currentBonus == BonusType.Speed && value != BonusType.Speed)
            {
                Ninja.ToggleSpeed();
                gameSpeed = gameSpeed != gameSpeedStamp ? gameSpeedStamp : gameSpeed;
            }

            preventDestroySameBonus = p_currentBonus == value; // if previous is same as current, do not destroy shield at the end of the first coroutine which set the bonus countdown 
            p_currentBonus = value;

            switch (value)
            {
                case BonusType.Protection:
                    break;
                case BonusType.Destruction:
                    StartCoroutine(BonusDuration(value));
                    break;
                case BonusType.Speed:
                    StartCoroutine(BonusDuration(value));

                    // avoid cumulating x2 speed
                    if (gameSpeed == gameSpeedStamp) gameSpeed *= 2f;
                    break;
                default:
                    p_currentBonus = BonusType.None; // in case we set something not expected
                    break;
            }

            if (value != BonusType.None) Ninja.SetShield(value);
        }
    }

    private void Awake()
    {
        Time.timeScale = 0;
        GetComponent<Spawner>().enabled = false;
        ambiantMusic = GetComponent<AudioSource>();

        if (_this == null)
        {
            _this = this;
        }

        Ninja = NinjaController.getNinjaInstance();

        s_PauseScreen = PauseScreen;
        s_EndgameScreen = EndgameScreen;
        s_InGameScreen = InGameScreen;
    }

    void Start()
    {
        Time.timeScale = 1;
        ToggleMusic(true);

        InvokeRepeating("ScoringByTimeSpent", 1f, .3f);
        GetComponent<Spawner>().enabled = true;
        highScore = PlayerPrefs.GetFloat("highScore", 0);
    }

    void Update()
    {
        scoreTextObject.text = score.ToString();

        if (Input.GetKeyUp("p"))
        {
            Utils.pauseGame();
        }

        if (Input.GetKeyUp("v"))
        {
            Debug.Log("current bonus : " + p_currentBonus);
        }

        // smooth pitch transition when speed bonus is activated
        if (p_currentBonus == BonusType.Speed && ambiantMusic.pitch < 1.5f)
        {
            t += .5f * Time.deltaTime;
            deltaSpeed = Mathf.Lerp(1.0f, 1.5f, t);
            ambiantMusic.pitch = deltaSpeed;
        } 
        else if (p_currentBonus != BonusType.Speed && ambiantMusic.pitch > 1.0f)
        {
            t -= .7f * Time.deltaTime;
            deltaSpeed = Mathf.Lerp(1.0f, 1.5f, t);
            ambiantMusic.pitch = deltaSpeed;
        } 
        else if (ambiantMusic.pitch == 1.0f)
        {
            t = 0;
        }
    }

    public void ScoringByTimeSpent()
    {
        Score = gameSpeed;

        if (Score > highScore)
        {
            highScore = Score;
            PlayerPrefs.SetFloat("highScore", highScore);
        }

        // Game speed management by thresholds : linear progression
        if (nextGameSpeedStep < Score)
        {
            if (gameSpeed != gameSpeedStamp)
            {
                gameSpeedStamp++;
                gameSpeed += 2;
            } else
            {
                gameSpeed++;
                gameSpeedStamp = gameSpeed;
            }

            nextGameSpeedStep *= 2;

        }
    }

    public void ToggleMusic() // for ambiant music
    {
        if (ambiantMusic.isPlaying)  ambiantMusic.Pause();
        else ambiantMusic.Play();
    }

    public void ToggleMusic(bool forceChoice) // for ambiant music
    {
        if (forceChoice) ambiantMusic.Play();
        else ambiantMusic.Pause();
    }

    public bool ToggleIsPlaying()
    {
        currentlyPlaying = !currentlyPlaying;

        return currentlyPlaying;
    }

    IEnumerator BonusDuration(BonusType bonusSet)
    {
        yield return new WaitForSeconds(bonusDuration);
        Debug.Log("End of bonus");

        if (currentBonus == bonusSet) // check if another bonus has been taken and replaced this one
        {
            if (!preventDestroySameBonus)
            {
                currentBonus = BonusType.None;

                if (currentBonus == BonusType.Speed)
                {
                    Ninja.ToggleSpeed();
                }
            } else
            {
                preventDestroySameBonus = false;
            }
            //Debug.Log("Back to business as usual");
        } else
        {
            //Debug.Log("Bonus state has changed !");
        }        
    }

    public void ObstacleHit()
    {
        Debug.Log("You loose, looser ! " + currentBonus);
        Utils.endGame();
    }
}
