using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoom : MonoBehaviour
{
    private DBManager DB;

    public Button createRoom;
    public InputField roomNameForm;
    public Dropdown boardList;
    public Dropdown timeList;

    private UserData user;
    private RoomData room;

    void Start()
    {
        DB = DBManager.Instance;
        user = UserData.Instance;

        user.gameState = GameState.CreateRoom;

        createRoom.onClick.AddListener(CreateRooms);
        // boardListe boards eklenecek 
        // boardListen boards çekilecek

        //boardList.AddOptions();
        //timeList.AddOptions();

    }

    void CreateRooms()
    {

        string roomName = roomNameForm.text;
        string boardList = "";
        string timeList = "";
        DB.CreateRoom(roomName,boardList,timeList);
    }
}
