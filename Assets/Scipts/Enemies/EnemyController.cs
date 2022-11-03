using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]

public class EnemyController : MonoBehaviour
{
    Animator animator;
    BoxCollider2D box2d;
    Rigidbody2D rb2d;
    SpriteRenderer sprite;
    bool isInvincible;
    GameObject explodeEffect;

    RigidbodyConstraints2D rb2dConstraints;

    public bool freezeEnemy;

    public int scorePoints = 500;
    public int currentHealth;
    public int maxHealth = 1;
    public int contactDamage = 1;
    public int explosionDamage = 0;
    public int bulletDamage = 1;
    public float bulletSpeed = 3f;

    public AudioClip shootBulletClip;

    [SerializeField] AudioClip damageClip;
    [SerializeField] AudioClip blockAttackClip;

    [SerializeField] public GameObject ExplodeEffectPrefab;
    public GameObject bulletShootPos;
    public GameObject bulletPrefab;
    public float ExplodeEffectDestroyDelay = 2f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        box2d = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        currentHealth = maxHealth;
    }
    //Rotate the sprite
    public void Flip()
    {
        transform.Rotate(0, 180f, 0);
    }
    //Make enemy invinvible
    public void Invincible(bool invincibility)
    {
        isInvincible = invincibility;
    }
    //Have enemies lose health
    public void TakeDamage(int damage)
    {
        if (!isInvincible)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            SoundManager.Instance.Play(damageClip);
            if (currentHealth <= 0)
            {
                Defeat();
            }
        }
        else
        {
            SoundManager.Instance.Play(blockAttackClip);
        }
    }

    void startDefeatAnimation()
    {
        explodeEffect = Instantiate(ExplodeEffectPrefab);
        explodeEffect.name = ExplodeEffectPrefab.name;
        explodeEffect.transform.position = sprite.bounds.center;
        explodeEffect.GetComponent<ExplosionScript>().SetDamageValue(this.explosionDamage);
        Destroy(explodeEffect, 2f);
    }
    //Explode enemies
    void StopDefeatAnimation()
    {
        Destroy(explodeEffect);
    }
    //Destroy game objects when dead
    void Defeat()
    {
        startDefeatAnimation();
        Destroy(gameObject);
        GameManager.Instance.AddScorePoints(this.scorePoints);
    }

    public void FreezeEnemy(bool freeze)
    {
        if (freeze)
        {
            freezeEnemy = true;
            animator.speed = 0;
            rb2dConstraints = rb2d.constraints;
            rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
        {
            freezeEnemy = false;
            animator.speed = 1;
            rb2d.constraints = rb2dConstraints;
        }
    }
    //Colliding with MegaMan
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("MegaMan"))
        {
            MegaMan MegaMan = other.gameObject.GetComponent<MegaMan>();
            MegaMan.HitSide(transform.position.x > MegaMan.transform.position.x);
            MegaMan.IsHit(this.contactDamage);
        }
    }
}
