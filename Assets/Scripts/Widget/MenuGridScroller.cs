﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MenuGridScroller : MonoBehaviour
{
    // 网格
    public GridLayoutGroup grid;

    public GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 添加新项
    public void AddButton(Sprite image, UnityAction listener) {
        // 创建新的子节点
        GameObject prefabInstance = Instantiate(prefab);
        prefabInstance.transform.SetParent(grid.transform);
        prefabInstance.transform.localScale=new Vector3Int(1,1,1);

        // 设置参数
        prefabInstance.GetComponent<Image>().sprite = image;
        prefabInstance.GetComponent<Button>().onClick.AddListener(listener);
    }
}
