﻿/*
* Unity C#, Firebase: Multiplayer Oyun Altyapısı Geliştirme Udemy Eğitimi
* Copyright (C) 2019 A.Gokhan SATMAN <abgsatman@gmail.com>
* This file is a part of TicTacToe project.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Transaction : MonoBehaviour
{
    private DBManager DB;

    private UserData user;
    private RoomData room;

    public Text playerAReadyText;
    public Text playerBReadyText;

    public Text noticeText;

    public GameObject inviteObject;

    private void Awake()
    {
        DB = DBManager.Instance;

        user = UserData.Instance;
        room = RoomData.Instance;

        user.gameState = GameState.Transaction;
        DB.SetReady();

        if(room.playerId == "PlayerB")
        {
            DB.OpenListenRoom();
            DB.CloseListenAcceptedInvites();

            playerAReadyText.enabled = false;
            playerBReadyText.enabled = false;
        }

        StartCoroutine(CheckReadyStatus());
    }

    IEnumerator CheckReadyStatus()
    {
        yield return new WaitUntil(() => room.PlayerAReady && room.PlayerBReady);

        noticeText.text = "Oyun yükleniyor..... Lütfen bekleyin...";
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Gameplay");
    }
}
