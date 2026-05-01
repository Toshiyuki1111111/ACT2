using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ItemTooltip : MonoBehaviour
{
    private string type;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI itemDescription;

    public void ShowToolTip(ItemData_Equipment item)
    {
        if (item == null) return;
        itemNameText.text = item.itemName;
        itemTypeText.text = item.equipmentType.ToString();
        if (itemTypeText.text == "Weapon") itemTypeText.text = "ÎäÆ÷";
        else if (itemTypeText.text == "Armor") itemTypeText.text = "¿ø¼×";
        else if (itemTypeText.text == "Amulet") itemTypeText.text = "»¤·û";
        else if (itemTypeText.text == "Flask") itemTypeText.text = "Ò©Ë®";

        itemDescription.text = item.GetDescription();

        gameObject.SetActive(true);
    }

    public void HideToolTip() => gameObject.SetActive(false);
}
