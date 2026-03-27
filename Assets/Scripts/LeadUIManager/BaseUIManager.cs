using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using LeadNameSpace;
using Unity.VisualScripting;

public class BaseUIManager : MonoBehaviour
{
    [TextArea(2,3)]
    [Header("面板功能介绍")]
    public string introduction;
    [Header("面板状态")]
    public bool currentStatus;


    public virtual void Awake()
    {
        LeadUIManager.Instance.SetUIManager(this);
        SetUIActive(currentStatus);
    }
 

    /// <summary>
    /// 设置面板显隐
    /// </summary>
    /// <param name="state"></param>
    public virtual void SetUIActive(bool state) 
    {
        if (currentStatus!= state)
        {
            currentStatus = state;
        }
        if (state == this.gameObject.activeSelf) 
        {
            return;
        }

        if (state)
        {
            this.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 设置面板动画
    /// </summary>
    /// <param name="vector2"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public Tween SetPlanMove(Vector2 targetPos, float time)
    {
        return this.transform.GetComponent<RectTransform>().DOAnchorPos(targetPos,time);
    }
}
