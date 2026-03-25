using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header("主要属性 Major stats")]
    public Stat strength;//力量，增加伤害,暴击伤害
    public Stat agility;//敏捷，增加闪避，暴击率
    public Stat intellgence;//智力，增加法伤，法抗
    public Stat vitality;//活力，增加生命

    [Header("攻击属性 Offensive stats")]
    public Stat damage;
    public Stat critChance;//暴击率
    public Stat critDamage;//暴击伤害
    //默认0暴击150爆伤

    [Header("防御属性 Defenive stats")]
    public Stat maxHealth;
    public Stat armor;//护甲
    public Stat evasion;//闪避

    [Header("魔法属性 Magic stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat ligitningDamage;

    public bool isIgnited;//点燃
    public bool isChilled;//冻结
    public bool isShocked;//触电（electrocution）

    [SerializeField] private int currentHealth;
    protected virtual void Start()
    {
        critChance.SetDefaultValue(0);
        critDamage.SetDefaultValue(150);
        currentHealth = maxHealth.GetValue();
    }
    public virtual void DoDamage(CharacterStats _targetStats)
    {
        if (CanAvoid(_targetStats))
        {
            return;
        }
        int totalDamage = damage.GetValue() + strength.GetValue();
        if (CanCrit())
        {
            totalDamage=CalculateCritDamage(totalDamage);
        }
        totalDamage -= _targetStats.armor.GetValue();
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
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
    private bool CanAvoid(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();
        if (UnityEngine.Random.Range(1, 100) < totalEvasion)
        {
            Debug.Log("Miss!");
            return true;
        }

        return false;
    }

    private bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();
        if (UnityEngine.Random.Range(1, 100) < totalCriticalChance)
        {
            Debug.Log("Crit!");
            return true;
        }
        return false;
    }

    private int CalculateCritDamage(int _damage)
    {
        float totalCritPower = (this.critDamage.GetValue() + strength.GetValue()) * .01f;//计算暴击伤害倍率
        float critDamage = _damage * totalCritPower;
        return Mathf.RoundToInt(critDamage);
    }
}
