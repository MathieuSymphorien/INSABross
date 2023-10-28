using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public int maxPlayers;

    //singleton
    public static NetworkManager instance;


    private void Awake()
     {
         if(instance != null && instance != this)
         {
             gameObject.SetActive(false);
         }
         else
         {
             instance = this;
             DontDestroyOnLoad(gameObject);
         }
     }

     private void Start()
     {
        System.Console.WriteLine("En train de se connecter");
        //Connecting to the master server at th begining of the game
        PhotonNetwork.ConnectUsingSettings();
     }

     public override void OnConnectedToMaster()
     {
         PhotonNetwork.JoinLobby();
     }

     public void CreateRoom(string roomName)
     {
         //creating different room
         RoomOptions options = new RoomOptions();
         //max player
         options.MaxPlayers = (byte)maxPlayers;
         //Create room
         PhotonNetwork.CreateRoom(roomName, options);
     }

      public void JoinRoom(string roomName)
      {
          PhotonNetwork.JoinRoom(roomName);
      }

      [PunRPC]
      public void ChangeScene(string sceneName)
      {
          PhotonNetwork.LoadLevel(sceneName);
      }
}
