using LeadNameSpace;
using libplctag;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddSignal : MonoBehaviour
{
    private int tag=0;
    public Button intBtn;
    public Button floatBtn;
    public Button boolBtn;

    public Transform CreatePoint;
    public GameObject gameSignalint;
    public GameObject gameSignalfloat;
    public GameObject gameSignalbool;
    // Start is called before the first frame update
    void Start()
    {
        intBtn.onClick.AddListener(() => {
            GameObject obj=Instantiate(gameSignalint, CreatePoint); 
            obj.name = tag + "_" + obj.name;
            tag++;
            LeadUIManager.Instance.GetUIManager<RigthPlan>().UpdateLocationSignal();
        });
        floatBtn.onClick.AddListener(() => {
            GameObject obj = Instantiate(gameSignalfloat, CreatePoint);
            obj.name = tag + "_" + obj.name ;
            tag++;
            LeadUIManager.Instance.GetUIManager<RigthPlan>().UpdateLocationSignal();
        });
        boolBtn.onClick.AddListener(() => {
            GameObject obj = Instantiate(gameSignalbool, CreatePoint);
            obj.name = tag + "_" + obj.name ;
            tag++;
            LeadUIManager.Instance.GetUIManager<RigthPlan>().UpdateLocationSignal();
        });
    }

}
