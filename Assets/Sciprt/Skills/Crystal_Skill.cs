using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crystal_Skill : Skill
{ 
    [SerializeField] private float crystalDuration;
    [SerializeField] private GameObject crystalPrefab;
    private GameObject currentCrystal;

    [SerializeField] private bool canMove;
    [SerializeField] private float moveSpeed;

    [Header("Crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockCrystalButton;
    public bool crystalUnlocked { get; private set; }

    [Header("Crystal Mirage")]
    [SerializeField] private UI_SkillTreeSlot unlockCrystalMirageButton;
    public bool crystalMirageUnlocked { get; private set; }

    [Header("Explosive Crystal")]
    [SerializeField] private UI_SkillTreeSlot unlockExplosiveCrystalButton;
    public bool explosiveCrystalUnlocked {  get; private set; }

    protected override void Start()
    {
        base.Start();

        unlockCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);
        unlockCrystalMirageButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalMirage);
        unlockExplosiveCrystalButton.GetComponent<Button>().onClick.AddListener(UnlockExplosiveCrystal);
    }

    private void UnlockCrystal()
    {
        if (unlockCrystalButton.unlocked)
            crystalUnlocked = true;
    }

    private void UnlockCrystalMirage()
    {
        if (unlockCrystalMirageButton.unlocked)
            crystalMirageUnlocked = true;
    }

    private void UnlockExplosiveCrystal()
    {
        if (unlockExplosiveCrystalButton.unlocked)
        {
            explosiveCrystalUnlocked = true;
        }
    }

    public override void UseSkill()
    {
        base.UseSkill();

        if (currentCrystal == null)
        {
            currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
            Crystal_Skill_Controller currentCrystalScript = currentCrystal.GetComponent<Crystal_Skill_Controller>();

            currentCrystalScript.SetupCrystal(crystalDuration,explosiveCrystalUnlocked,FindClosestEnemy(currentCrystal.transform));
        }
        else
        {
            if (crystalMirageUnlocked)
            {
                player.skill.clone.CreateClone(player.transform, Vector3.zero);
            }
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
