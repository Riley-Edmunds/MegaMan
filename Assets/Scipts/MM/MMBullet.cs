using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MMBullet : MonoBehaviour
{
    Animator animator;
    CircleCollider2D cc2d;
    Rigidbody2D rb2d;
    SpriteRenderer sprite;

    float destroyTime;

    bool freezeBullet;

    RigidbodyConstraints2D rb2dConstraints;

    public int damage = 1;

    [SerializeField] float bulletSpeed;
    [SerializeField] Vector2 bulletDirection;
    [SerializeField] float destroyDelay;

    [SerializeField] string[] collideWithTags = { "Enemy" };

    public enum BulletTypes { Default, MiniBlue, MiniGreen, MiniOrange, MiniPink, MiniRed };
    [SerializeField] BulletTypes bulletType = BulletTypes.Default;

    [System.Serializable]
    public struct BulletStruct
    {
        public Sprite sprite;
        public float radius;
        public Vector3 scale;
    }
    [SerializeField] BulletStruct[] bulletData;

    // Start is called before the first frame update
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        cc2d = GetComponent<CircleCollider2D>();

        SetBulletType(bulletType);
    }

    // Update is called once per frame
    void Update()
    {
        destroyTime -= Time.deltaTime;
        if (destroyTime < 0)
        {
            Destroy(gameObject);
        }
        if (freezeBullet) return;
    }

    public void SetBulletType(BulletTypes type)
    {
        // set sprite image & collider radius
        sprite.sprite = bulletData[(int)type].sprite;
        cc2d.radius = bulletData[(int)type].radius;
        transform.localScale = bulletData[(int)type].scale;
    }

    public void SetBulletSpeed(float speed)
    {
        // set bullet speed
        this.bulletSpeed = speed;
    }

    public void SetBulletDirection(Vector2 direction)
    {
        this.bulletDirection = direction;
    }

    public void SetDamageValue(int damage)
    {
        this.damage = damage;
    }

    public void SetDestroyDelay(float delay)
    {
        this.destroyDelay = delay;
    }

    public void SetCollideWithTags(params string[] tags)
    {
        // set game object tags bullet can collide with
        this.collideWithTags = tags;
    }

    //Stop bullets
    public void FreezeBullet(bool freeze)
    {
        if (freeze)
        {
            freezeBullet = true;
            rb2dConstraints = rb2d.constraints;
            animator.speed = 0;
            rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
            rb2d.velocity = Vector2.zero;
        }
        else
        {
            freezeBullet = false;
            animator.speed = 1;
            rb2d.constraints = rb2dConstraints;
            rb2d.velocity = bulletDirection * bulletSpeed;
        }
    }
    //Flip sprite based on direction
    public void Shoot()
    {
        sprite.flipX = (bulletDirection.x < 0);
        rb2d.velocity = bulletDirection * bulletSpeed;
        destroyTime = destroyDelay;
    }
    //Hurt enemies
    private void OnTriggerEnter2D(Collider2D other)
    {
        foreach (string tag in collideWithTags)
        {
            // check for collision with this tag
            if (other.gameObject.CompareTag(tag))
            {
                switch (tag)
                {
                    case "Enemy":
                        // enemy controller will apply the damage player bullet can cause
                        EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
                        if (enemy != null)
                        {
                            enemy.TakeDamage(this.damage);
                        }
                        break;
                    case "MegaMan":
                        // player controller will apply the damage enemy bullet can cause
                        MegaMan player = other.gameObject.GetComponent<MegaMan>();
                        if (player != null)
                        {
                            player.HitSide(transform.position.x > player.transform.position.x);
                            player.IsHit(this.damage);
                        }
                        break;
                }
                // remove the bullet - just not immediately
                Destroy(gameObject, 0.01f);
            }
        }
    }
}
