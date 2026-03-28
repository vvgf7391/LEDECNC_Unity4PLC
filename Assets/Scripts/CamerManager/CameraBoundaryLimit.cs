using game4automation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class CameraBoundaryLimit : BaseManager
{
    private Camera mianCamera;
    [Header("�Ƿ���������Ӧ�߽�")]
    public bool AdaptationLimit;

    [Header("��߽�")]
    public float minLimit_L;
    [Header("�ұ߽�")]
    public float maxLimit_R;
    [Header("ǰ�߽�")]
    public float maxLimit_F;
    [Header("��߽�")]
    public float minLimit_B;

    [Header("�߶�����")]
    public float maxHigth_U;
    [Header("�߶�����")]
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
    /// ���ݳ��س���������ȷ���߽�����
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
