using game4automation;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BaseElement : MonoBehaviour
{
    [HideInInspector]
    public Signal ThisSignal;
    public Text ElementName;
    public Text ElementType;
    public Toggle boolValue;
    public InputField Elementint;
    public InputField Elementfloat;
    public string Group;
    public SignalType signalType;
    public GameObject[] InputTypeValueObj;
    public enum SignalType
    { 
        InputBool, 
        InputInt, 
        InputFloat, 
        OutputBool, 
        OutputInt, 
        OutputFloat 
    }
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="signal"></param>
    public virtual void Initialization(Signal signal)
    {
        if (signal==null)
        {
            return;
        }
        ThisSignal=signal;

        Group = signal.transform.parent.name;
        ElementName.text=this.name = Group + ":" + signal.name;
        switch (signal) 
        {
            case PLCInputBool:      signalType= SignalType.InputBool; break;
            case PLCInputFloat:     signalType = SignalType.InputFloat; break;
            case PLCInputInt:       signalType = SignalType.InputInt; break;

            case PLCOutputBool: 
                signalType = SignalType.OutputBool;
                boolValue.SetIsOnWithoutNotify(signal.GetValue().Equals(true))  ;
                Elementfloat.transform.parent.gameObject.SetActive(false);
                Elementint.transform.parent.gameObject.SetActive(false);
                break;
            case PLCOutputFloat:
                signalType = SignalType.OutputFloat;
                Elementfloat.text = signal.GetValue().ToString();
                Elementint.transform.parent.gameObject.SetActive(false);
                boolValue.transform.parent.gameObject.SetActive(false);
                break;
            case PLCOutputInt:
                signalType = SignalType.OutputInt;
                Elementint.text = signal.GetValue().ToString();
                Elementfloat.transform.parent.gameObject.SetActive(false);
                boolValue.transform.parent.gameObject.SetActive(false);
                break;

        }
        ElementType.text = signalType.ToString();
    }
}
