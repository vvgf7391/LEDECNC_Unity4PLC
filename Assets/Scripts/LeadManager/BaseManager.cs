using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LeadNameSpace;
using UnityEngine.Timeline;
using NaughtyAttributes;
/// <summary>
/// 脚本基类
/// </summary>
public class BaseManager : MonoBehaviour
{
    [HideIf("是否保留该脚本")]
    public bool isDestroy;
    [TextArea]
    [Header("功能介绍")]
    public string introduction;
    public virtual void Awake()
    {
        IsDestroy();
        Debug.LogError(this.name);
        LeadManager.Instance.SetManager(this);
    }
    public void IsDestroy()
    {
        if (isDestroy)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

}
