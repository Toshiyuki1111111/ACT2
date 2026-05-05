using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    public Player player;
    public int soul;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public bool HaveEnoughMoney(int _price)
    {
        if (_price > soul)
        {
            Debug.Log("Ç®²»¹»!");
            return false;
        }
        soul -= _price;
        return true;
    }

    public int CurrentSoul()
    {
        return soul;
    }
}