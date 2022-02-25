using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineManager
{
    public static List<Action> lAction = new List<Action>();
    public static MonoBehaviour mono;
    public static void StartCoroutine(IEnumerator cb)
    {
        mono.StartCoroutine(cb);
    }
    public static void StopCoroutine(IEnumerator cb)
    {
        mono.StopCoroutine(cb);
    }

    public static void Init(MonoBehaviour monoBehaviour)
    {
        mono = monoBehaviour;
    }
}