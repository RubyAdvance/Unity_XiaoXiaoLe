using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class FruitItem : MonoBehaviour
{
    public int type;
    public int col;
    public int row;


    public void UpdateRowCol(int row, int col)
    {
        // 添加边界检查
        if (row < 0 || col < 0 || row >= GameController.instance.rowCount || col >= GameController.instance.colCount)
        {
            Debug.LogWarning($"试图更新水果到无效位置: ({row}, {col})");
            return;
        }

        this.row = row;
        this.col = col;
        var targetPos = GameController.instance.icePosDict[row][col];
        transform.DOLocalMove(targetPos, 0.2f);

        // 确保字典中存在该行
        if (!GameController.instance.fruitItemDict.ContainsKey(row))
        {
            GameController.instance.fruitItemDict.Add(row, new Dictionary<int, FruitItem>());
        }

        // 更新字典引用
        GameController.instance.fruitItemDict[row][col] = this;
    }

    void OnMouseDown()
    {
        if (GameController.instance.curFruiItem != null) Debug.LogError("持有的不为空！！");
        GameController.instance.curFruiItem = this;
    }

    public void RecycleSelf()
    {
        Destroy(gameObject);
        //播放特效
        EffectSpwan.Instance.ShowEffect(transform.position);
        EffectSpwan.Instance.ShowScore(transform.position);
    }
}
