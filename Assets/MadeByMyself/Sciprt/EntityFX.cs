using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    private SpriteRenderer sr;

    [Header("Flash FX")]
    [SerializeField] private Material hitMat;
    private Material originalMat;

    [Header("Ailment Color")]
    [SerializeField] private Color chillColor;
    [SerializeField] private Color[] igniteColor;
    [SerializeField] private Color[] shockColor;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMat = sr.material;
    }

    private IEnumerator FlashFX()
    {
        sr.material = hitMat;

        yield return new WaitForSeconds(.2f);

        sr.material = originalMat;
    }

    private void RedColorBlink()
    {
        if(sr.color != Color.white) 
            sr.color = Color.white;
        else
            sr.color = Color.red;
    }

    private void CancelColorChange()
    {
        CancelInvoke();
        sr.color = Color.white;
    }

    private void BlueColor()
    {
        sr.color = Color.blue;
    }

    private void CancelBlueColor()
    {
        sr.color= Color.white;
    }

    public void IgniteFXFor(float _seconds)
    {
        InvokeRepeating("IgniteColorFX", 0, .3f);
    }
     
    private void IgniteColorFX()
    {
        if (sr.color != igniteColor[0])
            sr.color = igniteColor[0];
        else 
            sr.color = igniteColor[1];
    }
}
