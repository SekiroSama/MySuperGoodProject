using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManage
{
    private static ResourceManage _instance;
    public static ResourceManage Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ResourceManage();
            }
            return _instance;
        }
    }

    private Dictionary<string, AssetBundle> resourceCache = new Dictionary<string, AssetBundle>();
    private AssetBundleManifest manifest;
    public void Init()
    {
        AssetBundle ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + "Android");
        manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
    }

    private AssetBundle LoadABRes(string resourceName)
    {
        if(resourceCache.ContainsKey(resourceName))
        {
            return resourceCache[resourceName];
        }
        // Unity 底层对 Android 的 LoadFromFile 做了特殊处理，能直接读 APK 里的资源
        AssetBundle ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + resourceName);
        resourceCache.Add(resourceName, ab);

        string[] dependencies = manifest.GetAllDependencies(resourceName);
        foreach(string depend in dependencies)
        {
            if (!resourceCache.ContainsKey(depend))
            {
                LoadABRes(depend);
            }
        }

        return ab;
    }

    /// <summary>
    /// 加载ab中的资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="packageName">ab包名</param>
    /// <param name="resourceName">资源名</param>
    /// <returns></returns>
    public T LoadRes<T>(string packageName, string resourceName) where T : Object
    {
        AssetBundle ab = LoadABRes(packageName);
        if (ab == null)
        {
            Debug.LogError("Load AssetBundle Failed: " + packageName);
            return null;
        }
        return ab.LoadAsset<T>(resourceName);
    }
}
