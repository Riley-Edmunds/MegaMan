using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beak : MonoBehaviour
{
    Animator animator;
    BoxCollider2D box2d;
    Rigidbody2D rb2d;
    EnemyController enemyController;

    int bulletIndex = 0;

    // color determines the bullet type
    MMBullet.BulletTypes bulletType;

    // how long Beak will stay closed 
    float closedTimer;
    public float closedDelay = 2f;

    // perform attack if MegaMan is within range
    bool doAttack;
    public float MegaManRange = 2f;

    public enum BeakColors { Blue, Orange, Red };
    [SerializeField] BeakColors BeakColor = BeakColors.Orange;

    public enum BeakStates { Closed, Open };
    [SerializeField] BeakStates BeakState = BeakStates.Closed;

    public enum BeakOrientations { Bottom, Top, Left, Right };
    [SerializeField] BeakOrientations BeakOrientation = BeakOrientations.Left;

    [SerializeField] RuntimeAnimatorController racBeakBlue;
    [SerializeField] RuntimeAnimatorController racBeakOrange;
    [SerializeField] RuntimeAnimatorController racBeakRed;

    void Awake()
    {
        // get components from EnemyController
        enemyController = GetComponent<EnemyController>();
        animator = enemyController.GetComponent<Animator>();
        box2d = enemyController.GetComponent<BoxCollider2D>();
        rb2d = enemyController.GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // set color and orientation
        SetColor(BeakColor);
        SetOrientation();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyController.freezeEnemy)
        {
            return;
        }

        // get MegaMan object 
        GameObject MegaMan = GameObject.FindGameObjectWithTag("MegaMan");

        // Beak has only two states - closed and open
        switch (BeakState)
        {
            case BeakStates.Closed:
                animator.Play("Beak_Closed");
                // check distance to MegaMan
                if (MegaMan != null && !doAttack)
                {
                    float distance = Vector2.Distance(transform.position, MegaMan.transform.position);
                    if (distance <= MegaManRange)
                    {
                        doAttack = true;
                        closedTimer = closedDelay;
                    }
                }
                // within distance and can attackracBeak
                if (doAttack)
                {
                    // delay before opening to attack
                    closedTimer -= Time.deltaTime;
                    if (closedTimer <= 0)
                    {
                        // switch to open state
                        BeakState = BeakStates.Open;
                    }
                }
                break;
            case BeakStates.Open:
                animator.Play("Beak_Open");
                break;
        }
    }

    public void SetColor(BeakColors color)
    {
        BeakColor = color;
        SetBulletType();
        SetAnimatorController();
    }

    void SetAnimatorController()
    {
        // set animator controller from color
        switch (BeakColor)
        {
            case BeakColors.Blue:
                animator.runtimeAnimatorController = racBeakBlue;
                break;
            case BeakColors.Orange:
                animator.runtimeAnimatorController = racBeakOrange;
                break;
            case BeakColors.Red:
                animator.runtimeAnimatorController = racBeakRed;
                break;
        }
    }

    void SetBulletType()
    {
        // set bullet type/color
        switch (BeakColor)
        {
            case BeakColors.Blue:
                bulletType = MMBullet.BulletTypes.MiniBlue;
                break;
            case BeakColors.Orange:
                bulletType = MMBullet.BulletTypes.MiniOrange;
                break;
            case BeakColors.Red:
                bulletType = MMBullet.BulletTypes.MiniRed;
                break;
        }
    }

    void SetOrientation()
    {
        // reset rotation
        transform.rotation = Quaternion.identity;
        // rotate orientation
        switch (BeakOrientation)
        {
            case BeakOrientations.Bottom:
                transform.Rotate(0, 0, 90f);
                break;
            case BeakOrientations.Top:
                transform.Rotate(0, 0, -90f);
                break;
            case BeakOrientations.Left:
                transform.Rotate(0, 0, 0);
                break;
            case BeakOrientations.Right:
                transform.Rotate(0, 180f, 0);
                break;
        }
    }

    private void ShootBullet()
    {
        GameObject bullet;
        Vector2[] bulletVectors = {
            new Vector2(.75f, .75f),
            new Vector2(1f, .15f),
            new Vector2(1f, -0.15f),
            new Vector2(0.75f, -0.75f)
        };
        //bullet orientation
        switch (BeakOrientation)
        {
            case BeakOrientations.Left:
                //postive x-axis
                break;
            case BeakOrientations.Right:
                bulletVectors[bulletIndex].x *= -1;
                break;
            case BeakOrientations.Bottom:
                //fires up 
                bulletVectors[bulletIndex] = Functions.RotateByAngle(bulletVectors[bulletIndex], 90f);
                break;
            case BeakOrientations.Top:
                //fires down
                bulletVectors[bulletIndex] = Functions.RotateByAngle(bulletVectors[bulletIndex], -90f);
                break;
        }
        //instantiate bullet prefab, set type, damage, speed, direction
        bullet = Instantiate(enemyController.bulletPrefab);
        bullet.name = enemyController.bulletPrefab.name;
        bullet.transform.position = enemyController.bulletShootPos.transform.position;
        bullet.GetComponent<MMBullet>().SetBulletType(bulletType);
        bullet.GetComponent<MMBullet>().SetDamageValue(enemyController.bulletDamage);
        bullet.GetComponent<MMBullet>().SetBulletSpeed(enemyController.bulletSpeed);
        bullet.GetComponent<MMBullet>().SetBulletDirection(bulletVectors[bulletIndex]);
        bullet.GetComponent<MMBullet>().SetCollideWithTags("MegaMan");
        bullet.GetComponent<MMBullet>().SetDestroyDelay(5f);
        bullet.GetComponent<MMBullet>().Shoot();
        //increment/reset bulletIndex
        if (++bulletIndex > bulletVectors.Length - 1)
        {
            //reset bulletIndex
            bulletIndex = 0;
        }
        //play only one bullet sound
        SoundManager.Instance.Play(enemyController.shootBulletClip);
    }

    //called from the animation event in Beak_Closed animation
    private void InvincibleAimationStart()
    {
        enemyController.Invincible(true);
    }

    //called from the first animation event in the Beak_Open animation
    private void OpenAnimationStart()
    {
        enemyController.Invincible(false);
    }

    //called from the last animation event in the Beak_Open animation
    private void OpenAnimationStop()
    {
        doAttack = false;
        BeakState = BeakStates.Closed;
    }
}
