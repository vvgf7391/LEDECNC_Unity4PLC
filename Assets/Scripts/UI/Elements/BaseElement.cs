using game4automation;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class BaseElement : MonoBehaviour
{
    [HideInInspector]
    public Signal ThisSignal;
    public Text ElementName;
    public Text ElementType;
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
    /// ≥ı ºªØ
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
            case PLCOutputBool:     signalType = SignalType.OutputBool; break;
            case PLCOutputFloat:    signalType = SignalType.OutputFloat; break;
            case PLCOutputInt:      signalType = SignalType.OutputInt; break;
        }
        ElementType.text = signalType.ToString();
        ShowInputObj(signal);
    }
    private void ShowInputObj(Signal signal)
    {
        switch (signal)
        {
            case PLCInputBool:
                InputTypeValueObj[1].SetActive(false);
                InputTypeValueObj[2].SetActive(false);
                break;
            case PLCInputFloat:
                InputTypeValueObj[0].SetActive(false);
                InputTypeValueObj[2].SetActive(false);
                break;
            case PLCInputInt:
                InputTypeValueObj[1].SetActive(false);
                InputTypeValueObj[0].SetActive(false);
                break;
            default:
                break;
        }
    }
}
