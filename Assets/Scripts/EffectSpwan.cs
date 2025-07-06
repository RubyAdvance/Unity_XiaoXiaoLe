using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 特效创建回收脚本
/// </summary> <summary>

public class EffectSpwan : MonoBehaviour
{
    public GameObject effectPrefab;
    public Transform effectRoot;
    private static EffectSpwan _instance;
    public static EffectSpwan Instance
    {
        get { return _instance; }
    }
    /// <summary>
    /// 特效对象池
    /// </summary>
    /// <typeparam name="GameObject"></typeparam>
    /// <returns></returns>
    private Queue<GameObject> effectPool = new Queue<GameObject>();

    private void Awake()
    {
        _instance = this;
    }
    public void ShowEffect(Vector3 pos)
    {
        GameObject go;
        if (effectPool.Count > 0)
        {
            //从池子中取
            go = effectPool.Dequeue();
        }
        else
        {
            go = Instantiate(effectPrefab);
            go.transform.SetParent(effectRoot, false);
            //监听动画帧事件
            var aniEvent = go.GetComponent<AnimationEvent>();
            if (aniEvent)
            {
                aniEvent.aniEventCallback = (str) =>
                {
                    if (str == "finish")
                    {
                        //动画播放结束
                        go.SetActive(false);
                        effectPool.Enqueue(go);
                    }
                };
            }

        }
        //设置位置
        go.transform.position = pos;
        //显示出来
        go.SetActive(true);

    }
}
