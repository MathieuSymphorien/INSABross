using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    [Header("Screens")]
    public GameObject mainScreen;
    public GameObject createRoomScreen;
    public GameObject lobbyScreen;
    public GameObject lobbyBrowserScreen;

    [Header("Main Screen")]
    public Button createRoomButton;
    public Button findRoomButton;

    [Header("Lobby")]
    public TextMeshProUGUI playerListText;
    public TextMeshProUGUI roomInfoText;
    public Button startGameButton;

    [Header("LobbyBrowser")]
    public RectTransform roomListContainer;
    public GameObject roomButtonPrefabs;

    private List<GameObject> roomButtons = new List<GameObject>();
    private List<RoomInfo> roomList = new List<RoomInfo>();
    private bool isConnectedToMaster = false;

    [Header("Music")]
    public AudioSource mainMusic;
    


    // Start is called before the first frame update
    void Start()
    {
        //disable the menus buttons at the start of the game
        createRoomButton.interactable = false;
        findRoomButton.interactable = false;

        mainMusic = GetComponent<AudioSource>();
        mainMusic.Play();

        //enable cursor
        Cursor.lockState = CursorLockMode.None;

        //if we are in the game or not ?
        if (PhotonNetwork.InRoom)
        {
            //Make the room visiable again
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.CurrentRoom.IsOpen = true; 
        }
    }

    //Swap the current screen
    public void SetScreen(GameObject screen)
    {
        //disable all screen first
        mainScreen.SetActive(false);
        lobbyBrowserScreen.SetActive(false);
        createRoomScreen.SetActive(false);
        lobbyScreen.SetActive(false);

        //active the request Screen
        screen.SetActive(true); 

        if(screen == lobbyBrowserScreen)
            UpdateLobbyBrowserUI();
    }

    public void OnPlayerNameChanged(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }

    public override void OnConnectedToMaster()
    {
        System.Console.WriteLine("En train de se connecter");
        isConnectedToMaster = true;
        Debug.Log("Connecté au serveur maître. menu");  // log
        createRoomButton.interactable = true;
        findRoomButton.interactable = true;
    }

    public void OnScreenRoomButton()
    {
        SetScreen(createRoomScreen);
    }

    public void OnFindRoomButton()
    {
        SetScreen(lobbyBrowserScreen);
    }

    public void OnBackToMainScreen()
    {
        SetScreen(mainScreen);
    }

    public void OnTrainingScreen()
    {
        SceneManager.LoadScene("Training");
    }

    public void OnCreateButton(TMP_InputField roomNameInput)
    {
        if (NetworkManager.instance == null)
        {
            Debug.LogError("NetworkManager.instance est null");
            return;
        }

        if (roomNameInput == null)
        {
            Debug.LogError("roomNameInput est null");
            return;
        }

        //ajouter la possibilité de rendre la taille de salle modudable (relié ca a un bouton)
        int maxPlayer = 20;

        NetworkManager.instance.CreateRoom(roomNameInput.text, maxPlayer);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Rejoint la salle avec succès.");  // log
        SetScreen(lobbyScreen);
        photonView.RPC("UpdateLobbyUi", RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobbyUi();
    }

    [PunRPC]
    void UpdateLobbyUi()
    {
        //enable start button just for the player  who created the room
        startGameButton.interactable = PhotonNetwork.IsMasterClient;

        //display all of the players
        playerListText.text = "";

        //loop throought all the players
        foreach (Player player in PhotonNetwork.PlayerList) 
        {
            playerListText.text += player.NickName + "\n";
        }

        //set the room info text
        roomInfoText.text = "<b> Room Name </b> \n" + PhotonNetwork.CurrentRoom.Name;
    }
    
    public void OnStartGameButton()
    {
        //Invisable the room which client master going to start it
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        //Tell everyone  to load to the game sccene
        Debug.Log(NetworkManager.instance);
        Debug.Log(NetworkManager.instance.photonView);

        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game");
    }

    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
    }

    GameObject CreateRoomButton()
    {
        GameObject buttonObject = Instantiate(roomButtonPrefabs, roomListContainer.transform);
        roomButtons.Add(buttonObject);
        return buttonObject;
    }

    void UpdateLobbyBrowserUI()
    {
        //disable alls rooms
        foreach(GameObject button in  roomButtons)
        {
            button.SetActive(false);
        }

        //Display all current rooms  in the master Client
        for (int x=0; x <roomList.Count; x++)
        {
            //get or create the button object
            GameObject button = x >= roomButtons.Count ? CreateRoomButton() : roomButtons[x];

            button.SetActive(true);
            //set the room name and player count text
            button.transform.Find("Room name text").GetComponent<TextMeshProUGUI>().text = roomList[x].Name;
            button.transform.Find("Player counter text").GetComponent<TextMeshProUGUI>().text = roomList[x].PlayerCount + " / " + roomList[x].MaxPlayers;

            //set the button when we click on them
            Button buttoncomp = button.GetComponent<Button>();
            string roomName =  roomList[x].Name;

            buttoncomp.onClick.RemoveAllListeners();
            buttoncomp.onClick.AddListener(()=>{ OnJoinRoomButton(roomName); });

        }
        
    }

    public void OnRefreshButton()
    {
        UpdateLobbyBrowserUI();
    }

    public void OnJoinRoomButton(string roomName)
    {
        if (isConnectedToMaster)
        {
            Debug.Log("avant l'appel de network manager ");
            NetworkManager.instance.JoinRoom(roomName);
            Debug.Log("Tentative de connexion à la salle : " + roomName);  // log
        }
        else
        {
            // Afficher un message à l'utilisateur indiquant qu'il n'est pas encore connecté
            Debug.Log("Bool isConnectedToMaster false log ");  // log
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> allRooms)
    {
        roomList = allRooms;
    }

}
