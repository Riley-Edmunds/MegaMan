using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class MegaMan : MonoBehaviour
{

    public static MegaMan Instance { get; private set; }

    public int maxHealth = 28;
    public int currentHealth = 28;
    public int lives = 2;
    public int bulletDamage = 1;
    public float bulletSpeed = 5f;
    public float speed;
    public float inputHorizontal;
    public float inputVertical;
    public bool shoot;
    public bool keyShootRelease;
    public bool isShooting;
    public bool isHurt;
    public bool isDead;
    public bool isLadder;
    public bool isClimbing;
    public bool isOnSpike;
    public bool isJumping;
    public bool isInvincible;
    public bool m_FacingRight;
    public bool hitSideRight;
    public bool isTakingDamage;
    public bool freezePlayer;
    public bool freezeInput;
    public bool freeze;
    public string currentLevel;
    public Animator animator;
    public Transform SpawnPoint;
    public Transform bulletShootPos;
    public GameObject bulletPrefab;
    public float shootTime;
    
    public GameObject StartBossGameObject;
    Tilemap StartBoss;
    RigidbodyConstraints2D constraints;

    [SerializeField] private BoxCollider2D bc;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask platformlm;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private AudioClip landingSound;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip explosionSound;
    void Awake()
    {
        bc = gameObject.GetComponent<BoxCollider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        //Ladders
        inputVertical = Input.GetAxis("Vertical");
        if (isLadder && Mathf.Abs(inputVertical) > 0f)
        {
            isClimbing = true;
        }

        //Jumping
        if (IsGrounded() && Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = true;
            animator.SetBool("isJumping", isJumping);
            float jumpVelocity = 14f;
            rb.velocity = Vector2.up * jumpVelocity;
            SoundManager.Instance.Play(landingSound);
            if (shoot== true)
            {
                isShooting = true;
                animator.SetBool("isJumpShooting", isShooting);
            }
            else
            {
                isShooting = false;
                animator.SetBool("isJumpShooting", isShooting);
            }
        }
        else
        {
            isJumping = false;
            animator.SetBool("isJumping", isJumping);
        }

        //hit
        if (isTakingDamage)
        {
            animator.Play("Hurt");
            return;
        }

        //Respawning
        if (currentHealth == 0)
        {
            Invoke("startDefeatAnimation", .1f);
        }

        debuging();
    }
    //Debuging/extras
    void debuging()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            startDefeatAnimation();
            Debug.Log("startDefeatAnimation()");
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            Invincible(!isInvincible);
            Debug.Log("Invincible"+ isInvincible);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            FreezePlayer(!freezePlayer);
            Debug.Log("Pause player: " + freezePlayer);
        }
    }


    void FixedUpdate()
    {
        //A/D Movement
        inputHorizontal = Input.GetAxis("Horizontal");
        //Shooting
        shoot = Input.GetKey(KeyCode.K);
        PlayerShootInput();
        if (isShooting == true && inputHorizontal != 0)
        {
            isShooting = true;
            animator.SetBool("isRunShooting", isShooting);
        }
        else
        {
            isShooting = false;
            animator.SetBool("isRunShooting", isShooting);
        }

        if (shoot == true && inputHorizontal ==0 && inputVertical ==0)
        {
            isShooting = true;
            animator.SetBool("isShooting", shoot);
        }
        else
        {
            isShooting = false;
            animator.SetBool("isShooting", shoot);
        }

        //Flipping sprite
        if (inputHorizontal > 0)
        {
            m_FacingRight = false;
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            m_FacingRight = true;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        //Horizontal speed
        animator.SetFloat("inputHorizontal", Mathf.Abs(inputHorizontal));
        rb.velocity = new Vector2(inputHorizontal * speed, rb.velocity.y);
        //Ladders
        if (isClimbing == true)
        {
            animator.SetBool("isClimbing", isClimbing);
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(rb.velocity.x, inputVertical * speed);
            if (shoot)
            {
                isShooting = true;
                animator.Play("Shoot-Climb");
            }
            else
            {
                isShooting = false;
                animator.SetBool("isClimbShooting", shoot);
            }
        }
        else
        {
            isClimbing = false;
            animator.SetBool("isClimbing", isClimbing);
            rb.gravityScale = 3f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Detecting ladder collision
        if (collision.CompareTag("Ladder"))
        {
            isLadder = true;
        }
        //Spike collision (intsant death)
        if (collision.CompareTag("Spike"))
        {
            isOnSpike = true;
            Respawn();
        }
        Vector3 hitPosition = Vector3.zero;
        //When to start boss fight
        if (collision.CompareTag("StartBoss"))
        {
            //possible change to destroy startboss
            StartBoss.SetTile(StartBoss.WorldToCell(hitPosition), null);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Detects when ladders are exited
        if (collision.CompareTag("Ladder"))
        {
            isLadder = false;
            isClimbing = false;
        }
        if (collision.CompareTag("Spike"))
        {
            isOnSpike = false;
        }
    }
    //Grounded means able to jump
    private bool IsGrounded()
    {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0f, Vector2.down, .1f, platformlm);
        return raycastHit2d.collider != null;
    }
    //Player shooting functionality
    void PlayerShootInput()
    {
        float shootTimeLength = 0;
        float keyShootReleaseTimeLength = 0;

        if(shoot & keyShootRelease)
        {
            SoundManager.Instance.Play(shootSound);
            isShooting = true;
            keyShootRelease = false;
            shootTime = Time.time;
            //Bullet
            ShootBullet();
        }
        if (!shoot && !keyShootRelease)
        {
            keyShootReleaseTimeLength = Time.time - shootTime;
            keyShootRelease = true;
        }
        if (isShooting)
        {
            shootTimeLength = Time.time - shootTime;
            if (shootTimeLength >= .25f || keyShootReleaseTimeLength >= .15f)
            {
                isShooting = false;
            }
        }
    }
    //Getting the bullet that needs to be used and setting variables
    void ShootBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletShootPos.position, Quaternion.identity);
        bullet.GetComponent<MMBullet>().SetDamageValue(bulletDamage);
        bullet.GetComponent<MMBullet>().SetBulletSpeed(bulletSpeed);
        bullet.GetComponent<MMBullet>().SetBulletDirection((m_FacingRight) ? Vector2.left : Vector2.right);
        bullet.GetComponent<MMBullet>().Shoot();
    }
    //Stopping player actions
    public void FreezePlayer(bool freeze)
    {
        if (freeze)
        {
            freezePlayer = true;
            constraints = rb.constraints;
            animator.speed = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
        {
            freezePlayer = false;
            animator.speed = 1;
            rb.constraints = constraints;
        }
    }
    //Stops player input
    public void FreezeInput(bool freeze)
    {
        freezeInput = freeze;
    }
    //Detects the hit side of the characcter for animation
    public void HitSide(bool rightSide)
    {
        // determines the push direction of the hit animation
        hitSideRight = rightSide;
    }
    //Handels when the character has taken damage
    public void IsHit(int damage)
    {
        //Make player invincible
        if (!isInvincible)
        {
            SoundManager.Instance.Play(damageSound);
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            //UI current/max float conversion
            UIHealthBar.Instance.SetValue(currentHealth / (float)maxHealth);
            if (currentHealth <= 0) { Invoke("startDefeatAnimation", .3f); }
            else
            { StartDamageAnimation(); }
        }
    }
    //Controls damage animations
    void StartDamageAnimation()
    {
        if (!isTakingDamage)
        {
            Invincible(!isInvincible);
            isTakingDamage = true;
            //Invincible(true);
            FreezeInput(true);
            float hitForceX = 2.50f;
            float hitForceY = 5.5f;
            if (!hitSideRight) hitForceX = -hitForceX;
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(hitForceX, hitForceY), ForceMode2D.Impulse);
        }
    }
    //Removes damage effects
    void StopDamageAnimation()
    {
        freezePlayer = false;
        Invincible(false);
        FreezeInput(false);
        isTakingDamage = false;
        animator.SetBool("isTakingDamage", isTakingDamage);
    }
    //Makes player invincible
    public void Invincible(bool invincibility)
    {
        isInvincible = invincibility;
    }
    //Plays death animation and respawns player
    void startDefeatAnimation()
    {
        SoundManager.Instance.Play(explosionSound);
        GameObject explodeEffect = Instantiate(explosionPrefab);
        explodeEffect.name = explosionPrefab.name;
        explodeEffect.transform.position = sr.bounds.center;
        Destroy(gameObject);
        Invoke("Respawn", 2.5f);
    }
    /*
    //Removes player lives
    private void IsDead()
    {
        lives = lives - 1;
        if (lives == 0) { LostAllLives(); }
        else { Respawn(); }
    }*/
    //Respawns the player at the begining of the level and removes a life
    private void Respawn()
    {
        lives = lives - 1;
        if (lives == 0) 
            { LostAllLives(); }
        else
        {
            //UnityEngine.SceneManagement.SceneManager.LoadScene(6);
            currentHealth = maxHealth;
            this.transform.position = SpawnPoint.position;
        }
    }
    //Sends player to level select
    private void LostAllLives()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(4);
    }
}
