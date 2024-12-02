using Start.Framework;
using UnityEngine;
using Log = Start.Framework.Log;

namespace Start.Runtime
{
    public class Main : MonoBehaviour
    {
        public static Transform Root;
        private GameController _gameController;
        private async void Start()
        {
            if (Root == null)
            {
                Root = transform;
            }
            DontDestroyOnLoad(this);
            await Manager.Initialize();
            SetHelper();
            _gameController = new GameController();
            _gameController.Initialize();
            await ProcedureManager.Instance.StartProcedure<ProcedureCheckGameVersion>();
        }
    
        /// <summary>
        /// 该接口的主要作用是传入对应对象的接口到对应的Manager中
        /// </summary>
        private void SetHelper()
        {
            Log.SetLog(new LogHelper());
            UIManager.SetHelper(new UIHelper());
            ConfigManager.SetHelper(new ConfigHelper());
            DownloadManager.SetHelper(new DownloadHelper());
            ResourceManager.SetHelper(new ResourceHelper());
            HttpManager.SetHelper(new HttpHelper());
            AudioManager.SetHelper(new AudioHelper());
        }
    
        void Update()
        {
            Manager.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }
        private async void OnDestroy()
        {
            await Manager.DeInitialize();
            ReferencePool.ClearAll();
            _gameController.DeInitialize();
            _gameController = null;
            Root = null;
        }
    }
}


