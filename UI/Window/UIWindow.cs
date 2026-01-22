using System.Collections.Generic;
using UnityEngine;


public class UIWindowManager
{
    static Dictionary<string, UIWindowBase> windows = new Dictionary<string, UIWindowBase>();

    public static T GetWindow<T>(string path, string windowName, Transform root = null) where T : UIWindowBase
    {
        if (string.IsNullOrEmpty(windowName) == true)
            return null;

        if (windows.ContainsKey(windowName) == false || windows[windowName] == null)
            windows[windowName] = Load<T>(path, windowName, root);
        else if (root != null)
            windows[windowName].transform.parent = root;
        return windows[windowName] as T;
    }

    public static T Load<T>(string path, string windowName, Transform root = null) where T : UIWindowBase
    {
        if (string.IsNullOrEmpty(windowName) == true)
            return null;

        GameObject asset = AssetManager.Instance.GetAsset<GameObject>(path, windowName);
        if (root == null) root = ExampleActivity.Instance.UIRoot;
        GameObject clone = GameObject.Instantiate(asset, root);
        T win = clone.GetComponent<T>();
        win.enabled = false;
        return win;
    }
}
