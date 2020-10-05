using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button playButton;
    public Button profileButton;
    public Button marketButton;
    public Button exitButton;
    public Button leaderBoardButton;
    public Button settingsButton;

    void Awake()
    {
        playButton.onClick.AddListener(GoLobby);
        profileButton.onClick.AddListener(GoProfile);
        marketButton.onClick.AddListener(GoMarket);
        exitButton.onClick.AddListener(ExitGame);
        leaderBoardButton.onClick.AddListener(GoLeaderBoard);
        settingsButton.onClick.AddListener(GoSettings);
    }

    void GoSettings()
    {
        
    }

    void GoLeaderBoard()
    {
        
    }

    void ExitGame()
    {
        
    }

    void GoMarket()
    {
        
    }

    void GoProfile()
    {
        Debug.Log("Profile gidiyorsunuz...");
        SceneManager.LoadScene("Profile");
    }

    void GoLobby()
    {
        Debug.Log("Lobbye gidiyorsunuz...");
        SceneManager.LoadScene("Lobby");
    }
}
