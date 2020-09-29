/*
* Unity C#, Firebase: Multiplayer Oyun Altyapısı Geliştirme Udemy Eğitimi
* Copyright (C) 2019 A.Gokhan SATMAN <abgsatman@gmail.com>
* This file is a part of TicTacToe project.
*/

using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase.Extensions;

public class DBManager : Singleton<DBManager>
{
    public AuthManager auth;

    public UserData user;
    public RoomData room;
    public BoardData board;

    public DatabaseReference usersDatabase;
    public DatabaseReference roomsDatabase;
    public DatabaseReference invitesDatabase;
    public DatabaseReference acceptedInvitesDatabase;

    public string FirebaseDBURL = "https://tiktakto-98b71.firebaseio.com/";

    void Start()
    {
        auth = AuthManager.Instance;

        user = UserData.Instance;
        room = RoomData.Instance;
        board = BoardData.Instance;

        StartCoroutine(Initialization());
    }

    private IEnumerator Initialization()
    {
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(FirebaseDBURL);

        var task = FirebaseApp.CheckAndFixDependenciesAsync();

        while (!task.IsCompleted)
        {
            yield return null;
        }

        if(task.IsFaulted || task.IsCanceled)
        {
            Debug.Log("IsFaulted || IsCanceled");
            yield break;
        }

        var dependencyStatus = task.Result;


        if (dependencyStatus == DependencyStatus.Available)
        {
            usersDatabase = FirebaseDatabase.DefaultInstance.GetReference("Users");
            roomsDatabase = FirebaseDatabase.DefaultInstance.GetReference("Rooms");
            invitesDatabase = FirebaseDatabase.DefaultInstance.GetReference("Invites");
            acceptedInvitesDatabase = FirebaseDatabase.DefaultInstance.GetReference("AcceptedInvites");

            if (auth.auth.CurrentUser != null)
            {
                auth.AutoLogin(auth.auth.CurrentUser.UserId);
            }
            else
            {
                SceneManager.LoadScene("Login");
            }
        }
        else
        {
            Debug.Log("Burası mı ?");
            Debug.LogError(String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
        }
            
        
    }

    public void CreateUser(string username, string country, string city)
    {
        Dictionary<string, object> general = new Dictionary<string, object>();
        general["Username"] = username;
        general["Country"] = country;
        general["City"] = city;

        Dictionary<string, object> progression = new Dictionary<string, object>();
        progression["Score"] = 0;
        progression["numberofgameswin"] = 0;
        progression["numberofgameslost"] = 0;
        progression["totalnumberofsos"] = 0;
        progression["level"] = 0;
        progression["totalnumberofgames"] = 0;
        progression["totalgoldcount"] = 0;

        progression["leaguelevel"] = "";

        usersDatabase.Child(user.userId).Child("General").UpdateChildrenAsync(general);
        user.username = username;
        user.country = country;
        user.city = city;

        usersDatabase.Child(user.userId).Child("Progression").UpdateChildrenAsync(progression);
        user.score = 0;
        user.numberofgameswin = 0;
        user.numberofgameslost = 0;
        user.totalnumberofsos = 0;
        user.level = 0;
        user.totalnumberofgames = 0;
        user.totalgoldcount = 0;
        //user.leaguelevel = "";

        Debug.Log("Kullanıcı başarıyla oluşturuldu, login sahnesine yönlendiriliyorsunuz...");

        SceneManager.LoadScene("Login");
    }

    public void GetUserInformation()
    {
        usersDatabase.Child(user.userId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if(task.IsFaulted)
            {
                Debug.Log("faulted");
                return;
            }
            if(task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                string username = snapshot.Child("General").Child("Username").Value.ToString();
                string city = snapshot.Child("General").Child("City").Value.ToString();
                string country = snapshot.Child("General").Child("Country").Value.ToString();
                string leaguelevel = snapshot.Child("Progression").Child("leaguelevel").Value.ToString();

                int score = int.Parse(snapshot.Child("Progression").Child("Score").Value.ToString());
                int level = int.Parse(snapshot.Child("Progression").Child("level").Value.ToString());
                int numberofgameslost = int.Parse(snapshot.Child("Progression").Child("numberofgameslost").Value.ToString());
                int numberofgameswin = int.Parse(snapshot.Child("Progression").Child("numberofgameswin").Value.ToString());
                int totalnumberofsos = int.Parse(snapshot.Child("Progression").Child("totalnumberofsos").Value.ToString());
                int totalgoldcount = int.Parse(snapshot.Child("Progression").Child("totalgoldcount").Value.ToString());
                int totalnumberofgames = int.Parse(snapshot.Child("Progression").Child("totalnumberofgames").Value.ToString());

                user.username = username;
                user.city = city;
                user.country = country;
                //user.leaguelevel = leaguelevel;

                user.level = level;
                user.numberofgameslost = numberofgameslost;
                user.numberofgameswin = numberofgameswin;
                user.totalnumberofsos = totalnumberofsos;
                user.totalgoldcount = totalgoldcount;
                user.totalnumberofgames = totalnumberofgames;
                user.score = score;

                Debug.Log("Kullanıcı login oldu ve bilgileri çekildi, mainmenu sahnesine yönlendiriliyorsunuz...");

                SceneManager.LoadScene("MainMenu");
            }
        });
    }

