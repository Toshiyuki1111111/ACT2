using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole_Skill_Controller : MonoBehaviour
{
    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;

    private float maxSize;
    private float growSpeed;
    private float shrinkSpeed;

    private bool canGrow = true;
    private bool canShrink;
    private bool canCreateHotKeys = true;
    private bool cloneAttackReleased;

    private int amountOfAttacks = 4;
    private float cloneAttackCooldown = .3f;
    private float cloneAttackTimer;

    public List<Transform> targets = new List<Transform>();
    private List<GameObject> createHotKey = new List<GameObject>();

    public void SetupBlackhole(float _maxSize,float _growSpeed,float _shrinkSpeed,int _amountOfAttack,float _cloneAttackCooldown)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        amountOfAttacks = _amountOfAttack;
        cloneAttackCooldown = _cloneAttackCooldown;

    }



    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReleaseCloneAtatack();
        }

        CloneAttackLogic();

        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);

            if (transform.localScale.x < 0)
                Destroy(gameObject);
        }

    }

    private void ReleaseCloneAtatack()
    {
        DestoryHotKeys();
        cloneAttackReleased = true;
        canCreateHotKeys = false;

        PlayerManager.instance.player.MakeTransprent(true);
    }

    private void CloneAttackLogic()
    {
        if (cloneAttackTimer < 0 && cloneAttackReleased)
        {
            cloneAttackTimer = cloneAttackCooldown;
            int randomIndex = Random.Range(0, targets.Count);

            float xOffset;
            xOffset = Random.Range(0, 100) > 50 ? 2 : -2;

            SkillManager.instance.clone.CreateClone(targets[randomIndex], new Vector3(xOffset, 0));
            amountOfAttacks--;
        }

        if (amountOfAttacks <= 0)
        {
            Invoke("FinishBlackHoleAbility", .5f);
        }
    }

    private void FinishBlackHoleAbility()
    {
        PlayerManager.instance.player.ExitBlackHoleAbility();
        canShrink = true;
        cloneAttackReleased = false;
    }

    private void DestoryHotKeys()
    {
        if (createHotKey.Count < 0)
            return;
        for(int i = 0; i < createHotKey.Count; i++)
        {
            Destroy(createHotKey[i]);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(true);

            CreateHotKey(collision);

            //newHotKey.GetComponent<Blackhole_Hotkey_Controller>().SetupHotKey()
            //targets.Add(collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)=>collision.GetComponent<Enemy>().FreezeTime(false);

    private void CreateHotKey(Collider2D collision)
    {
        if (keyCodeList.Count <= 0)
        {
            Debug.LogWarning("Not enough hot keys in a key code list!");
            return;
        }

        if(!canCreateHotKeys)
        {
            return;
        }

        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);
        createHotKey.Add(newHotKey);
        KeyCode choosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(choosenKey);

        Blackhole_Hotkey_Controller newHotKeySciprt = newHotKey.GetComponent<Blackhole_Hotkey_Controller>();
        newHotKeySciprt.SetupHotKey(choosenKey, collision.transform, this);
    }

    public void AddEnemyToList(Transform _enemyTransform)=>targets.Add(_enemyTransform);
}
