/*
* Unity C#, Firebase: Multiplayer Oyun Altyapısı Geliştirme Udemy Eğitimi
* Copyright (C) 2019 A.Gokhan SATMAN <abgsatman@gmail.com>
* This file is a part of TicTacToe project.
*/

public class Room
{
    public string roomId;
    public string hostId;
    public string roomName;
    
    public Room(string _roomId, string _hostId, string _roomName)
    {
        this.roomId = _roomId;
        this.hostId = _hostId;
        this.roomName = _roomName;
    }
}
