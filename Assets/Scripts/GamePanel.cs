using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{

    public Button returnBtn;
    public TextMeshProUGUI scoreText;

    private static GamePanel _instance;
    public static GamePanel Instance
    {
        get { return _instance; }
    }

    public int curScore;

    private void Awake()
    {
        _instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        returnBtn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
        });
    }

    public void UpdateScore(int delta)
    {
        curScore += delta;
        scoreText.text = curScore.ToString();
    }
}
