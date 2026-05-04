using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Armor,
    Amulet,
    Flask
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Equipment")]
public class ItemData_Equipment : ItemData
{
    public EquipmentType equipmentType;
    public float itemCooldowm;
    public ItemEffect[] itemEffects;
    [TextArea]
    public string itemEffectDescription;

    [Header("主要属性 Major stats")]
    public int strength;//力量
    public int agility;//敏捷
    public int intelligence;//智力
    public int vitality;//活力

    [Header("攻击属性 Offensive stats")]
    public int damage;
    public int critChance;//暴击率
    public int critDamage;//暴击伤害

    [Header("防御属性 Defenive stats")]
    public int maxHealth;
    public int armor;//护甲
    public int evasion;//闪避
    public int magicResistance;

    [Header("魔法属性 Magic stats")]
    public int fireDamage;
    public int iceDamage;
    public int ligitningDamage;

    [Header("Craft requirements")]
    public List<InventoryItem> craftMaterials;

    public void Effect(Transform _enemyPosition)
    {
        foreach(var item in itemEffects)
        {
            item.ExecuteEffect(_enemyPosition);
        }
    }

    public void AddModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.AddModifier(strength);
        playerStats.agility.AddModifier(agility);
        playerStats.intelligence.AddModifier(intelligence);
        playerStats.vitality.AddModifier(vitality);

        playerStats.damage.AddModifier(damage);
        playerStats.critChance.AddModifier(critChance);
        playerStats.critDamage.AddModifier(critDamage);

        playerStats.maxHealth.AddModifier(maxHealth);
        playerStats.armor.AddModifier(armor);
        playerStats.evasion.AddModifier(evasion);
        playerStats.magicResistance.AddModifier(magicResistance);

        playerStats.fireDamage.AddModifier(fireDamage);
        playerStats.iceDamage.AddModifier(iceDamage);
        playerStats.ligitningDamage.AddModifier(ligitningDamage);

    }

    public void RemoveModifiers()
    {
        PlayerStats playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        playerStats.strength.RemoveModifier(strength);
        playerStats.agility.RemoveModifier(agility);
        playerStats.intelligence.RemoveModifier(intelligence);
        playerStats.vitality.RemoveModifier(vitality);

        playerStats.damage.RemoveModifier(damage);
        playerStats.critChance.RemoveModifier(critChance);
        playerStats.critDamage.RemoveModifier(critDamage);

        playerStats.maxHealth.RemoveModifier(maxHealth);
        playerStats.armor.RemoveModifier(armor);
        playerStats.evasion.RemoveModifier(evasion);
        playerStats.magicResistance.RemoveModifier(magicResistance);

        playerStats.fireDamage.RemoveModifier(fireDamage);
        playerStats.iceDamage.RemoveModifier(iceDamage);
        playerStats.ligitningDamage.RemoveModifier(ligitningDamage);
    }

    public override string GetDescription()
    {
        sb.Length = 0;

        AddItemDescription(strength, "力量");
        AddItemDescription(agility, "敏捷");
        AddItemDescription(intelligence, "智识");
        AddItemDescription(vitality, "体质");

        AddItemDescription(damage, "攻击力");
        AddItemDescription(critChance, "暴击率");
        AddItemDescription(critDamage, "暴击伤害");

        AddItemDescription(maxHealth, "生命值");
        AddItemDescription(evasion, "闪避");
        AddItemDescription(armor, "护甲");
        AddItemDescription(magicResistance, "魔法抗性");

        AddItemDescription(fireDamage, "火");
        AddItemDescription(iceDamage, "冰");
        AddItemDescription(ligitningDamage, "雷");

        if (itemEffectDescription.Length > 0)
        {
            sb.AppendLine();
            sb.Append(WrapText(itemEffectDescription, 9));
        }

        return sb.ToString();
    }

    private string WrapText(string text, int charsPerLine)//扩展方法
    {
        if (string.IsNullOrEmpty(text) || charsPerLine <= 0)
            return text;

        var wrapped = new StringBuilder();
        for (int i = 0; i < text.Length; i += charsPerLine)
        {
            int remaining = text.Length - i;
            int take = Mathf.Min(charsPerLine, remaining);

            wrapped.Append(text.Substring(i, take));

            if (remaining > charsPerLine)
                wrapped.Append('\n');
        }
        return wrapped.ToString();
    }

    private void AddItemDescription(int _value,string _name)
    {
        if (_value != 0)
        {
            if (sb.Length > 0)
                sb.AppendLine();
            if (_value > 0)
                sb.Append("+ " + _value + " " + _name);
        }
    }

}
