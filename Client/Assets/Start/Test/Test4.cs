using System;
using UnityEngine;

namespace Start.Test
{
    public class Test4 : MonoBehaviour
    {
        private async void Start()
        {
            try
            {
                var a = AssetBundle.LoadFromFileAsync("");
                await a;
                print("111");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}