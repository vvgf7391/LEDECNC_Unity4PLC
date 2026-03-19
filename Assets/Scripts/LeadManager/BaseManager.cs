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
    [TextArea]
    [Header("功能介绍")]
    public string introduction;
    public virtual void Awake()
    {
        Debug.LogError(this.name);
        LeadManager.Instance.SetManager(this);
    }
    

}
