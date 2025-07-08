using UnityEngine;

namespace Start
{
    public partial class Main : MonoBehaviour
    {
        public static Transform Root;

        private void Awake()
        {
            if (Root == null)
            {
                Root = transform;
            }
            DontDestroyOnLoad(this);
        }
        
        private async void Start()
        {
            await Manager.Initialize();
            await ProcedureManager.Instance.StartProcedure<ProcedureMain>();
        }
        
        void Update()
        {
            Manager.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }
        
        private async void OnDestroy()
        {
            await Manager.DeInitialize();
            ReferencePool.ClearAll();
            Root = null;
        }
    }
}