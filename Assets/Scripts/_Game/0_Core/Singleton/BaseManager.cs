using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
/// <summary>
/// 不继承MonoBehaviour的单例类
/// 
/// 使用此类的时候必须私有化构造函数，否则反射拿不到私有构造函数，单例无法实例化
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseManager<T> where T : class
{
  private static T instance;
    public static T Instance
    {
        get {
            if (instance == null)
            {
                Type type = typeof(T);
               ConstructorInfo info = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                if (info != null)
                {
                    instance = info.Invoke(null) as T;
                }
                else
                {
                    Debug.LogError("没有得到对应的无参构造函数");
                }
            }
           
            //instance = new T();
            return instance; }
    }
}
