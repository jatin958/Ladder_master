using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public uiManager uiManager;
    [SerializeField] bool isgameStarted = false;
    [SerializeField] private bool isGameOver = false;
    private void Awake()
    {
        instance = this;
    }

    public void StartGame()
    {
        isgameStarted = true;
    }

    public bool isGameStart()
    {
        return isgameStarted;
    }

    public bool GetisGameOver()
    {
        return isGameOver;
    }

    public void setgameOver(bool value)
    {
        isGameOver = value;
    }

    public void restartGame()
    {
        SceneManager.LoadScene("Scenes/gamePlay");
    }
}
