using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clone_Skill : Skill
{
    [Header("Clone info")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Header("Clone attack")]
    [SerializeField] private UI_SkillTreeSlot cloneAttackUnlockButton;
    [SerializeField] private bool canAttack;

    #region Unlock region
    private void UnlockCloneAttack()
    {
        if (cloneAttackUnlockButton.unlocked)
            canAttack = true;
    }

    protected override void CheckUnlock()
    {
        base.CheckUnlock();

        UnlockCloneAttack();
    }
    #endregion

    protected override void Start()
    {
        base.Start();

        cloneAttackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneAttack);
    }

    public void CreateClone(Transform _clonePosition,Vector3 _offset)
    {
        GameObject newClone = Instantiate(clonePrefab);

        newClone.GetComponent<Clone_Skill_Controller>().SetupClone(_clonePosition, cloneDuration,canAttack,_offset,FindClosestEnemy(newClone.transform));
    }
}
