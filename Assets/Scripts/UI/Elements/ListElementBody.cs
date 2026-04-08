using game4automation;
using RuntimeInspectorNamespace;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListElementBody : BaseElement
{
    
    public override void Initialization(Signal signal)
    {
        base.Initialization(signal);

    }

    private void Start()
    {
        StartCoroutine("UpdateVariableData");
        boolValue.onValueChanged.AddListener( (bool call)=> { SetSignalValue(SignalType.OutputBool); } );
        Elementint.onValueChanged.AddListener((string call) => { SetSignalValue(SignalType.OutputInt); });
        Elementfloat.onValueChanged.AddListener((string call) => { SetSignalValue(SignalType.OutputFloat); });
    }

    /// <summary>
    /// 设置信号值
    /// </summary>
    public void SetSignalValue(SignalType signalType) 
    {
        //try
        {
            switch (signalType)
            {
                case SignalType.InputBool:
                    ThisSignal.SetValue(boolValue.isOn);
                    break;
                case SignalType.InputInt:
                    if (!Elementint.text.IsNull()&&Elementint.text.Length > 0) 
                    {
                        ThisSignal.SetValue(Convert.ToInt32(Elementint.text));
                    }
                    break;
                case SignalType.InputFloat:
                    if (!Elementfloat.text.IsNull()&& Elementfloat.text.Length > 0) 
                    {
                        ThisSignal.SetValue(Convert.ToSingle(Elementfloat.text));
                    }
                    break;
                case SignalType.OutputBool:
                    ThisSignal.SetValue(boolValue.isOn);
                    break;
                case SignalType.OutputInt:
                    if (!Elementint.text.IsNull() && Elementint.text.Length > 0) 
                    {
                        ThisSignal.SetValue(Elementint.text);
                    }
                    break;
                case SignalType.OutputFloat:
                    if (!Elementfloat.text.IsNull()&&Elementfloat.text.Length > 0) 
                    {
                        ThisSignal.SetValue(Elementfloat.text);
                    }
                    break;
                default:
                    break;
            }
        }
       // catch (Exception e)
        {

            //Debug.Log("当前信号值设置有误"+e );
        }
        
        
    }

    /// <summary>
    /// 数据更新
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdateVariableData() 
    {
        while (true) 
        {
            boolValue.SetIsOnWithoutNotify(ThisSignal.GetValue().Equals(true));
            Elementfloat.text = ThisSignal.GetValue().ToString();
            Elementint.text = ThisSignal.GetValue().ToString();
            yield return new WaitForSeconds(0.1f);
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
