using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Test6 : MonoBehaviour
{
    
    
    private Stopwatch _stopWatch1 = new Stopwatch();
    private Stopwatch _stopWatch2 = new Stopwatch();

    private Dictionary<int, int> _testDict = new Dictionary<int, int>();
    private Dictionary<int, int> _testDict2 = new Dictionary<int, int>();
    private Dictionary<int, int> _testDict3 = new Dictionary<int, int>();
    void Start()
    {
        for (int i = 0; i < 10000000; i++)
        {
            _testDict.Add(i, i);
        }
        
        _stopWatch2.Start();
        using (var e = _testDict.GetEnumerator())
        {
            while (e.MoveNext())
            {               
                _testDict3.Add(e.Current.Key, e.Current.Value);
            }
        }
        _stopWatch2.Stop();
        Debug.Log("2:" + _stopWatch2.ElapsedMilliseconds);
        
        _stopWatch1.Start();
        foreach (var item in _testDict)
        {
            _testDict2.Add(item.Key, item.Value);
        }
        _stopWatch1.Stop();
        Debug.Log("1:" + _stopWatch1.ElapsedMilliseconds);
    }
}