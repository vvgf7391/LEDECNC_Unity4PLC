using LeadNameSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// UI管理类
/// LHZ
/// </summary>
namespace LeadNameSpace 
{
    public class LeadUIManager : MonoBehaviour
    {
        private static LeadUIManager _instance;
        public static LeadUIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    // 尝试在场景中查找
                    _instance = FindObjectOfType<LeadUIManager>();
                    if (_instance == null)
                    {
                        // 如果找不到，创建一个新的游戏对象并挂载
                        GameObject go = new GameObject(typeof(LeadUIManager).Name);
                        _instance = go.AddComponent<LeadUIManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        public List<BaseUIManager> baseManagers = new List<BaseUIManager>();

        public T GetUIManager<T>() where T : BaseUIManager
        {
            foreach (var manager in baseManagers)
            {
                if (manager is T t)
                {
                    return t;
                }
            }
            return null;
        }

        public void SetUIManager(BaseUIManager manager)
        {
            if (!baseManagers.Contains(manager))
            {
                baseManagers.Add(manager);
            }
        }

    }
}

