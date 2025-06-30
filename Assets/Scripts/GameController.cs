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
    /// 待消除的水果列表
    /// </summary>
    /// <typeparam name="FruitItem"></typeparam>
    /// <returns></returns>
    private List<FruitItem> needToEliminateList = new List<FruitItem>();

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
        FruitChange(fruitItem, targetItem);
        needToEliminateList.Clear();
        yield return new WaitForSeconds(0.2f);
        //检测消除
        if (CheckEliminateHorizontal() || CheckEliminateVertical())
        {
            RemoveEliminateFruits();
            yield return new WaitForSeconds(0.6f);
            //上方水果掉落
            DropOtherFruits();
            //消除列表置空
            needToEliminateList.Clear();
            yield return new WaitForSeconds(0.2f);
            //再次检测
            StartCoroutine(AutoEliminate());
        }
        else
        {
            //没有任何消除，回退交换
            FruitChange(fruitItem, targetItem);
        }
    }

    void FruitChange(FruitItem fruitItem, FruitItem targetItem)
    {
        var tempRow = fruitItem.row;
        var tempCol = fruitItem.col;
        fruitItem.UpdateRowCol(targetItem.row, targetItem.col);
        targetItem.UpdateRowCol(tempRow, tempCol);
        //置空
        curFruiItem = null;

    }

    bool CheckEliminateHorizontal()
    {
        bool isEliminate = false;
        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            for (int colIndex = 0; colIndex < colCount - 2; colIndex++)
            {
                var item1 = fruitItemDict[rowIndex][colIndex];
                var item2 = fruitItemDict[rowIndex][colIndex + 1];
                var item3 = fruitItemDict[rowIndex][colIndex + 2];
                if (null != item1 && null != item2 && null != item3 && item1.type == item2.type && item2.type == item3.type)
                {
                    isEliminate = true;
                    if (!needToEliminateList.Contains(item1)) needToEliminateList.Add(item1);
                    if (!needToEliminateList.Contains(item2)) needToEliminateList.Add(item2);
                    if (!needToEliminateList.Contains(item3)) needToEliminateList.Add(item3);
                }
            }
        }
        return isEliminate;
    }

    bool CheckEliminateVertical()
    {
        bool isEliminate = false;
        for (int colIndex = 0; colIndex < colCount; colIndex++)
        {
            for (int rowIndex = 0; rowIndex < rowCount - 2; rowIndex++)
            {
                var item1 = fruitItemDict[rowIndex][colIndex];
                var item2 = fruitItemDict[rowIndex + 1][colIndex];
                var item3 = fruitItemDict[rowIndex + 2][colIndex];
                if (null != item1 && null != item2 && null != item3 && item1.type == item2.type && item2.type == item3.type)
                {
                    isEliminate = true;
                    if (!needToEliminateList.Contains(item1)) needToEliminateList.Add(item1);
                    if (!needToEliminateList.Contains(item2)) needToEliminateList.Add(item2);
                    if (!needToEliminateList.Contains(item3)) needToEliminateList.Add(item3);
                }
            }
        }
        return isEliminate;
    }

    void RemoveEliminateFruits()
    {
        for (int i = needToEliminateList.Count - 1; i >= 0; i--)
        {
            Destroy(needToEliminateList[i].gameObject);
            //字典数据更新,消除的只置空不删除，后续可做填充判断
            var row = needToEliminateList[i].row;
            var col = needToEliminateList[i].col;
            fruitItemDict[row][col] = null;
        }
    }
    /// <summary>
    /// 被消除的水果上方的水果需要掉落下来
    /// </summary>
    void DropOtherFruits()
    {
        for (int i = 0; i < needToEliminateList.Count; i++)
        {
            var startRow = needToEliminateList[i].row + 1;
            for (int j = startRow; j < rowCount; i++)
            {
                var dropFruit = fruitItemDict[j][needToEliminateList[i].col];
                dropFruit.UpdateRowCol(dropFruit.row - 1, dropFruit.col);
            }

        }
        //删除的
    }

    IEnumerator AutoEliminate()
    {
        if (CheckEliminateHorizontal() || CheckEliminateVertical())
        {
            RemoveEliminateFruits();
            yield return new WaitForSeconds(0.2f);
            DropOtherFruits();
            //消除列表置空
            needToEliminateList.Clear();
            yield return new WaitForSeconds(0.6f);
            //递归调用
            StartCoroutine(AutoEliminate());
        }
    }
}
