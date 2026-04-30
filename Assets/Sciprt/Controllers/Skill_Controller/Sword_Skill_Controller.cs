using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Skill_Controller : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D cd;
    private Player player;
    [SerializeField] private float returnSpeed;

    private bool canRotate = true;
    private bool isReturning;

    [Header("Pierce info")]
    [SerializeField] private float pierceAmount;

    [Header("Bounce info")]
    [SerializeField] private float bounceSpeed;
    private bool isBouncing;
    private int BounceAmount;
    private List<Transform> enemyTarget;
    private int targetIndex;


    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>(); 
        cd = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (canRotate)
            transform.right = rb.velocity;

        if (isReturning)
        {

            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, returnSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.transform.position) < 0.5f)
                player.CatchTheSword();
        }

        BounceLogic();
    }

    private void BounceLogic()//弹射相关
    {
        if (isBouncing && enemyTarget.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[targetIndex].position, bounceSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, enemyTarget[targetIndex].position) < 0.1f)
            {
                targetIndex++;
                BounceAmount--;

                if (BounceAmount <= 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }

                if (targetIndex >= enemyTarget.Count)
                    targetIndex = 0;
            }
        }
    }

    public void SetupSword(Vector2 _dir,float _gravityScale,Player _player)
    {
        player = _player;

        rb.velocity = _dir;
        rb.gravityScale = _gravityScale;

        if (pierceAmount <= 0)
            anim.SetBool("Rotation", true);
    }

    public void SetupBounce(bool _isBouncing,int _amountOfBounces)
    {
        isBouncing= _isBouncing;
        BounceAmount= _amountOfBounces;
        enemyTarget = new List<Transform>();
    }

    public void SetupPierce(int _pierceAmount)
    {
        pierceAmount = _pierceAmount;
    }

    public void ReturnSword()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;//  约束 限制物理模拟中的自由度和运动  FreezeAll 限制所有物体的移动和旋转
        //rb.isKinematic = false;  冻结物理引擎引起的运动

        transform.parent = null;
        isReturning = true;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning)
            return;
        if (collision.GetComponent<Enemy>())
        {
            player.stats.DoDamage(collision.GetComponent<CharacterStats>());
        }
        

        if (collision.GetComponent<Enemy>() != null)
        {
            if (isBouncing && enemyTarget.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);

                foreach (var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                        enemyTarget.Add(hit.transform);
                }
            }
        }

        StuckInto(collision);
    }

    private void StuckInto(Collider2D collision)
    {
        if (pierceAmount > 0 && collision.GetComponent<Enemy>() != null)
        {
            pierceAmount --;
            return;
        }

        canRotate = false;
        cd.enabled = false;

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (isBouncing && enemyTarget.Count > 0)
        {
            cd.enabled = true;
            return;
        }

        anim.SetBool("Rotation", false);

        transform.parent = collision.transform;
    }
}