    public void CreateRoom(string roomName, string boardName, string time)
    {
        string roomId = roomsDatabase.Push().Key;
        room.roomId = roomId;

        Dictionary<string, object> boards = new Dictionary<string, object>();
        boards["s1"] = "";
        boards["s2"] = "";
        boards["s3"] = "";
        boards["s4"] = "";
        boards["s5"] = "";
        boards["s6"] = "";
        boards["s7"] = "";
        boards["s8"] = "";
        boards["s9"] = "";

        Dictionary<string, object> roomDetails = new Dictionary<string, object>();
        roomDetails["PlayerA"] = user.userId;
        roomDetails["PlayerB"] = "none";
        roomDetails["Result"] = "none";
        roomDetails["PlayerAReady"] = false;
        roomDetails["PlayerBReady"] = false;
        roomDetails["Board"] = boards;
        roomDetails["RoomName"] = roomName;
       // roomDetails["BoardList"] = roomName;
       // roomDetails["TimeList"] = roomName;

        roomsDatabase.Child(room.roomId).UpdateChildrenAsync(roomDetails);

        room.playerId = "PlayerA";

        OpenListenRoom();
        OpenListenInvites();

        Debug.Log("Oda Kuruldu... Diğer oyuncu bekleniyor... Transaction sahnesine yönlendiriliyorsunuz...");
        SceneManager.LoadScene("Transaction");
    }

