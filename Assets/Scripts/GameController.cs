using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("底部冰块网格的生成")]
    private int rowCount = 9;
    private int colCount = 7;
    public float iceWidth = 0.98f;
    public GameObject icePrefab;
    public Transform iceParent;

    [Header("水果的生成")]
    public Transform fruitParent;
    public GameObject[] fruitPrefabs;
    /// <summary>
    /// 存储冰块的位置,key1：row,key2：col,value：position
    /// </summary>
    public Dictionary<int, Dictionary<int, Vector3>> icePosDict = new Dictionary<int, Dictionary<int, Vector3>>();
    /// <summary>
    /// 存储所有的水果key1：row,key2：col,value：FruitItem
    /// </summary>

    public Dictionary<int, Dictionary<int, FruitItem>> fruitItemDict = new Dictionary<int, Dictionary<int, FruitItem>>();
    /// <summary>
    /// 当前选中的水果
    /// </summary>
    public FruitItem curFruiItem;

    /// <summary>
    /// 手指水平滑动量
    /// </summary>
    private float m_fingerMoveX;
    /// <summary>
    /// 手指竖直滑动量
    /// </summary>
    private float m_fingerMoveY;

    private static GameController _instance;

    public static GameController instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
        SpwanIce();
        SpawanFruits();

    }

    /// <summary>
    /// 冰块地图的生成
    /// </summary>
    void SpwanIce()
    {
        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            for (int colIndex = 0; colIndex < colCount; colIndex++)
            {
                // 实例化冰块物体
                var obj = Instantiate(icePrefab, iceParent);
                obj.transform.localPosition = new Vector3((colIndex - colCount / 2f) * iceWidth + iceWidth / 2f, (rowIndex - rowCount / 2f) * iceWidth + iceWidth / 2f, 0);
                //记录冰块位置
                if (!icePosDict.ContainsKey(rowIndex))
                {
                    icePosDict.Add(rowIndex, new Dictionary<int, Vector3>());
                }
                icePosDict[rowIndex].Add(colIndex, obj.transform.localPosition);
            }
        }
        icePrefab.SetActive(false);
        //根据分辨率动态调整缩放，让所有的冰块都能完整显示
        var baseRatio = 750f / 1334f * Screen.height;
        //新宽小于原宽，视野变窄，缩小
        var newWidthScale = Screen.width / baseRatio;
        if (newWidthScale < 1)
        {
            iceParent.localScale = new Vector3(iceParent.localScale.x * newWidthScale, iceParent.localScale.y * newWidthScale, 1);
        }

    }

    void SpawanFruits()
    {
        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            for (int colIndex = 0; colIndex < colCount; colIndex++)
            {
                //随机生成水果
                var fruitPrefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];
                var obj = Instantiate(fruitPrefab, fruitParent);
                //初始化
                var fruitItem = obj.GetComponent<FruitItem>();
                fruitItem.UpdateRowCol(rowIndex, colIndex);
            }
        }
        //设置缩放
        fruitParent.localScale = iceParent.localScale;
    }

    void Update()
    {
        //检测玩家的操作
        if (curFruiItem == null) return;
        if (Input.GetMouseButtonUp(0))
        {
            //释放水果
            curFruiItem = null;
            return;
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButton(0))
#else
     if(1 == Input.touchCount && Input.touches[0].phase == TouchPhase.Moved)
#endif
        {
            m_fingerMoveX = Input.GetAxis("Mouse X");
            m_fingerMoveY = Input.GetAxis("Mouse Y");
        }

        // 滑动量太小，不处理
        if (Mathf.Abs(m_fingerMoveX) < 0.3f && Mathf.Abs(m_fingerMoveY) < 0.3f)
            return;

        //水果移动
        OnFruitMove();
        //重置
        m_fingerMoveX = 0;
        m_fingerMoveY = 0;
    }
    /// <summary>
    /// 水果移动
    /// </summary>
    private void OnFruitMove()
    {
        if (Mathf.Abs(m_fingerMoveX) > Mathf.Abs(m_fingerMoveY))
        {
            //横向滑动
            var targetItem = fruitItemDict[curFruiItem.row][m_fingerMoveX > 0 ? curFruiItem.col + 1 : curFruiItem.col - 1];
            if (null != targetItem)
            {
                StartCoroutine(ExchangeAndMatch(curFruiItem, targetItem));
            }
            else
            {
                curFruiItem = null;
            }
        }
        else if (Mathf.Abs(m_fingerMoveX) < Mathf.Abs(m_fingerMoveY))
        {
            //纵向滑动
            var targetItem = fruitItemDict[m_fingerMoveY > 0 ? curFruiItem.row + 1 : curFruiItem.row - 1][curFruiItem.col];
            if (null != targetItem)
            {
                StartCoroutine(ExchangeAndMatch(curFruiItem, targetItem));
            }
            else
            {
                curFruiItem = null;
            }

        }
    }

    IEnumerator ExchangeAndMatch(FruitItem fruitItem, FruitItem targetItem)
    {
        //交换水果
        //检测消除
        yield return null;
    }
}
