using System.Collections;
using System.Collections.Generic;
using System.IO;
using Start;
using UnityEngine;

public class Test5 : MonoBehaviour
{
    
    async void Start()
    {
        /*Manifest manifest = SerializerUtility.DeserializeObject<Manifest>("H:/_File/Unity/Start/Client/Assets/StreamingAssets/AssetBundle/LocalBuiltInManifest.b");
        string content = UnityUtility.ToJson(manifest);
        FileUtility.WriteAllText("H:/_File/Unity/Start/Client/Assets/StreamingAssets/AssetBundle/LocalBuiltInManifest.json",content);*/
        
        //string resourcePath = "H:\\_File\\Unity\\Start\\Client\\AssetBundle\\1.1.5\\Android\\BuiltIn\\LocalBuiltInResource.s";
        string resourcePath = "H:/_File/Unity/Start/Client/Assets/StreamingAssets/AssetBundle/LocalBuiltInResource.s";
        uint crc = 645040835;
        ulong offset = 198893;
        AssetBundleCreateRequest assetBundleCreateRequest =  AssetBundle.LoadFromFileAsync(resourcePath, crc, offset);
        await assetBundleCreateRequest;
        string prefab = "Assets/Asset/UI/ConfigPanel.prefab";
        GameObject go = assetBundleCreateRequest.assetBundle.LoadAsset<GameObject>(prefab);
        Instantiate(go, this.transform);
    }

    
}
