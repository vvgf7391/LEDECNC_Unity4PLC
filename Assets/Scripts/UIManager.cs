using UnityEngine;
using UnityEngine.UI;
using game4automation;

public class UIManager : MonoBehaviour
{
    [SerializeField] private InputField netIdInputField;
    [SerializeField] private TwinCatAdsInterface twinCatAdsInterface;

    private void Awake()
    {

    }
    

 
    public void TextChange()
    {
        //检测输入的内容是否为数字
        string pattern = @"^[0-9.]*$";
        if (!System.Text.RegularExpressions.Regex.IsMatch(netIdInputField.text, pattern))
        {
            netIdInputField.text = System.Text.RegularExpressions.Regex.Replace(netIdInputField.text, @"[^0-9.]", "");
        }
    }
 
    public void TextEnd()
    {
        twinCatAdsInterface.NetId = netIdInputField.text;
        print("输入结束，当前NetId: " + twinCatAdsInterface.NetId);
    }

}
