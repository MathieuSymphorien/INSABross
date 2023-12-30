using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    //public int maxPlayers;

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
        Debug.Log("Connecté au serveur maître.");
        PhotonNetwork.JoinLobby();
     }

     public void CreateRoom(string roomName, int maxPlayers)
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
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinRoom(roomName);
        }
        else
        {
            Debug.LogError("Client non prêt pour les opérations. Etat actuel: " + PhotonNetwork.NetworkClientState);
            // Vous pouvez choisir de reconnecter ici ou d'alerter l'utilisateur
        }
    }

      [PunRPC]
      public void ChangeScene(string sceneName)
      {
        // Trouver tous les PhotonView dans la scène
        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();

        // Réinitialiser l'ID de chaque PhotonView (sinon la transition se fait mal entre les scènes et des id peuvent etre pareil se qui donne une erreur). Ici le systeme re attribue automatiquement les id
        foreach (PhotonView view in photonViews)
        {
            view.ViewID = 0;
        }
        PhotonNetwork.LoadLevel(sceneName);
      }
}
