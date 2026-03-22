using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("Major stats")]
    public Stat strength;//力量，增加伤害
    public Stat agility;//敏捷，增加闪避
    public Stat intellgence;//智力，增加法伤法抗
    public Stat vitality;//活力，增加生命

    [Header("Defenive stats")]
    public Stat maxHealth;
    public Stat armor;




    public Stat damage;
    [SerializeField] private int currentHealth;
    protected virtual void Start()
    {
        currentHealth = maxHealth.GetValue();
    }
    public virtual void DoDamage(CharacterStats _targetStats)
    {
        int totalDamage = damage.GetValue() + strength.GetValue();
        _targetStats.TakeDamage(totalDamage);
    }
   public virtual void TakeDamage(int _damage)
   {
        currentHealth -= _damage;

        Debug.Log(_damage);

        if(currentHealth <= 0)
        {
            Die();
        }
   }

    protected virtual void Die()
    {

    }
}
