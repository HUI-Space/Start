using UnityEngine;

namespace Start.Editor
{
    public class TestBatch
    {
        public static void Run()
        {
            for (int i = 0; i < 100; i++)
            {
                Debug.Log("Run: " + i);
            }
        }
    }
}