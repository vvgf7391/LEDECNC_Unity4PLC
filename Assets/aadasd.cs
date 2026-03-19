using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LeadNameSpace;
public class aadasd : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LeadManager.Instance.GetManager<CameraBoundaryLimit>().SetPlanSize(1,2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
