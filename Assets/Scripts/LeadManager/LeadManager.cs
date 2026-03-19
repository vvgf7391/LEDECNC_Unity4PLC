using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 雷德管理类-统一管理脚本方法
/// LHZ
/// </summary>
namespace LeadNameSpace 
{
    public class LeadManager : MonoBehaviour
    {
        private static LeadManager _instance;
        public static LeadManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    // 尝试在场景中查找
                    _instance = FindObjectOfType<LeadManager>();
                    if (_instance == null)
                    {
                        // 如果找不到，创建一个新的游戏对象并挂载
                        GameObject go = new GameObject(typeof(LeadManager).Name);
                        _instance = go.AddComponent<LeadManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        public List<BaseManager> baseManagers = new List<BaseManager>();

        public T GetManager<T>() where T : BaseManager
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

        public void SetManager(BaseManager manager) 
        {
            if (!baseManagers.Contains(manager)) 
            {
                baseManagers.Add(manager);
            }
        }

    }
}

