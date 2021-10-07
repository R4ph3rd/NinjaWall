using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Utils : MonoBehaviour
{
    static GameObject pauseScreen;
    static GameObject InGameScreen;

    static GameManager GM;
    static Spawner SP;
    void Start()
    {
        Initialize();
    }

    static private void Initialize()
    {
        if (GM == null) GM = GameManager.getGMInstance();
        if (SP == null) SP = Spawner.getSpawnerInstance();
        if (pauseScreen == null) pauseScreen = GameManager.s_PauseScreen;
        if (InGameScreen == null) InGameScreen = GameManager.s_InGameScreen;
    }

    static public void PlaySound(AudioSource sound)
    {
        if (sound.isPlaying)
        {
            sound.time = 0;
            sound.Play();
        } 
        else 
        {
            sound.Play();
        }
    }

    static public void startGame()
    {
        Initialize();

        InGameScreen.SetActive(true);
        Time.timeScale = 1;
        GM.ToggleIsPlaying();
        GM.ToggleMusic();
    }

    public void NewStartGame()
    {
        SceneManager.LoadScene("Assets/Scenes/Game.unity");
        //Time.timeScale = 1;
        //GM.ToggleIsPlaying();
        //GM.ToggleMusic();
        //SceneManager.LoadScene("Game");
        //EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");
    }

    static public void pauseGame()
    {
        Initialize();

        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            pauseScreen.SetActive(true);
            GM.ToggleMusic();
        }   
    }

    static public void resumeGame()
    {
        Initialize();

        if (Time.timeScale == 0)
        {
            pauseScreen.SetActive(false);
            Time.timeScale = 1;
            GM.ToggleMusic();
        }
    }

    static public void quitGame()
    {
        Initialize();

        GM.ToggleIsPlaying();
        Application.Quit();
    }

    static public void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        Initialize();
        GM.Score = 0;
        Time.timeScale = 1;
        GM.ToggleMusic();
        InGameScreen.SetActive(true);
    }

    static public void endGame()
    {
        Initialize();
        Time.timeScale = 0;
        GM.ToggleMusic();
        Instantiate(GameManager.s_EndgameScreen);
    }

    static public float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    static public void Fall(Transform obj)
    {
        Initialize();

        if (obj != null)
        {
            obj.Translate(Vector3.down * Time.deltaTime * GM.gameSpeed);
        }
    }
}
