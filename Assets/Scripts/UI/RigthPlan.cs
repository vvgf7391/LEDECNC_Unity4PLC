using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 右侧面板
/// LHZ
/// </summary>
public class RigthPlan : BaseUIManager
{
    public Button showBtn;
    public Button hideBtn;

    public override void Start()
    {
        base.Start();
        showBtn.onClick.AddListener(ShowBtnEven);
        hideBtn.onClick.AddListener(HideBtnEven);
    }
    /// <summary>
    /// 点击显示按钮事件
    /// </summary>
    public void ShowBtnEven() 
    {
        SetPlanMove(new Vector2(-1*width, 0), 1).OnComplete(() =>
        {
            hideBtn.gameObject.SetActive(true);
            showBtn.gameObject.SetActive(false);
        });

    }
    /// <summary>
    /// 点击隐藏按钮事件
    /// </summary>
    public void HideBtnEven()
    {
        SetPlanMove(new Vector2(0, 0), 1).OnComplete(() =>
        {
            hideBtn.gameObject.SetActive(false);
            showBtn.gameObject.SetActive(true);
        });
    }
}
