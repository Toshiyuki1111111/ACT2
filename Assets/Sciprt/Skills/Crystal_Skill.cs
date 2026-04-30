using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal_Skill : Skill
{ 
    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [SerializeField] private bool canExplode;
    [SerializeField] private bool canMove;
    [SerializeField] private float moveSpeed;
    public override void UseSkill()
    {
        base.UseSkill();

        if (currentCrystal == null)
        {
            currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
            Crystal_Skill_Controller currentCrystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>();

            currentCrystalScript.SetupCrystal(crystalDuration,canExplode,FindClosestEnemy(currentCrystal.transform));
        }
        else
        {
            Vector2 playerPos = player.transform.position;

            player.transform.position = currentCrystal.transform.position;
            currentCrystal.transform.position = playerPos;
            player.StartCoroutine(player.BusyFor(1f));
            //Destroy(currentCrystal);
            currentCrystal.GetComponent<Crystal_Skill_Controller>()?.CrystalFinish();
            //?.空条件运算符  返回 null 时，不会调用 CrystalFinish方法,整个表达式直接返回null，避免抛出 NullReferenceException
        }
    }
}
