using game4automation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListElementBody : BaseElement
{
    private bool updateSignal;
    
    public override void Initialization(Signal signal)
    {
        base.Initialization(signal);

    }

    private void Start()
    {
        StartCoroutine("UpdateVariableData");
        
    }

    public void SetUpdateSignal(bool tag) 
    {
        updateSignal = tag;
    }

    /// <summary>
    /// 数据更新
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdateVariableData() 
    {
        while (true) 
        {
            if (!updateSignal)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }
            else 
            {
                boolValue.SetIsOnWithoutNotify(ThisSignal.GetValue().Equals(true));
                Elementfloat.text = ThisSignal.GetValue().ToString();
                Elementint.text = ThisSignal.GetValue().ToString();
                yield return new WaitForSeconds(0.1f);
            }
            
        }
        
    }
    public void OnSetElementValue(object value) 
    {
        switch (signalType)
        {
            case SignalType.InputBool:
                ThisSignal.SetValue(Convert.ToBoolean(value));
                break;
            case SignalType.InputInt:
                ThisSignal.SetValue(Convert.ToInt32(value));
                break;
            case SignalType.InputFloat:
                ThisSignal.SetValue(Convert.ToSingle(value));
                break;
            case SignalType.OutputBool:
                ThisSignal.SetValue(Convert.ToBoolean(value));
                break;
            case SignalType.OutputInt:
                ThisSignal.SetValue(Convert.ToInt32(value));
                break;
            case SignalType.OutputFloat:
                ThisSignal.SetValue(Convert.ToSingle(value));
                break;
            default:
                break;
        }
    }
}
