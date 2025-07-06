using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    /// <summary>
    /// 动画结束回调委托
    /// </summary> <summary>
    public Action<string> aniEventCallback;


    /// <summary>
    /// 动画帧事件
    /// </summary>
    /// <param name="str"></param> <summary>
    public void OnAnimationFinishCallBack(string str)
    {
        aniEventCallback?.Invoke(str);

    }
}
