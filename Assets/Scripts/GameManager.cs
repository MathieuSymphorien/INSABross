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
    private int playersInGame;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
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


        //spawn player depend on the list of player that joined the lobby room
        if(playersInGame == PhotonNetwork.PlayerList.Length)
        {
            SpawnPlayer();
        }
    }

    void SpawnPlayer()
    {
        //spawn player randomly in spawn point list position
        GameObject playerObject = PhotonNetwork.Instantiate(playerPrefabPath, spawnPoint[Random.Range(0, spawnPoint.Length)].position,Quaternion.identity);

        //instantiate
    }
}
