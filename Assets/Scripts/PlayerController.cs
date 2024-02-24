using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun
{

    public Transform attackpoint;
    public int damage;
    public float attackRange;
    public float attackDelay;
    public float lastAttackTime;
   
    [HideInInspector]
    public int id;
    public Animator playerAnim;
    public Rigidbody2D rig;
    public Player photonPlayer;
    public SpriteRenderer sr;
    //public HeaderInfo headerInfo;
    public float moveSpeed;
    public int gold;
    public int currentHP;
    public int maxHP;
    public bool isDead;

    public static PlayerController me;
    public HeaderInformation headerInfo;


    public float jumpForce = 10f;
    public float moveAcceleration = 50f;
    private bool isJumping = false;
    public LayerMask groundLayer;
    public Transform groundCheck;
    private bool isGrounded;

    [PunRPC]
    public void Initialized(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;
        GameManager.instance.players[id - 1] = this;
        headerInfo.Initialized(player.NickName, maxHP);

        if (player.IsLocal)
            me = this;
        else
            rig.isKinematic = false;
    }

    void Update()
    {
        if (!photonView.IsMine)
            return;

        Move();

        // Check if the player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime > attackDelay)
            Attack();
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
        }
        else if (x < 0) // Moving left
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }


    private void Jump()
    {
        rig.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }

    /*
    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
            return;

        Move();
        if(Input.GetMouseButtonDown(0) && Time.time - lastAttackTime > attackDelay)
            Attack();
    }

    private void Move()
    {
        //get the horizontal and vertical input value
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        //apply  the value to our velocity
        rig.velocity = new Vector2(x, y) * moveSpeed;
    }*/

    void Attack()
    {
        //reset attack delay time
        lastAttackTime = Time.time;
        //send raycast in front of player
        RaycastHit2D hit = Physics2D.Raycast(attackpoint.position, transform.forward, attackRange);
        playerAnim.SetTrigger("Attack");
        if(hit.collider != null && hit.collider.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            //DAmage to enemy
        }
        playerAnim.SetTrigger("Attack");
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
        isDead = true;
        rig.isKinematic = true;
        transform.position = new Vector3(0, 90, 0);

        Vector3 spawnPos = GameManager.instance.spawnPoint[Random.Range(0, GameManager.instance.spawnPoint.Length)].position;
        StartCoroutine(Spawn(spawnPos, GameManager.instance.respawnTime));
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
