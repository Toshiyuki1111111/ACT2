using System;
using UnityEngine;

public enum SwordType
{
    Regular,//常规
    Bounce,//反弹
    Pierce,
    Spin
}

public class Sword_Skill : Skill
{
    public SwordType swordType = SwordType.Regular;
    [Header("Bounce info")]
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;

    [Header("Peirce info")]
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravity;

    [Header("Skill info")]
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float swordGravity;

    private Vector2 finalDir;
    [Header("Aim Adjustment")]
    [SerializeField] private float angleAdjustmentSpeed = 50f; // 角度调整速度（度/秒）
    [SerializeField] private float maxAdjustmentAngle = 80f;   // 最大调整角度

    private float currentAngleOffset = 10f; // 当前角度偏移
    private bool isAdjustingAngle = false; // 是否在调整角度
    private int aimStartFacingDir; // 记录开始瞄准时的朝向

    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBetweenDots;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Transform dotsParent;

    private GameObject[] dots;

    protected override void Start()
    {
        base.Start();

        GenerateDots();
        SetupGravity();

    }

    private void SetupGravity()
    {
        if (swordType == SwordType.Bounce)
            swordGravity = bounceGravity;
        else if (swordType == SwordType.Pierce)
            swordGravity = pierceGravity;
    }

    protected override void Update()
    {
        base.Update();

        if (player == null)
            return;

        // 按下U键开始瞄准
        if (Input.GetKeyDown(KeyCode.U))
        {
            isAdjustingAngle = true;
            currentAngleOffset = 0f; // 重置角度偏移
            aimStartFacingDir = player.facingDir; // 记录开始瞄准时的朝向
            DotsActive(true);
        }
        if (Input.GetKeyUp(KeyCode.U))
        {
            isAdjustingAngle = false;
            // 松开U键时发射（这里只处理冷却时间，剑会在动画事件中创建）
            if (CanUseSkill())
            {
                cooldownTimer = cooldown; // 只设置冷却时间，不创建剑
            }
            DotsActive(false);
        }

        // 在调整角度状态下，使用W/S键调整角度
        if (isAdjustingAngle)
        {
            HandleAngleAdjustment();
        }

        // 计算最终方向
        UpdateFinalDirection();

        // 显示/更新轨迹点
        if (Input.GetKey(KeyCode.U))
        {
            UpdateDotsPosition();
        }
    }

    private void HandleAngleAdjustment()
    {
        float adjustmentInput = 0f;

        if (Input.GetKey(KeyCode.W))
            adjustmentInput = 1f;
        else if (Input.GetKey(KeyCode.S))
            adjustmentInput = -1f;

        if (aimStartFacingDir == -1)
            adjustmentInput *= -1;

        // 计算角度偏移
        currentAngleOffset += adjustmentInput * angleAdjustmentSpeed * Time.deltaTime;
        currentAngleOffset = Mathf.Clamp(currentAngleOffset, -maxAdjustmentAngle, maxAdjustmentAngle);
    }

    private void UpdateFinalDirection()
    {
        // 使用开始瞄准时的朝向作为基础方向
        Vector2 baseDirection = aimStartFacingDir == 1 ? Vector2.right : Vector2.left;

        // 应用角度偏移
        float baseAngle = Mathf.Atan2(baseDirection.y, baseDirection.x) * Mathf.Rad2Deg;
        float adjustedAngle = baseAngle + currentAngleOffset;
        Vector2 adjustedDirection = new Vector2(
            Mathf.Cos(adjustedAngle * Mathf.Deg2Rad),
            Mathf.Sin(adjustedAngle * Mathf.Deg2Rad)
        ).normalized;

        finalDir = new Vector2(
            adjustedDirection.x * launchForce.x,
            adjustedDirection.y * launchForce.y
        );
    }

    private void UpdateDotsPosition()
    {
        for (int i = 0; i < numberOfDots; i++)
        {
            if (dots[i] != null)
                dots[i].transform.position = DotsPosition(i * spaceBetweenDots);
        }
    }

    // 这个方法现在由动画事件调用
    public void CreateSword()
    {
        if (player == null || swordPrefab == null)
            return;

        GameObject newSword = Instantiate(swordPrefab, player.transform.position, transform.rotation);
        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>();

        if (swordType == SwordType.Bounce)
            newSwordScript.SetupBounce(true, bounceAmount);
        else if (swordType == SwordType.Pierce)
            newSwordScript.SetupPierce(pierceAmount);

        newSwordScript.SetupSword(finalDir, swordGravity, player);
        player.AssignNewSword(newSword);

        // 重置状态
        isAdjustingAngle = false;
        currentAngleOffset = 0f;
    }

    #region Aim region
    public void DotsActive(bool _isActive)
    {
        if (dots == null || player.sword != null)//try
            return;

        for (int i = 0; i < dots.Length; i++)
        {
            if (dots[i] != null)
                dots[i].SetActive(_isActive);
        }
    }

    private void GenerateDots()
    {
        if (dotPrefab == null || dotsParent == null)
            return;

        dots = new GameObject[numberOfDots];
        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotPrefab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }

    private Vector2 DotsPosition(float t)
    {
        if (player == null)
            return Vector2.zero;

        // 使用当前finalDir的方向计算轨迹点
        Vector2 direction = finalDir.normalized;
        float speed = finalDir.magnitude;

        Vector2 position = (Vector2)player.transform.position +
                          direction * speed * t +
                          0.5f * (Physics2D.gravity * swordGravity) * (t * t);

        return position;
    }
    #endregion
}