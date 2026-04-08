using game4automation;
using LeadNameSpace;
using MathNet.Numerics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static game4automation.Game4AutomationBehavior;
using static game4automation.TwinCatAdsInterface;

public class TwinCATinterfaceElment : MonoBehaviour
{
    public Dropdown dropdownActive;
    public InputField inputFieldID;
    public InputField inputFieldPort;
    public Dropdown dropdownMode;
    public InputField inputFieldCommands;
    public Toggle toggleDeBug;
    private int Activetag;
    private int Modetag;

    public Button RemoveAllSignal;
    public Button AddAllSignal;
    
    public TwinCatAdsInterface TwinCatAdsInterface;

    public Toggle UpDateToggle;

    private bool UpdateSignal;
    private void Start()
    {
        RemoveAllSignal.onClick.AddListener(() => { TwinCatAdsInterface.DestroyAllSignals();    });
        AddAllSignal.onClick.AddListener(() => { 
            TwinCatAdsInterface.ImportAllSignals();
            // TwinCatAdsInterface.ImportSignals(false);
        });
        GetTwinCatAllDate();
        StartCoroutine("UpdateGetTwinCatAllDate") ;
        dropdownActive.onValueChanged.AddListener(SetTwinCATActive);
        dropdownMode.onValueChanged.AddListener(SetTwinCatUpdateMode);
        toggleDeBug.onValueChanged.AddListener(SetDebugMod);
        UpDateToggle.onValueChanged.AddListener( SetUpdateSignal);


    }
    /// <summary>
    /// 设置连接状态
    /// </summary>
    /// <param name="tag"></param>
    public void SetTwinCATActive(int tag) 
    {
        switch (tag)
        {
            case 0: TwinCatAdsInterface.Active=ActiveOnly.Always;   break;
            case 1: TwinCatAdsInterface.Active=ActiveOnly.Connected; break;
            case 2: TwinCatAdsInterface.Active=ActiveOnly.Disconnected; break;
            case 3: TwinCatAdsInterface.Active=ActiveOnly.Never; break;
            case 4: TwinCatAdsInterface.Active=ActiveOnly.DontChange; break;
        };
    }
    /// <summary>
    /// 设置更新模式
    /// </summary>
    /// <param name="tag"></param>
    public void SetTwinCatUpdateMode(int tag) 
    {
        switch (tag)
        {
            case 0: TwinCatAdsInterface.UpdateMode= updatemode.Cyclic; break;
            case 1: TwinCatAdsInterface.UpdateMode = updatemode.OnChange; break;
            case 2: TwinCatAdsInterface.UpdateMode = updatemode.CyclicSumCommand; break;
        };
    }

    public void SetID() 
    {
        TwinCatAdsInterface.NetId = inputFieldID.text;
    }
    public void SetPort()
    {
        TwinCatAdsInterface.Port = Convert.ToInt32(inputFieldPort.text);
    }
    public void SetCommands() 
    {
        TwinCatAdsInterface.MaxNumberADSSubCommands= Convert.ToInt32(inputFieldCommands.text);
    }
    public void SetDebugMod(bool open) 
    {
        Debug.LogWarning(open);
        TwinCatAdsInterface.DebugMode = toggleDeBug.enabled;
    }
    IEnumerator UpdateGetTwinCatAllDate() 
    {
        while (true) 
        {
            if (!UpdateSignal)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }
            else 
            {
                GetTwinCatAllDate();
                Debug.LogWarning("Tw");
                yield return new WaitForSeconds(0.5f);
            }
            
        }
    }

    /// <summary>
    /// 获取ADS初始化信息
    /// </summary>
    public void  GetTwinCatAllDate() 
    {
        //act
        switch (TwinCatAdsInterface.Active) 
        {
            case ActiveOnly.Always: Activetag = 0; break;
            case ActiveOnly.Connected: Activetag = 1; break;
            case ActiveOnly.Disconnected: Activetag = 2; break;
            case ActiveOnly.Never: Activetag = 3; break;
            case ActiveOnly.DontChange: Activetag = 4; break;
        } ;
        dropdownActive.value = Activetag;
        //id
        inputFieldID.text = TwinCatAdsInterface.NetId;
        //port
        inputFieldPort.text = TwinCatAdsInterface.Port.ToString();
        //mode
        switch (TwinCatAdsInterface.UpdateMode)
        {
            case updatemode.Cyclic: Modetag = 0; break;
            case updatemode.OnChange: Modetag = 1; break;
            case updatemode.CyclicSumCommand: Modetag = 2; break;
        };
        dropdownMode.value = Modetag;
        //maxads
        inputFieldCommands.text= TwinCatAdsInterface.MaxNumberADSSubCommands.ToString();
        //debug
        toggleDeBug.SetIsOnWithoutNotify(TwinCatAdsInterface.DebugMode);
    }


    public void SetUpdateSignal(bool act) 
    {
        UpdateSignal = act;

        //LeadUIManager.Instance.GetUIManager<RigthPlan>().GetAllElementDic();
        //foreach (var item in  LeadUIManager.Instance.GetUIManager<RigthPlan>().GetAllElementDic())
        //{
        //    item.Value.SetUpdateSignal(UpdateSignal);
        //}
        //;
    }
}
