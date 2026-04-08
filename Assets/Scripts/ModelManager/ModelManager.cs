using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class ModelManager : MonoBehaviour
{
    /// <summary>
    ///外壳
    /// </summary>
    public GameObject WK;
    public Toggle toggle;
    void Start()
    {
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            WK.SetActive(false);
        }
        else
        {
            WK.SetActive(true);
        }
    }
}
