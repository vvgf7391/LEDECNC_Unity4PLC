using DG.Tweening;
using game4automation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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

    private float width;
    private float height;

    [Space(10)]
    [Tooltip("创建UI元素的位置（Output类型）")]
    public Transform CreateElementTransform;
    [Tooltip("建议只放OutPut类型变量的父对象")]
    public List<GameObject> VariableParentObjectOutPut;
    [Tooltip("生成的UI物体")]
    public GameObject ListElementBody;

    private Dictionary<string, ListElementBody> dicElements = new Dictionary<string, ListElementBody>();
    private List<Signal> signals = new List<Signal>();
    public  void Start()
    {
        showBtn.onClick.AddListener(ShowBtnEven);
        hideBtn.onClick.AddListener(HideBtnEven);
        width = this.GetComponent<RectTransform>().sizeDelta.x;
        height = this.GetComponent<RectTransform>().sizeDelta.y;
        GetAllElements();
    }

    /// <summary>
    /// 获取所有元素
    /// </summary>
    public void GetAllElements() 
    {
        if (VariableParentObjectOutPut==null|| VariableParentObjectOutPut.Count==0)
        {
            return;
        }
        foreach (var element in VariableParentObjectOutPut)
        {
            signals.AddRange(element.GetComponentsInChildren<Signal>());
        }
        StartCoroutine("CreateElements");
    }
    IEnumerator CreateElements ()
    {
        GameObject obj;
        foreach (Signal signal in signals)
        {
            obj=Instantiate(ListElementBody, CreateElementTransform);
            obj.GetComponent<ListElementBody>().Initialization(signal);
        }
        yield return null;
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
