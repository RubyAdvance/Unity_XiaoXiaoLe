using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    public Button startBtn;
    private void Awake()
    {
        startBtn.onClick.AddListener(() =>
    {
        // 登录按钮被点击，进入Game场景
        SceneManager.LoadScene(1);
    });
    }

}
