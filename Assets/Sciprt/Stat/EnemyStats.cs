using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;
    private ItemDrop myDropSystem;
    public Stat soulsDropAmount;
    protected override void Start()
    {
        base.Start();

        soulsDropAmount.SetDefaultValue(100);
        enemy = GetComponent<Enemy>();
        myDropSystem = GetComponent<ItemDrop>();
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
    }
    protected override void Die()
    {
        base.Die();
        enemy.Die();

        PlayerManager.instance.soul += soulsDropAmount.GetValue();
        myDropSystem.GenerateDrop();
        Destroy(gameObject, 2f);
    }
}
