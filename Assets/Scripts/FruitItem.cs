using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitItem : MonoBehaviour
{
    public int type;
    public int col;
    public int row;


    public void UpdateRowCol(int row, int col)
    {
        this.row = row;
        this.col = col;
        //设置位置
        transform.localPosition = GameController.instance.icePosDict[row][col];
        //存储
        if (!GameController.instance.fruitItemDict.ContainsKey(row)) GameController.instance.fruitItemDict.Add(row, new Dictionary<int, FruitItem>());
        GameController.instance.fruitItemDict[row][col] = this;
    }

    void OnMouseDown()
    {
        if(GameController.instance.curFruiItem!= null) Debug.LogError("持有的不为空！！");
        GameController.instance.curFruiItem = this;
    }
}
