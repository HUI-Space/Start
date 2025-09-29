using System.Collections;
using System.Collections.Generic;
using System.IO;
using Start;
using UnityEngine;
using UnityEngine.Profiling;

public class Test6 : MonoBehaviour
{
    
    void Start()
    {
        Vector2 v = new Vector2(1, 2);
    }


    private bool isEnd = true;
    
   
    private void Update()
    {
        
    }
    
    private void LateUpdate()
    {
        if (isEnd)
        {
            Debug.Log($"Test6 LateUpdate 调用：{gameObject.name}，帧号：{Time.frameCount}");
            isEnd = false;
        }
    }
    
}
