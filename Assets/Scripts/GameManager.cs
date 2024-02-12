 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class GameManager : MonoBehaviourPun
{

    [Header("Players")]
    public string playerPrefabPath;
    public Transform[] spawnPoint;
    public float respawnTime;
    public int playersInGame;
    public PlayerController[] players;

    public static GameManager instance;

    [Header("Music Stage")]
    public AudioSource stageMusic;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
        players = new PlayerController[PhotonNetwork.CurrentRoom.MaxPlayers];

        stageMusic = GetComponent<AudioSource>();
        stageMusic.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    void ImInGame()
    {
        //add one to playerer in game variable for every player join the game
        playersInGame++;
        Debug.Log("in game");
        
        Debug.Log(PhotonNetwork.PlayerList.Length);

        //spawn player depend on the list of player that joined the lobby room
        if (playersInGame == PhotonNetwork.PlayerList.Length)
        {
            Debug.Log("in if");
            SpawnPlayer2();
        }
    }

    

    void SpawnPlayer2()
    {
        Debug.Log("Spawn");
        if (playerPrefabPath == null || playerPrefabPath == "")
        {
            Debug.LogError("Le chemin du pr�fab du joueur n'est pas d�fini.");
            return;
        }

        // Spawn player randomly in spawn point list position
        GameObject playerObject = PhotonNetwork.Instantiate(playerPrefabPath, spawnPoint[Random.Range(0, spawnPoint.Length)].position, Quaternion.identity);

        // Initialize the player
        PlayerController playerController = playerObject.GetComponent<PlayerController>();

        if (playerController != null)
        {
            // Vous pouvez ajouter un appel � une fonction RPC ici pour initialiser le joueur,
            // ou faire d'autres configurations sp�cifiques n�cessaires.
            playerController.photonView.RPC("Initialized", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer);
        }
        else
        {
            Debug.LogError("Le pr�fab instanci� ne contient pas de composant PlayerController.");
        }
    }

    void SpawnPlayer()
    {
        if (playerPrefabPath == null || playerPrefabPath == "")
        {
            Debug.LogError("Le chemin du pr�fab du joueur n'est pas d�fini.");
            return;
        }
        //spawn player randomly in spawn point list position
        GameObject playerObject = PhotonNetwork.Instantiate(playerPrefabPath, spawnPoint[Random.Range(0, spawnPoint.Length)].position, Quaternion.identity);

        //instantiate
    }

}
