using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal_Skill_Controller : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();
    private CircleCollider2D cd=>GetComponent<CircleCollider2D>();

    private float crystalExistTimer;

    private bool canExplode;
    //private bool canMove;
    //private float moveSpeed;

    private bool canGrow;

    private Transform closestTarget;
    [SerializeField] private float growSpeed;

    private Player player;
    public void SetupCrystal(float _crystalDuration,bool _canExplode,Transform _closestTarget)
    {
        crystalExistTimer = _crystalDuration;
        canExplode = _canExplode;
        //canMove = _canMove;
        //moveSpeed = _moveSpeed;
        closestTarget = _closestTarget;
    }

    private void Start()
    {
        player = PlayerManager.instance.player;
    }
    private void Update()
    {
        crystalExistTimer -= Time.deltaTime;

        if (crystalExistTimer < 0)
        {
            CrystalFinish();
        }
        //if (canMove)
        //{
        //    transform.position = Vector2.MoveTowards(transform.position, closestTarget.position, moveSpeed * Time.deltaTime);
        //    if(Vector2.Distance(transform.position, closestTarget.position) < 1)
        //    {
        //        CrystalFinish();
        //    }
        //}
        if (canGrow)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(3, 3), growSpeed * Time.deltaTime);
        }
    }

    private void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
                player.stats.DoMagicalDamage(hit.GetComponent<CharacterStats>());
        }
    }

    public void CrystalFinish()
    {
        if (canExplode)
        {
            canGrow = true;
            anim.SetTrigger("Explode");
        }
        else
        {
            SelfDestory();
        }
    }

    public void SelfDestory() => Destroy(gameObject);
}
