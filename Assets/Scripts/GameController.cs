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


    void Awake()
    {
        SpwanIce();
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
}
