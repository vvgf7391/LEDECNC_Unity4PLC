using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SetDegBug : MonoBehaviour
{
    private int tag=0;
    public InputField debugtext;
    public Transform SetCreatePoint;
    void OnEnable()
    {
        
        Application.logMessageReceivedThreaded += HandleThreadedLog;
    }

    void HandleThreadedLog(string logString, string stackTrace, LogType type)
    {
        GameObject obj =Instantiate(debugtext.gameObject, SetCreatePoint);
        switch (type)
        {
            case LogType.Error:
                obj.GetComponentInChildren<Text>().color = Color.red;
                break;
            case LogType.Warning:
                obj.GetComponentInChildren<Text>().color = Color.yellow;
                break;
            case LogType.Assert:
            case LogType.Log:
            case LogType.Exception:
            default:
                obj.GetComponentInChildren<Text>().color = Color.white;
                break;
        }
        //debugtext.text = tag+ "."+type + ":" +logString + "stackTrace:"+ stackTrace;
        obj.GetComponent<InputField>().text= DateTime.Now.ToString()+"_"+type.ToString()+"_"+"logString:" + logString + "stackTrace:" + stackTrace;
        
    }
}
