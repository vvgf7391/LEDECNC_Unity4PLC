using game4automation;
using LeadNameSpace;
using MathNet.Numerics;
using RuntimeInspectorNamespace;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
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

    private void Start()
    {
        RemoveAllSignal.onClick.AddListener(() => {
            try
            {
                TwinCatAdsInterface.DestroyAllSignals();
                LeadUIManager.Instance.GetUIManager<RigthPlan>().DestroyAllSignal();
            }
            catch (Exception error)
            {

                Debug.LogException(error);
            }
            
        });
        AddAllSignal.onClick.AddListener(() => {
            try
            {
                TwinCatAdsInterface.ImportSignals(false);
                LeadUIManager.Instance.GetUIManager<RigthPlan>().DestroyAllSignal();
                LeadUIManager.Instance.GetUIManager<RigthPlan>().GetAllElements();
            }
            catch (Exception error)
            {

                Debug.LogException(error);
            }
            
        });
        GetTwinCatAllDate();
        StartCoroutine("UpdateGetTwinCatAllDate");
        dropdownActive.onValueChanged.AddListener(SetTwinCATActive);
        dropdownMode.onValueChanged.AddListener(SetTwinCatUpdateMode);
        toggleDeBug.onValueChanged.AddListener(SetDebugMod);
        inputFieldID.onValueChanged.AddListener((str) => { SetID(str); });
        inputFieldPort.onValueChanged.AddListener((str) => { SetPort(str); });
        inputFieldCommands.onValueChanged.AddListener((str) => { SetCommands(str); });

    }
    /// <summary>
    /// 设置连接状态
    /// </summary>
    /// <param name="tag"></param>
    public void SetTwinCATActive(int tag)
    {
        switch (tag)
        {
            case 0: TwinCatAdsInterface.Active = ActiveOnly.Always; break;
            case 1: TwinCatAdsInterface.Active = ActiveOnly.Connected; break;
            case 2: TwinCatAdsInterface.Active = ActiveOnly.Disconnected; break;
            case 3: TwinCatAdsInterface.Active = ActiveOnly.Never; break;
            case 4: TwinCatAdsInterface.Active = ActiveOnly.DontChange; break;
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
            case 0: TwinCatAdsInterface.UpdateMode = updatemode.Cyclic; break;
            case 1: TwinCatAdsInterface.UpdateMode = updatemode.OnChange; break;
            case 2: TwinCatAdsInterface.UpdateMode = updatemode.CyclicSumCommand; break;
        };
    }

    public void SetID(string text)
    {
        if (text.IsNull())
        {
            return;
        }
        TwinCatAdsInterface.NetId = inputFieldID.text;
    }
    public void SetPort(string text)
    {
        if (text.IsNull())
        {
            return;
        }
        TwinCatAdsInterface.Port = Convert.ToInt32(inputFieldPort.text);
    }
    public void SetCommands(string text)
    {
        if (text.IsNull())
        {
            return;
        }
        TwinCatAdsInterface.MaxNumberADSSubCommands = Convert.ToInt32(inputFieldCommands.text);
    }
    public void SetDebugMod(bool open)
    {
        TwinCatAdsInterface.DebugMode = toggleDeBug.isOn;
    }
    IEnumerator UpdateGetTwinCatAllDate()
    {
        while (true)
        {
            GetTwinCatAllDate();
            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// 获取ADS初始化信息
    /// </summary>
    public void GetTwinCatAllDate()
    {
        //act
        switch (TwinCatAdsInterface.Active)
        {
            case ActiveOnly.Always: Activetag = 0; break;
            case ActiveOnly.Connected: Activetag = 1; break;
            case ActiveOnly.Disconnected: Activetag = 2; break;
            case ActiveOnly.Never: Activetag = 3; break;
            case ActiveOnly.DontChange: Activetag = 4; break;
        };
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
        inputFieldCommands.text = TwinCatAdsInterface.MaxNumberADSSubCommands.ToString();
        //debug
        toggleDeBug.SetIsOnWithoutNotify(TwinCatAdsInterface.DebugMode);
    }

}
