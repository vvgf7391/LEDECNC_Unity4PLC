using game4automation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;
public class CameraBoundaryLimit : BaseManager
{
    private Camera mianCamera;
    [Header("是否启用自适应边界")]
    public bool AdaptationLimit;

    [Header("左边界")]
    public float minLimit_L;
    [Header("右边界")]
    public float maxLimit_R;
    [Header("前边界")]
    public float maxLimit_F;
    [Header("后边界")]
    public float minLimit_B;

    [Header("高度上限")]
    public float maxHigth_U;
    [Header("高度下限")]
    public float minHigth_D;
    Transform cam;
    public override void Awake()
    {
        base.Awake();
        mianCamera = Camera.main;
    }
    
    private void Start()
    {   if (AdaptationLimit) 
        {
            Vector3 vector = this.transform.parent.localScale;
            SetPlanSize(vector.x, vector.z);
        }
    }

    private void FixedUpdate()
    {
        if (mianCamera != null)
        {
            Vector3 tragert = this.transform.position;
            tragert.x = Math.Clamp(tragert.x,minLimit_L,maxLimit_R);
            tragert.y = Math.Clamp(tragert.y,minHigth_D,maxHigth_U);
            tragert.z = Math.Clamp(tragert.z, minLimit_B, maxLimit_F);
            this.transform.position=tragert;
        }
    }

    /// <summary>
    /// 根据场地长、宽缩放确定边界限制
    /// </summary>
    /// <param name="size"></param>
    public void SetPlanSize(float x,float z) 
    {
        minLimit_L = x / 2 * -1;
        minLimit_B = z / 2 * -1;
        maxLimit_R = x / 2 ;
        maxLimit_F = z / 2 ;
    }

}
