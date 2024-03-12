 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class GameManager : MonoBehaviourPun
{

    [Header("Players")]
    public string playerPrefabPath;
    public Transform[] spawnPoint;
    private Queue<Transform> availableSpawnPoints;
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
        Debug.Log("Start");
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
    public void PlayerDied()
    {
        playersInGame--;

        // V�rifie si le jeu doit se terminer
        if (playersInGame <= 1)
        {
            SceneManager.LoadScene("Menu");
            Debug.Log("Jeu termin�");
        }
    }

    [PunRPC]
    void ImInGame()
    {
        //add one to playerer in game variable for every player join the game
        playersInGame++;
        Debug.Log("in game");
        
        Debug.Log(PhotonNetwork.PlayerList.Length);
        Debug.Log(playersInGame);
        //spawn player depend on the list of player that joined the lobby room
        if (playersInGame == PhotonNetwork.PlayerList.Length)
        {
            Debug.Log("in if");
            SpawnPlayer();
        }
    }

    

    void SpawnPlayer()
    {
        Debug.Log("Spawn");
        if (playerPrefabPath == null || playerPrefabPath == "")
        {
            Debug.LogError("Le chemin du pr�fab du joueur n'est pas d�fini.");
            return;
        }

        if (availableSpawnPoints.Count == 0)
        {
            Debug.LogError("Pas assez de spawn points !");
            return;
        }

        Transform spawnLocation = availableSpawnPoints.Dequeue();
        GameObject playerObject = PhotonNetwork.Instantiate(playerPrefabPath, spawnLocation.position, Quaternion.identity);

        // Spawn player randomly in spawn point list position
        //GameObject playerObject = PhotonNetwork.Instantiate(playerPrefabPath, spawnPoint[Random.Range(0, spawnPoint.Length)].position, Quaternion.identity);

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

   

    void InitializeSpawnPoints()
    {
        availableSpawnPoints = new Queue<Transform>(spawnPoint.OrderBy(x => Random.value));
    }

    Transform GetRandomSpawnPoint()
    {
        List<Transform> freeSpawnPoints = new List<Transform>(spawnPoint);
        while (freeSpawnPoints.Count > 0)
        {
            Transform spawnPointToTry = freeSpawnPoints[Random.Range(0, freeSpawnPoints.Count)];
            if (IsSpawnPointFree(spawnPointToTry))
            {
                return spawnPointToTry;
            }
            else
            {
                freeSpawnPoints.Remove(spawnPointToTry);
            }
        }
        Debug.LogError("Aucun point de spawn libre !");
        return null; // ou retourner un point de spawn par d�faut
    }

    bool IsSpawnPointFree(Transform spawnPoint)
    {
        // Vous pouvez utiliser une v�rification de collision/overlap ici pour voir si le point est libre
        Collider[] hitColliders = Physics.OverlapSphere(spawnPoint.position, 0.5f);
        return hitColliders.Length == 0; // ou une condition appropri�e
    }

    /*public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length == playersInGame)
        {
            // Appelez votre m�thode de spawn ici
            SpawnPlayer();
        }
    }*/


}