    public void GetRoomList()
    {

        
        roomsDatabase.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("faulted");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                List<string> menuList = new List<string>();

                int index = 0;
                foreach (DataSnapshot r in snapshot.Children)
                {
                    string _roomId = r.Key;
                    string _roomName = snapshot.Child(_roomId).Child("RoomName").Value.ToString();
                    string _hostId = snapshot.Child(_roomId).Child("PlayerA").Value.ToString();
                    room.roomList.Add(new Room(_roomId, _hostId, _roomName));

                    menuList.Add(_roomName);
                    index++;
                }

                //roomList.Add(_roomName);
            }
        });
    }
    public void AddRoomToScrollView()
    {
        // veriler scroll'a yüklenecek
        // 1. content empty gameobject scrollview'in altında 
        // 2. 
        // veriler scroll'dan çekilecek
        GetRoomList();

    }
    public void SendInvite(string roomId)
    {
        Dictionary<string, object> invite = new Dictionary<string, object>();
        invite[user.userId] = user.userId;

        invitesDatabase.Child(roomId).UpdateChildrenAsync(invite);

        OpenListenAcceptedInvites();
    }

    public void AcceptInvite(string inviteUserId)
    {
        if(room.playerId =="PlayerA" && room.roomId != "")
        {
            Dictionary<string, object> roomDetails = new Dictionary<string, object>();
            roomDetails["PlayerB"] = inviteUserId;
            roomDetails["PlayerBReady"] = true;

            roomsDatabase.Child(room.roomId).UpdateChildrenAsync(roomDetails);

            room.OtherUserId = inviteUserId;

            Dictionary<string, object> acceptedInvite = new Dictionary<string, object>();
            acceptedInvite["RoomID"] = room.roomId;

            acceptedInvitesDatabase.Child(inviteUserId).UpdateChildrenAsync(acceptedInvite);

            GetOtherUserInformation();

            Debug.Log("Davet kabul edildi... Oyun sahnesine yönlendiriliyorsunuz...");
        }
    }

    public void SetResult(bool type = false)
    {
        Dictionary<string, object> result = new Dictionary<string, object>();
        if(type == false)
        {
            if (room.playerId == "PlayerA")
            {
                result["Result"] = "PlayerB";
            }
            if (room.playerId == "PlayerB")
            {
                result["Result"] = "PlayerA";
            }
        }
        else
        {
            if (room.playerId == "PlayerA")
            {
                result["Result"] = "PlayerA";
            }
            if (room.playerId == "PlayerB")
            {
                result["Result"] = "PlayerB";
            }
        }
        

        roomsDatabase.Child(room.roomId).UpdateChildrenAsync(result);
    }

    public void SetReady()
    {
        Dictionary<string, object> ready = new Dictionary<string, object>();
        if(room.playerId == "PlayerA")
        {
            ready["PlayerAReady"] = true;
        }
        if (room.playerId == "PlayerB")
        {
            ready["PlayerBReady"] = true;
        }

        roomsDatabase.Child(room.roomId).UpdateChildrenAsync(ready);
    }

    public void EditScore()
    {
        int score = user.score;
        score++;
        int newScore = score;
        Dictionary<string, object> scoreParam = new Dictionary<string, object>();
        scoreParam["Score"] = newScore;

        usersDatabase.Child(user.userId).Child("Progression").UpdateChildrenAsync(scoreParam).ContinueWith(task => {
            if(task.IsFaulted)
            {
                Debug.Log("faulted");
            }
            else if(task.IsCompleted)
            {
                user.score = newScore;
            }
        });
    }

    public void DoAction(string p)
    {
        Dictionary<string, object> action = new Dictionary<string, object>();
        if (room.playerId == "PlayerA")
        {
            action[p] = "X";
        }
        if (room.playerId == "PlayerB")
        {
            action[p] = "O";
        }

        roomsDatabase.Child(room.roomId).Child("Board").UpdateChildrenAsync(action);
    }

    public void GetOtherUserInformation()
    {
        Debug.Log("GetOtherUserInformation methodu çalıştı!");
        usersDatabase.Child(room.OtherUserId).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("faulted");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                room.OtherUsername = snapshot.Child("General").Child("Username").Value.ToString();
                string _otherScore = snapshot.Child("Progression").Child("Score").Value.ToString();
                room.OtherScore = int.Parse(_otherScore);

                Debug.Log("Rakip bilgileri: " + room.OtherUsername + "-" + room.OtherScore);
            }
        });
    }

    public void RemoveAllInvites()
    {
        if(room.playerId == "PlayerA")
        {
            invitesDatabase.Child(room.roomId).RemoveValueAsync();
        } 
    }

    public void RemoveAllAcceptedInvites()
    {
        if(room.playerId == "PlayerB")
        {
            acceptedInvitesDatabase.Child(user.userId).RemoveValueAsync();
        }
    }

    public void RemoveRoom()
    {
        roomsDatabase.Child(room.roomId).RemoveValueAsync();
    }

    public void OpenListenRoom()
    {
        roomsDatabase.Child(room.roomId).ValueChanged += ListenRoom;
    }

    public void ListenRoom(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        var snapshot = args.Snapshot;

        Debug.Log("Değişiklik algılandı #1");

        if (user.gameState == GameState.Gameplay)
            room.Result = snapshot.Child("Result").Value.ToString();

        if (room.playerId == "PlayerA")
            room.OtherUserId = snapshot.Child("PlayerB").Value.ToString();

        if (room.playerId == "PlayerB")
            room.OtherUserId = snapshot.Child("PlayerA").Value.ToString();

        if(user.gameState != GameState.Gameplay)
        {
            room.PlayerAReady = (bool)snapshot.Child("PlayerAReady").GetValue(true);
            room.PlayerBReady = (bool)snapshot.Child("PlayerBReady").GetValue(true);
        }

        //Board positions for 9 sections
        board.S1 = snapshot.Child("Board").Child("s1").Value.ToString();
        board.S2 = snapshot.Child("Board").Child("s2").Value.ToString();
        board.S3 = snapshot.Child("Board").Child("s3").Value.ToString();
        board.S4 = snapshot.Child("Board").Child("s4").Value.ToString();
        board.S5 = snapshot.Child("Board").Child("s5").Value.ToString();
        board.S6 = snapshot.Child("Board").Child("s6").Value.ToString();
        board.S7 = snapshot.Child("Board").Child("s7").Value.ToString();
        board.S8 = snapshot.Child("Board").Child("s8").Value.ToString();
        board.S9 = snapshot.Child("Board").Child("s9").Value.ToString();

        if(user.gameState == GameState.Gameplay)
            FindObjectOfType<Gameplay>().CheckTourConditions();
    }

    public void CloseListenRoom()
    {
        FirebaseDatabase.DefaultInstance.GetReference("Rooms").Child(room.roomId).ValueChanged -= ListenRoom;
    }

    public void OpenListenInvites()
    {
        FirebaseDatabase.DefaultInstance.GetReference("Invites").Child(room.roomId).ValueChanged += ListenInvites;
    }

    public void ListenInvites(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        Debug.Log("Değişiklik algılandı #2");

        foreach (DataSnapshot invite in args.Snapshot.Children)
        {
            string inviteUserId = invite.Key;

            if(user.gameState == GameState.Transaction)
            {
                GameObject invitedObject = Instantiate(FindObjectOfType<Transaction>().inviteObject, GameObject.Find("Canvas").transform);
                invitedObject.GetComponent<InviteManager>().otherUserId = inviteUserId;
            }
        }
    }

    public void CloseListenInvites()
    {
        FirebaseDatabase.DefaultInstance.GetReference("Invites").Child(room.roomId).ValueChanged -= ListenInvites;
    }

    public void OpenListenAcceptedInvites()
    {
        FirebaseDatabase.DefaultInstance.GetReference("AcceptedInvites").Child(user.userId).ValueChanged += ListenAcceptedInvites;
    }

    public void ListenAcceptedInvites(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        DataSnapshot snapshot = args.Snapshot;

        Debug.Log("Değişiklik algılandı #3");

        if (snapshot.HasChild("RoomID"))
        {
            string _roomId = snapshot.Child("RoomID").Value.ToString();
            room.roomId = _roomId;

            room.playerId = "PlayerB";

            if (room.roomId != "")
            {
                //CloseListenAcceptedInvites();
                Debug.Log("eşleşme sağlandı... transaction sahnesine yönlendiriliyorsunuz...");
                SceneManager.LoadScene("Transaction");
            }
        }
        else
        {
            Debug.Log("roomId yok");
        }
    }

    public void CloseListenAcceptedInvites()
    {
        FirebaseDatabase.DefaultInstance.GetReference("AcceptedInvites").Child(user.userId).ValueChanged -= ListenAcceptedInvites;
    }

    public void FinishGame()
    {
        //event kapama
        CloseListenAcceptedInvites();
        CloseListenInvites();
        CloseListenRoom();

        SceneManager.LoadScene("Result");
    }
}
