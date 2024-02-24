using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class PlayerController : MonoBehaviourPun
{

    public BoxCollider2D attackpoint;
    public int damage;
    public int baseDamage; 
    public int enhancedDamage; // La force d'attaque augmentée par le power-up
    public float powerUpDuration = 5f; //durée du power up
    //public float attackRange;
    public float attackDelay;
    public float lastAttackTime;
    public Transform attackPos;

    public int lifes;

    [HideInInspector]
    public int id;
    public Animator playerAnim;
    public Rigidbody2D rig;
    public Player photonPlayer;
    public SpriteRenderer sr;
    //public HeaderInfo headerInfo;
    public float moveSpeed;
    public int currentHP;
    public int maxHP;
    public bool isDead;
    

    public static PlayerController me;
    public HeaderInformation headerInfo;


    public float jumpForce = 10f;
    public float moveAcceleration = 50f;
    //private bool isJumping = false;
    public LayerMask groundLayer;
    public Transform groundCheck;
    private bool isGrounded;

    [PunRPC]
    public void Initialized(Player player)
    {
        Debug.Log("I m initialized");
        id = player.ActorNumber;
        photonPlayer = player;
        damage = baseDamage;
        GameManager.instance.players[id - 1] = this;
        headerInfo.Initialized(player.NickName, maxHP);

        if (player.IsLocal)
            me = this;
        else
            rig.isKinematic = true;


    }

    void Update()
    {
        PhotonNetwork.OfflineMode = true;
        if (!photonView.IsMine)
            return;

        Move();
        
        // Check if the player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.P) && Time.time - lastAttackTime > attackDelay)
            playerAnim.SetTrigger("Attack");
            //Attack();
    }

    private void Move()
    {
        float x = Input.GetAxis("Horizontal");

        // Apply force for movement
        rig.AddForce(new Vector2(x * moveAcceleration, 0));

        // Limit the speed of the character
        if (Mathf.Abs(rig.velocity.x) > moveSpeed)
        {
            rig.velocity = new Vector2(Mathf.Sign(rig.velocity.x) * moveSpeed, rig.velocity.y);
        }

        // If no input is given, stop the character's horizontal movement
        if (x == 0 && isGrounded)
        {
            rig.velocity = new Vector2(0, rig.velocity.y);
            
        }

        // Flip the sprite based on movement direction
        if (x > 0) // Moving right
        {
            GetComponent<SpriteRenderer>().flipX = false;
            playerAnim.SetBool("Move", true);
            attackPos.localPosition = new Vector3(4.82f, attackPos.localPosition.y, attackPos.localPosition.z);

        }
        else if (x < 0) // Moving left
        {
            GetComponent<SpriteRenderer>().flipX = true;
            playerAnim.SetBool("Move", true);
            attackPos.localPosition = new Vector3(-4.82f, attackPos.localPosition.y, attackPos.localPosition.z);

        }
        else
        {
            playerAnim.SetBool("Move", false);
        }
    }


    private void Jump()
    {
        rig.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }

    

 
    void Attack() { 

        //reset attack delay time
        lastAttackTime = Time.time;

       
        attackpoint.enabled = true;
        // Appeler une coroutine pour désactiver le collider après un court délai
        StartCoroutine(DisableColliderAfterDelay());

    }

    IEnumerator DisableColliderAfterDelay()
    {
        // Attendre un court délai
        yield return new WaitForSeconds(0.5f); // Ajustez cette durée selon la durée de l'animation d'attaque

        // Désactiver le BoxCollider2D
        attackpoint.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("nom " + other.name);
        if (other.CompareTag("Player"))
        {
            //TakeDamage(4);
            // Obtenez le PhotonView de l'ennemi
            PhotonView enemyPhotonView = other.GetComponent<PhotonView>();

            // Vérifiez si l'ennemi a un PhotonView et si vous êtes le joueur local
            if (enemyPhotonView != null && photonView.IsMine)
            {
                // Infligez des dégâts en utilisant RPC
      
                enemyPhotonView.RPC("TakeDamage", RpcTarget.All, damage);
            }
        }
        if (other.CompareTag("Bot")) 
        {
            // Obtenez le PhotonView du bot
            PhotonView botPhotonView = other.GetComponent<PhotonView>();

            if (botPhotonView != null)
            {
                // Infligez des dégâts au bot en utilisant RPC  
                botPhotonView.RPC("TakeDamage", RpcTarget.All, damage);
            }
        }
        if (other.CompareTag("PowerUpStrenght"))
        {
            Destroy(other.gameObject);
            StartCoroutine(TempIncreaseDamage());
        }
        if (other.CompareTag("PowerUpSpeed"))
        {
            
        }
        else if (other.CompareTag("punch"))
        {
            Debug.Log("item");
        }
    }

    IEnumerator TempIncreaseDamage()
    {
        damage = enhancedDamage; // Augmentez la force d'attaque

        // Attendre pendant la durée du power-up
        yield return new WaitForSeconds(powerUpDuration);

        damage = baseDamage; // Réinitialisez la force d'attaque à sa valeur de base
    }


    [PunRPC]
    public void TakeDamage(int damageAmount)
    {
        currentHP -= damageAmount;
        headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, currentHP);
        if(currentHP <= 0)
        {
            //Die
            Die();
        }
        else
        {
            photonView.RPC("FlashDamage", RpcTarget.All);
        }
    }

    [PunRPC]
    void FlashDamage()
    {
        StartCoroutine(DamageFlash());
        IEnumerator DamageFlash()
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            sr.color = Color.white;
        }
    }

    void Die()
    {
        lifes--;
        isDead = true;
        rig.isKinematic = true;
        transform.position = new Vector3(0, 90, 0);
        if (lifes <= 0)
        {
            // notifie GameManager pour notifier qu'un joueur est mort
            GameManager.instance.photonView.RPC("PlayerDied", RpcTarget.AllBuffered);
        }
        else
        {
            Vector3 spawnPos = GameManager.instance.spawnPoint[Random.Range(0, GameManager.instance.spawnPoint.Length)].position;
            StartCoroutine(Spawn(spawnPos, GameManager.instance.respawnTime));
        }
    }

    IEnumerator Spawn(Vector3 spawnPos, float timeToSpawn)
    {
        yield return new WaitForSeconds(timeToSpawn);
        isDead = false;
        transform.position = spawnPos;
        currentHP = maxHP;
        rig.isKinematic = false;

        //update health  UI
        
    }
}
