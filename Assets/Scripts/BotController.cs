using System.Collections;
using UnityEngine;
using Photon.Pun;

public class BotController : MonoBehaviourPunCallbacks
{
    public int maxHP = 100;
    private int currentHP;
    public Transform spawnPoints; // Assurez-vous d'assigner un parent contenant plusieurs points de spawn comme enfants dans l'éditeur Unity
    public SpriteRenderer sr;

    void Start()
    {
        PhotonNetwork.OfflineMode = true;
        currentHP = maxHP;
    }

    [PunRPC]
    public void TakeDamage(int damageAmount)
    {
        currentHP -= damageAmount;
        Debug.Log("Bot took damage: " + damageAmount + ". Current HP: " + currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(FlashDamage());
        }
    }

    void Die()
    {
        Debug.Log("Bot died. Respawning...");
        // Choisissez un point de spawn aléatoire pour le respawn
        int spawnIndex = Random.Range(0, spawnPoints.childCount);
        transform.position = spawnPoints.GetChild(spawnIndex).position;

        // Reset HP
        currentHP = maxHP;
    }

    IEnumerator FlashDamage()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
    }
}
