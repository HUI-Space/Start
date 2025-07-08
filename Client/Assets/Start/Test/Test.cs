using System.Collections;
using System.Linq;
using Start;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Profiling;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Get("");
        await unityWebRequest.SendWebRequest();
        
        Debug.Log($"内存变化: {Profiler.GetTotalAllocatedMemoryLong()/1024}KB");
        var request  = AssetBundle.LoadFromFileAsync(
            "H:\\_File\\Unity\\Start\\Client\\AssetBundle\\1.1.5\\Android\\1745825531669\\assets_asset_audio_background_background_1.s");
        await request;
        var request2  = AssetBundle.LoadFromFileAsync(
            "H:\\_File\\Unity\\Start\\Client\\AssetBundle\\1.1.5\\Android\\1745825531669\\assets_asset_audio_background_background_2.s");
        await request2;
        var request3  = AssetBundle.LoadFromFileAsync(
            "H:\\_File\\Unity\\Start\\Client\\AssetBundle\\1.1.5\\Android\\1745825531669\\assets_asset_audio_ui_ui_1.s");
        await request3;
        Debug.Log($"内存变化: {Profiler.GetTotalAllocatedMemoryLong()/1024}KB");
        // 获取所有已加载的 AssetBundle
        var loadedBundles = AssetBundle.GetAllLoadedAssetBundles().ToList();

        long totalMemory = 0;
        
        foreach (var bundle in loadedBundles)
        {
            // 获取 AssetBundle 名称及内存占用（需要 Unity 2020.1+）
#if UNITY_2020_1_OR_NEWER
            var bundleMemory = UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(bundle);
#else
            var bundleMemory = (long)UnityEngine.Profiling.Profiler.GetRuntimeMemorySize(bundle);
#endif
            
            Debug.Log($"{bundle.name} 内存占用: {bundleMemory / 1024} KB");
            totalMemory += bundleMemory;
        }

        Debug.Log($"总 AssetBundle 内存: {totalMemory / 1024} KB");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log($"内存变化: {Profiler.GetTotalAllocatedMemoryLong()/1024}KB");
            AssetBundle.UnloadAllAssetBundles(true);
            Debug.Log($"内存变化: {Profiler.GetTotalAllocatedMemoryLong()/1024}KB");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(Post_Demo());
        }
    }
    
    string PostURL = "http://edu.xiaotu.cn/index/Unityapi/sendCourseInfo";

    
    public IEnumerator Post_Demo()
    {
        //Post请求的参数
        WWWForm form = new WWWForm();
        form.AddField("uid", "tea10000");
        form.AddField("course_id", "N700003");
        form.AddField("course_duration", "121212");
        form.AddField("course_score", "80");
        UnityWebRequest webRequest = UnityWebRequest.Post(PostURL, form);
        //发送请求
        yield return webRequest.SendWebRequest();
        if (string.IsNullOrEmpty(webRequest.error))
        {//Post的请求成功 //Post请求的返回参数
            var data = webRequest.downloadHandler.text;
              Debug.Log("成功:"+data);        }
        else
        {//Post的请求失败
            Debug.Log("失败");
        }
    }
}
public class UserInfo : Err
{
    public string uid;
    public string name;
    public int site_id;
}
public class Err
{
    public int err_no;
    public string err_msg;
}
