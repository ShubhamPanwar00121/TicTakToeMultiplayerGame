using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class firebaseManager : MonoBehaviour
{
    #region referances
    DatabaseReference databaseRef;
    [HideInInspector] public string roomId = "R";
    public static firebaseManager instance { get; private set; }
    public int playerNumber = 1;
    #endregion

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    void Start()
    {
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public void MakeRoom()
    {
        UiManager.UIinstance.RoomCreateClicked();

        int RoomCount;
        databaseRef.Child("databaseData").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                 UiManager.UIinstance.SetLogMessage("task is Faulted: create room");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                try
                {
                    RoomCount = int.Parse(snapshot.Child("matchCount").Value.ToString());
                    RoomCount++;
                    databaseRef.Child("databaseData").Child("matchCount").SetValueAsync(RoomCount);
                }
                catch
                {
                    Debug.Log("exception in getting value of match count");
                    RoomCount = 1;
                    databaseRef.Child("databaseData").Child("matchCount").SetValueAsync(RoomCount);
                }
                roomId = roomId + RoomCount;
                databaseRef.Child("matchRooms").Child(roomId).Child("P1").SetValueAsync("0");
                databaseRef.Child("matchRooms").Child(roomId).Child("P2").SetValueAsync("0");
                databaseRef.Child("matchRooms").Child(roomId).Child("P1 Massage").SetValueAsync("no message yet");
                databaseRef.Child("matchRooms").Child(roomId).Child("P2 Massage").SetValueAsync("no message yet");
                databaseRef.Child("matchRooms").Child(roomId).Child("P2").ValueChanged += ListenToPlayer2;
                databaseRef.Child("matchRooms").Child(roomId).Child("P2 Massage").ValueChanged += GetP2Messages;

                UiManager.UIinstance.RoomCreated();
            }
        });
    }
    private void ListenToPlayer2(object sender, ValueChangedEventArgs e)
    {
        if(string.Equals(GameManager.Instance.GetCurrentSceneName(),"StartMenu") && e.Snapshot.Value.ToString() == "1" )
        {
            GameManager.Instance.LoadGameScene();
        }
        
        else if(string.Equals(GameManager.Instance.GetCurrentSceneName(), "GameScene"))
        {
            TilePojo pojo = JsonUtility.FromJson<TilePojo>(e.Snapshot.Value.ToString());
            UiManager.UIinstance.OnBtnPress(GameManager.Instance.tiles[pojo.i, pojo.j].gameObject, true);
        }
        
    }
    public void JoinRoom(string roomId)
    {
        this.roomId = roomId;
        databaseRef.Child("matchRooms").Child(roomId).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if(task.IsFaulted)
            {
                UiManager.UIinstance.SetLogMessage("task is faulted: join room");
            }
            else if(task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                try
                {
                    int checkForRoom = int.Parse(snapshot.Child("P1").Value.ToString());
                    playerNumber = 2;

                    databaseRef.Child("matchRooms").Child(roomId).Child("P2").SetValueAsync("1");
                    databaseRef.Child("matchRooms").Child(roomId).Child("P1").ValueChanged += ListenToPlayer1;
                    databaseRef.Child("matchRooms").Child(roomId).Child("P1 Massage").ValueChanged += GetP1Messages;
                    GameManager.Instance.LoadGameScene();
                }
                catch
                {
                    UiManager.UIinstance.SetLogMessage("Room Does Not Exist, Reopen App And Try Again");
                }
            }
        });
    }
    private void ListenToPlayer1(object sender, ValueChangedEventArgs e)
    {
        TilePojo pojo = JsonUtility.FromJson<TilePojo>(e.Snapshot.Value.ToString());
        UiManager.UIinstance.OnBtnPress(GameManager.Instance.tiles[pojo.i, pojo.j].gameObject, true);
    }
    public void SetP1Values(string indexOfButtonPressed)
    {
        databaseRef.Child("matchRooms").Child(roomId).Child("P1").SetValueAsync(indexOfButtonPressed);
    }
    public void SetP2Values(string indexOfButtonPressed)
    {
        databaseRef.Child("matchRooms").Child(roomId).Child("P2").SetValueAsync(indexOfButtonPressed);
    }
    private void GetP1Messages(object sender, ValueChangedEventArgs e)
    {
        UiManager.UIinstance.OnMessageGet(e.Snapshot.Value.ToString());
    }
    private void GetP2Messages(object sender, ValueChangedEventArgs e)
    {
        UiManager.UIinstance.OnMessageGet(e.Snapshot.Value.ToString());
    }
    public void SendMessageGame(string message)
    {
        if (playerNumber == 1) databaseRef.Child("matchRooms").Child(roomId).Child("P1 Massage").SetValueAsync(message);
        else if (playerNumber == 2) databaseRef.Child("matchRooms").Child(roomId).Child("P2 Massage").SetValueAsync(message);
    }
}
