using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EngameScores : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI myScoreText;
    public TextMeshProUGUI highestScoreText;

    GameManager GM;

    void Start()
    {
        GM = GameManager.getGMInstance();
        highestScoreText.text = PlayerPrefs.GetFloat("highScore", 0f).ToString();
        myScoreText.text = GM.Score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
