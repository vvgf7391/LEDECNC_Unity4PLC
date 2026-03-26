using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using LeadNameSpace;
using Unity.VisualScripting;

public class BaseUIManager : MonoBehaviour
{
    [TextArea]
    [Header("面板功能介绍")]
    public string introduction;
    [Header("面板状态")]
    public bool currentStatus;

    public float width;
    public float height;
    public virtual void Awake()
    {
        LeadUIManager.Instance.SetUIManager(this);
        SetUIActive(currentStatus);
    }
    public virtual void Start() 
    {
        width = this.GetComponent<RectTransform>().sizeDelta.x;
        height = this.GetComponent<RectTransform>().sizeDelta.y;
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
        bool isOK=false;
        return this.transform.GetComponent<RectTransform>().DOAnchorPos(targetPos,time).OnComplete(() => { isOK = true; });
    }
}
