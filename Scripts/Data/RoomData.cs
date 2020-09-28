﻿/*
* Unity C#, Firebase: Multiplayer Oyun Altyapısı Geliştirme Udemy Eğitimi
* Copyright (C) 2019 A.Gokhan SATMAN <abgsatman@gmail.com>
* This file is a part of TicTacToe project.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData : Singleton<RoomData>
{
    #region General
    public string roomId;
    public string playerId;
    #endregion

    #region OpponentInformation
    [SerializeField]
    private string _otherUserId;
    public string OtherUserId
    {
        get
        {
            return _otherUserId;
        }
        set
        {
            _otherUserId = value;
        }
    }

    [SerializeField]
    private string _othercountry;
    public string Othercountry
    {
        get
        {
            return _othercountry;
        }
        set
        {
            _othercountry = value;
        }
    }

    [SerializeField]
    private string _othercity;
    public string Othercity
    {
        get
        {
            return _othercity;
        }
        set
        {
            _othercity = value;
        }
    }

    [SerializeField]
    private string _otherleaguelevel;
    public string Otherleaguelevel
    {
        get
        {
            return _otherleaguelevel;
        }
        set
        {
            _otherleaguelevel = value;
        }
    }

    [SerializeField]
    private string _otherUsername;
    public string OtherUsername
    {
        get
        {
            return _otherUsername;
        }
        set
        {
            _otherUsername = value;
            if(UserData.Instance.gameState == GameState.Gameplay)
            {
                if (playerId == "PlayerA")
                {
                    FindObjectOfType<Gameplay>().playerBUsername.text = OtherUsername;
                }
                else if (playerId == "PlayerB")
                {
                    FindObjectOfType<Gameplay>().playerAUsername.text = OtherUsername;
                }
            }
        }
    }

    [SerializeField]
    private int _otherlevel;
    public int Otherlevel
    {
        get
        {
            return _otherlevel;
        }
        set
        {
            _otherlevel = value;
        }
    }

    [SerializeField]
    private int _othertotalnumberofsos;
    public int Othertotalnumberofsos
    {
        get
        {
            return _othertotalnumberofsos;
        }
        set
        {
            _othertotalnumberofsos = value;
        }
    }

    [SerializeField]
    private int _othernumberofgameslost;
    public int Othernumberofgameslost
    {
        get
        {
            return _othernumberofgameslost;
        }
        set
        {
            _othernumberofgameslost = value;
        }
    }

    private int _othernumberofgameswin;
    public int Othernumberofgameswin
    {
        get
        {
            return _othernumberofgameswin;
        }
        set
        {
            _othernumberofgameswin = value;
        }
    }

    private int _othertotalgoldcount;
    public int Othertotalgoldcount
    {
        get
        {
            return _othertotalgoldcount;
        }
        set
        {
            _othertotalgoldcount = value;
        }
    }

    private int _othertotalnumberofgames;
    public int Othertotalnumberofgames
    {
        get
        {
            return _othertotalnumberofgames;
        }
        set
        {
            _othertotalnumberofgames = value;
        }
    }

    [SerializeField]
    private int _otherScore;
    public int OtherScore
    {
        get
        {
            return _otherScore;
        }
        set
        {
            _otherScore = value;
            if (UserData.Instance.gameState == GameState.Gameplay)
            {
                if (playerId == "PlayerA")
                {
                    FindObjectOfType<Gameplay>().playerBScore.text = OtherScore.ToString();
                }
                else if (playerId == "PlayerB")
                {
                    FindObjectOfType<Gameplay>().playerAScore.text = OtherScore.ToString();
                }
            }
        }
    }
    #endregion

    #region Gameplay
    [SerializeField]
    private string _turn = "PlayerA";
    public string Turn
    {
        get
        {
            return _turn;
        }
        set
        {
            _turn = value;
            if (value == "PlayerA" && UserData.Instance.gameState == GameState.Gameplay)
            {
                FindObjectOfType<Gameplay>().playerATurn.enabled = true;
                FindObjectOfType<Gameplay>().playerBTurn.enabled = false;
            }
            else if (value == "PlayerB" && UserData.Instance.gameState == GameState.Gameplay)
            {
                FindObjectOfType<Gameplay>().playerATurn.enabled = false;
                FindObjectOfType<Gameplay>().playerBTurn.enabled = true;
            }
        }
    }
    #endregion

    #region Result
    [SerializeField]
    private string _result;
    public string Result
    {
        get
        {
            return _result;
        }
        set
        {
            if(_result == value)
            {
                return;
            }
            _result = value;
            if(value == "PlayerA" && UserData.Instance.gameState == GameState.Gameplay)
            {
                if (playerId == "PlayerA")
                {
                    Debug.Log("Kazandın!");
                }
                else if (playerId == "PlayerB")
                {
                    Debug.Log("Kaybettin!");
                }
                DBManager.Instance.FinishGame();
            }
            else if (value == "PlayerB" && UserData.Instance.gameState == GameState.Gameplay)
            {
                if (playerId == "PlayerA")
                {
                    Debug.Log("Kaybettin!");
                }
                else if (playerId == "PlayerB")
                {
                    Debug.Log("Kazandın!");
                }
                DBManager.Instance.FinishGame();
            }
        }
    }
    #endregion

    #region Others
    public List<Room> roomList = new List<Room>();

    [SerializeField]
    private bool _playerAReady;
    public bool PlayerAReady
    {
        get
        {
            return _playerAReady;
        }
        set
        {
            if(_playerAReady == value)
            {
                return;
            }
            _playerAReady = value;
            if (value == true && UserData.Instance.gameState == GameState.Transaction)
            {
                FindObjectOfType<Transaction>().playerAReadyText.text = "Player A Hazır!";
                FindObjectOfType<Transaction>().playerAReadyText.color = new Color32(65, 203, 41, 255);
            }
        }
    }

    [SerializeField]
    private bool _playerBReady;
    public bool PlayerBReady
    {
        get
        {
            return _playerBReady;
        }
        set
        {
            if(_playerBReady == value)
            {
                return;
            }
            _playerBReady = value;
            if (value == true && UserData.Instance.gameState == GameState.Transaction)
            {
                FindObjectOfType<Transaction>().playerBReadyText.text = "Player B Hazır!";
                FindObjectOfType<Transaction>().playerBReadyText.color = new Color32(65, 203, 41, 255);
            }
        }
    }
    #endregion
}
