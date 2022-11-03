using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    int damage = 0;

    string[] collideWithTags = { "MegaMan" };

    public void SetDamageValue(int damage)
    {
        this.damage = damage;
    }

    public void SetCollideWithTags(params string[] tags)
    {
        this.collideWithTags = tags;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (this.damage > 0)
        {
            foreach (string tag in collideWithTags)
            {
                // check for collision with this tag
                if (other.gameObject.CompareTag(tag))
                {
                    switch (tag)
                    {
                        case "Enemy":
                            // If enemies collide damage might be added
                            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
                            if (enemy != null)
                            {
                                enemy.TakeDamage(this.damage);
                            }
                            break;
                        case "MegaMan":
                            // If MM collides then damage may be added
                            MegaMan player = other.gameObject.GetComponent<MegaMan>();
                            if (player != null)
                            {
                                player.HitSide(transform.position.x > player.transform.position.x);
                                player.IsHit(this.damage);
                            }
                            break;
                    }
                }
            }
        }
    }

}