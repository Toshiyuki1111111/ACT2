 using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
public enum StatType
{
    strength,
    agility,
    intelligence,
    vitality,
    damage,
    critChance,
    critDamage,
    maxHealth,
    armor,
    evasion,
    magicResistance,
    fireDamage,
    iceDamage,
    ligitningDamage,
}

public class CharacterStats : MonoBehaviour
{
    private EntityFX fx;

    public bool isDead = false;


    [Header("主要属性 Major stats")]
    public Stat strength;//力量，增加伤害,暴击伤害
    public Stat agility;//敏捷，增加闪避，暴击率
    public Stat intelligence;//智力，增加法伤，法抗
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
    public Stat magicResistance;

    [Header("魔法属性 Magic stats")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat ligitningDamage;

    
    [SerializeField] private float slowPercentage = .2f;

    public bool isIgnited;//点燃
    public bool isChilled;//寒冷
    public bool isShocked;//触电（electrocution shocked）
    /*点燃，持续造成燃烧伤害
      寒冷，降低防御力
      触电，降低伤害*/

    [SerializeField] private float ailmentsDuration = 2;
    private float ignitedTimer;
    private float chilledTimer;
    private float shockedTimer;


    private float igniteDamageCoodlown = .3f;
    private float igniteDamageTimer;
    private int igniteDamage;

    private int shockDamage;
    [SerializeField] private GameObject shockStrikePrefab;

    public int currentHealth;

    protected virtual void Start()
    {
        critChance.SetDefaultValue(0);
        critDamage.SetDefaultValue(150);
        currentHealth = GetMaxHealth();

        fx = GetComponent<EntityFX>();
    }
    public virtual void IncreaseStatBy(int _modifier, float _duration, Stat _statToModify)
    {
        StartCoroutine(StatBuff(_modifier, _duration, _statToModify));
    }

    private IEnumerator StatBuff(int _modifier, float _duration, Stat _statToModify)
    {
        _statToModify.AddModifier(_modifier);
        yield return new WaitForSeconds(_duration);
        _statToModify.RemoveModifier(_modifier);
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;
        chilledTimer -= Time.deltaTime;
        shockedTimer -= Time.deltaTime;
        igniteDamageTimer -= Time.deltaTime;


        if (ignitedTimer < 0)
        {
            isIgnited = false;
            //fx.CancelColorChange();
        }
        if(chilledTimer < 0)
        {
            isChilled = false;
            //fx.CancelColorChange();
        }
        if(shockedTimer < 0)
        {
            isShocked = false;
            //fx.CancelColorChange();
        }
        
        
        if (isIgnited && igniteDamageTimer < 0)
        {
            if (isDead) return;
            Debug.Log("Take burn damage " + igniteDamage);

            currentHealth -= igniteDamage;

            if (currentHealth < 0)
                Die();

            igniteDamageTimer = igniteDamageCoodlown;
        }
    }

    #region Calculate Damage
    public virtual void DoDamage(CharacterStats _targetStats)
    {
        if (CanAvoid(_targetStats))
        {
            return;
        }
        int totalDamage = damage.GetValue() + strength.GetValue();
        if (isChilled)//寒冷降低攻击
        {
            totalDamage = Mathf.RoundToInt(totalDamage * .8f);
        }
        if (CanCrit())
        {
            totalDamage=CalculateCritDamage(totalDamage);
        }
        if (_targetStats.isShocked)//触电降低护甲
        {
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * .8f);
        }
        else
        {
            totalDamage -= _targetStats.armor.GetValue();

        }
        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        _targetStats.TakeDamage(totalDamage);       //后续取消
        //DoMagicalDamage(_targetStats);
    }
    public virtual void TakeDamage(int _damage)
    {
        if (isDead) return;
        currentHealth -= _damage;

        GetComponent<Entity>().DamageImpact();
        fx.StartCoroutine("FlashFX");
        Debug.Log(_damage);

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private bool CanAvoid(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();
        float evasionChance = totalEvasion / (100f + totalEvasion);
        evasionChance = Mathf.Clamp(evasionChance, 0, 50);
        if (UnityEngine.Random.value < evasionChance)
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
    #endregion

    #region Magic Damage And Allments
    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _ligitningDamage = ligitningDamage.GetValue();
        int totalMagicalDamage = _fireDamage + _iceDamage + _ligitningDamage + intelligence.GetValue();
        totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);

        _targetStats.TakeDamage(totalMagicalDamage);

        //以下为附加状态逻辑
        bool applyIgnite = false;
        bool applyChill = false;
        bool applyShock = false;
        int maxMagicDamage = Mathf.Max(_fireDamage, _iceDamage, _ligitningDamage);
        if (Mathf.Max(_fireDamage, _iceDamage, _ligitningDamage) <= 0)
        {
            return;
        }

        if (maxMagicDamage == _fireDamage)
        {
            applyIgnite = true;
        }
        else if (maxMagicDamage == _iceDamage)
        {
            applyChill = true;
        }
        else
        {
            applyShock = true;
        }

        if (applyIgnite)
        {
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * .1f));
        }
        if (applyShock)
        {
            _targetStats.SetupShockStrikeDamage(Mathf.RoundToInt(_ligitningDamage * .1f));
        }

        _targetStats.ApplyAllments(applyIgnite, applyChill, applyShock);


    }
    public void ApplyAllments(bool _ignite,bool _chill,bool _shock)
    {
        if (isDead) return;
        bool canApplyIgnite = !isIgnited && !isChilled && !isShocked;
        bool canApplyChill = !isIgnited && !isChilled && !isShocked;
        bool canApplyShock = !isIgnited && !isChilled;
        if (_ignite && canApplyIgnite)
        {
            isIgnited = _ignite;
            ignitedTimer = ailmentsDuration;
            fx.IgniteFXFor(ailmentsDuration);
        }
        if (_chill && canApplyChill)
        {
            isChilled = _chill;
            chilledTimer = ailmentsDuration;
            GetComponent<Entity>().SlowEntityBy(slowPercentage, ailmentsDuration);
            fx.ChillFXFor(ailmentsDuration);
        }
        if (_shock)
        {
            if (!isShocked)
            {
                isShocked = _shock;
                shockedTimer = ailmentsDuration;
                fx.ShockFXFor(ailmentsDuration);
            }
            else
            {
                if (GetComponent<Player>() != null)
                    return;

                HitNearestTargetWithShockStrike();
            }
        }
    }
    #endregion

    #region Health

    public virtual void IncreaseHealthBy(int _amount)
    {
        currentHealth += _amount;
        if (currentHealth > GetMaxHealth())
            currentHealth = GetMaxHealth();
    }

    #endregion

    private void HitNearestTargetWithShockStrike()
    {
        //寻找目标 发射电弧
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 15);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null && Vector2.Distance(transform.position, hit.transform.position) > 1)
            {
                float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hit.transform;
                }
            }

            //if (closestEnemy != null)
            //    closestEnemy = transform;
        }

        if (closestEnemy != null)
        {
            GameObject newShockStrike = Instantiate(shockStrikePrefab, transform.position, Quaternion.identity);
            newShockStrike.GetComponent<ShockStrikeController>().Setup(shockDamage, closestEnemy.GetComponent<CharacterStats>());
        }
    }

    
    protected virtual void Die()
    {
        if (isDead) return;
        else
            isDead = true;
    }

    #region Setup AllmentsDamage
    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage;

    public void SetupShockStrikeDamage(int _damage) => shockDamage = _damage;
    #endregion

    public int GetMaxHealth()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 3;
    }

    public Stat GetStat(StatType _statType)
    {
        switch (_statType)
        {
            case StatType.strength:
                return strength;
            case StatType.agility:
                return agility;
            case StatType.intelligence:
                return intelligence;
            case StatType.vitality:
                return vitality;
            case StatType.damage:
                return damage;
            case StatType.critChance:
                return critChance;
            case StatType.critDamage:
                return critDamage;
            case StatType.maxHealth:
                return maxHealth;
            case StatType.armor:
                return armor;
            case StatType.evasion:
                return evasion;
            case StatType.magicResistance:
                return magicResistance;
            case StatType.fireDamage:
                return fireDamage;
            case StatType.iceDamage:
                return iceDamage;
            case StatType.ligitningDamage:
                return ligitningDamage;
            default:
                return null;
        }
    }
}
