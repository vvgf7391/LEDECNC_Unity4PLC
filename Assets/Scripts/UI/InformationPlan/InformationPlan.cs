using DG.Tweening;
using game4automation;
using LibUA.Core;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class InformationPlan : BaseUIManager
{
    public OPCUA_Interface OPCUA;
    #region 折叠下拉菜单及按钮
    [Header("折叠下拉菜单及按钮")]
    public Button Drop1Btn;
    public Button Drop2Btn;
    public Button Drop3Btn;
    public Button Drop4Btn;
    public RectTransform rect1;
    public RectTransform rect2;
    public RectTransform rect3;
    public RectTransform rect4;
    #endregion
    #region 文本输入框
    [Space(5)]
    [Header("核心文本输入框")]
    public InputField equipement;
    public InputField serverIP;
    public InputField serverPort;
    public InputField userName;
    public InputField passWord;
    [Space(5)]
    [Header("会话与节点控制输入框")]
    public InputField SessionTimeoutMs;
    public InputField TopNodeID;
    public Toggle DebugMode;
    public InputField MaxNONPS;
    [Space(5)]
    [Header("连接状态监控输入框")]
    public GameObject isConnected;
    public GameObject isReconnecting;
    public InputField ReconnectTime;
    [Space(5)]
    [Header("应用身份信息输入框")]
    public InputField AppllcationName;
    public InputField AppllcationURN;
    public InputField ProductURL;
    [Space(5)]
    [Header("证书与订阅策略输入框")]
    public InputField CPC;
    public InputField Path;
    public InputField SPI;
    public InputField MNPP;
    #endregion
    #region 特殊按钮
    //public Button OpenFilePathBtn;
    public Toggle RememberPassWordBtn;
    public Button RestBtn;
    public Button Connected;
    public Image StateImage;
    #endregion


    void Start()
    {
        Drop1Btn.onClick.AddListener(() => {
            if (rect1.gameObject.activeSelf)
            {
                rect1.gameObject.SetActive(false);
            }
            else
            {
                rect1.gameObject.SetActive(true);
            }
        });

        Drop2Btn.onClick.AddListener(() => {
            if (rect2.gameObject.activeSelf)
            {
                rect2.gameObject.SetActive(false); 
            }
            else
            {
                 rect2.gameObject.SetActive(true);
            }
        });

        Drop3Btn.onClick.AddListener(() => {
            if (rect3.gameObject.activeSelf)
            {
                rect3.gameObject.SetActive(false);
            }
            else
            {
               rect3.gameObject.SetActive(true);
            }
        });

        Drop4Btn.onClick.AddListener(() => {
            if (rect4.gameObject.activeSelf)
            {
                 rect4.gameObject.SetActive(false);
            }
            else
            {
               rect4.gameObject.SetActive(true);
            }
        });

        equipement.onValueChanged.AddListener((name) => { OPCUA.Name = name; PlayerPrefs.SetString("equipement",name); });
        serverIP.onValueChanged.AddListener((ip) => { OPCUA.ServerIP = ip; PlayerPrefs.SetString("serverIP", ip); });
        serverPort.onValueChanged.AddListener((port) => { OPCUA.ServerPort = Convert.ToInt32(port); PlayerPrefs.SetInt("serverPort", Convert.ToInt32(port)); });
        userName.onValueChanged.AddListener((username) => { OPCUA.UserName = username; PlayerPrefs.SetString("userName", username); });
        passWord.onValueChanged.AddListener((password) => { OPCUA.Password = password; PlayerPrefs.SetString("passWord", password); });

        SessionTimeoutMs.onValueChanged.AddListener((SessionTimeoutMs) => { OPCUA.SessionTimeoutMs =Convert.ToInt32( SessionTimeoutMs); PlayerPrefs.SetInt("SessionTimeoutMs", Convert.ToInt32(SessionTimeoutMs)); });
        TopNodeID.onValueChanged.AddListener((NodeID) => { OPCUA.TopNodeId = NodeID; PlayerPrefs.SetString("TopNodeID", NodeID); });
        DebugMode.onValueChanged.AddListener((DebugMode) => { OPCUA.DebugMode = DebugMode; PlayerPrefs.SetString("DebugMode", DebugMode.ToString()); });
        MaxNONPS.onValueChanged.AddListener((MaxNONPS) => { OPCUA.MaxNumberOfNodesPerSubscription = Convert.ToInt32(MaxNONPS); PlayerPrefs.SetInt("MaxNONPS", Convert.ToInt32(MaxNONPS)); });
        if (!OPCUA.IsConnected)
        {
            isConnected.GetComponentInChildren<Image>().color = Color.red;
            isConnected.GetComponentInChildren<Text>().text = "未连接";
            StateImage.color = Color.red;
        }
        else 
        {
            isConnected.GetComponentInChildren<Image>().color = Color.green;
            isConnected.GetComponentInChildren<Text>().text = "已连接";
            StateImage.color = Color.green;
        }
        if (!OPCUA.IsReconnecting)
        {
            isReconnecting.GetComponentInChildren<Image>().color = Color.red;
            isReconnecting.GetComponentInChildren<Text>().text = "否";
        }
        else 
        {
            isReconnecting.GetComponentInChildren<Image>().color = Color.yellow;
            isReconnecting.GetComponentInChildren<Text>().text = "是";
        }
        ReconnectTime.onValueChanged.AddListener((time) => { OPCUA.ReconnectTime = Convert.ToInt32(time); PlayerPrefs.SetInt("ReconnectTime", Convert.ToInt32(time)); });

        AppllcationName.onValueChanged.AddListener((Appname) => { OPCUA.ApplicationName = Appname; PlayerPrefs.SetString("AppllcationName", Appname); });
        AppllcationURN.onValueChanged.AddListener((urn) => { OPCUA.ApplicationURN = urn; PlayerPrefs.SetString("AppllcationURN", urn); });
        ProductURL.onValueChanged.AddListener((url) => { OPCUA.ProductURI = url; PlayerPrefs.SetString("ProductURL", url); });

        CPC.onValueChanged.AddListener((cpc) => { OPCUA.ClientPrivateCertificate = cpc; PlayerPrefs.SetString("CPC", cpc); });
        Path.onValueChanged.AddListener((path) => { OPCUA.ClientPublicCertificate = path; PlayerPrefs.SetString("Path", path); });
        SPI.onValueChanged.AddListener((spi) => { OPCUA.SubscriptionPublishingIntervall = Convert.ToSingle(spi); PlayerPrefs.SetFloat("SPI", Convert.ToSingle(spi)); });
        MNPP.onValueChanged.AddListener((mnpp) => { OPCUA.MaxNotificationsPerPublish = Convert.ToInt32(mnpp); PlayerPrefs.SetInt("MNPP", Convert.ToInt32(mnpp)); });

        bool RemberPassword=false;
        RememberPassWordBtn.onValueChanged.AddListener((rememberPassWord) => { RemberPassword = rememberPassWord; });
        RestBtn.onClick.AddListener(() => { });
        Connected.onClick.AddListener(() => { 
            if (RemberPassword) { PlayerPrefs.Save(); } 
        
        });
    }

    /// <summary>
    /// 设置OPCUA值
    /// </summary>
    private void SetOPCUAValue(string BserverIP,  string BserverPort, in string Bname, in string BuserName,in string BpassWord,
        in int BSessionTimeoutMs,in string BTopNodeID,in bool BDebugMode,in int BMaxNONPS,in int BReconnectTime,in string BAppllcationName,
        in string BAppllcationURN,in string BProductURL,in string BCPC,in string BPath,in float BSPI,in int BMNPP) 
    {
        equipement.text=Bname;
        serverIP.text = BserverIP;
        serverPort.text = BserverPort;
        userName.text = BuserName;
        passWord.text = BpassWord;
        SessionTimeoutMs.text = BSessionTimeoutMs.ToString();
        TopNodeID.text = BTopNodeID;
        DebugMode.isOn = BDebugMode;
        MaxNONPS.text = BMaxNONPS.ToString();
        ReconnectTime.text = BReconnectTime.ToString();
        AppllcationName.text = BAppllcationName;
        AppllcationURN.text = BAppllcationURN;
        ProductURL.text = BProductURL;
        CPC.text = BCPC;
        Path.text = BPath;
        SPI.text=BSPI.ToString();
        MNPP.text=BMNPP.ToString();
    }
    /// <summary>
    /// 保存的值
    /// </summary>
    private void ReadSaveValue()
    {
        //SetOPCUAValue("127.0.0.1","4840");
    }
    /// <summary>
    /// 默认值
    /// </summary>
    private void DefaultValue() 
    {
        //SetOPCUAValue("127.0.0.1","4840");
    }
}
