/*
* Unity C#, Firebase: Multiplayer Oyun Altyapısı Geliştirme Udemy Eğitimi
* Copyright (C) 2019 A.Gokhan SATMAN <abgsatman@gmail.com>
* This file is a part of TicTacToe project.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class Lobby : MonoBehaviour
{
    private DBManager DB;

    public GameObject roomPrefab;
    public GameObject parent;
    public Text text;


    public Button createRoomButton;
    public Button refreshLobbyButton;

    private UserData user;
    private RoomData room;
    public List<string> roomList;

    
    void Awake()
    {
        DB = DBManager.Instance;
        user = UserData.Instance;
        
        user.gameState = GameState.Lobby;
        //joinRoom.onClick.AddListener(JoinRoom);
        createRoomButton.onClick.AddListener(CreateRoom);
        refreshLobbyButton.onClick.AddListener(RefreshLobby);

        //DB.GetRoomList();
        //Debug.Log(menuList[1]);
        StartCoroutine(DB.GetRoomList((menuList) => {
            Debug.Log("Geldi: " + menuList.Count);

        }));

        //Instantiate(roomPrefab, parent.transform.position, Quaternion.identity, parent.transform);
    }

    //void JoinRoom()
    //{
    //    Debug.Log(gameListForm.options[gameListForm.value].text);
    //    string _roomId = gameListForm.options[gameListForm.value].text;
    //  DB.SendInvite(_roomId);
    //}

    void CreateRoom()
    {
        SceneManager.LoadScene("CreateRoom");
    }

    public void RefreshLobby()
    {
        Debug.Log("yenilendi");
        Instantiate(roomPrefab, parent.transform.position, Quaternion.identity, parent.transform);
    }
    
}
    
