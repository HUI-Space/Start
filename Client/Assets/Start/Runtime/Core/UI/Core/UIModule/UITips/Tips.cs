using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Start
{
    public class Tips : MonoBehaviour
    {
        public Text MessageText;
        public Button StopButton;
        public CanvasGroup CanvasGroup;
        public UIEventHandler EventHandler;

        private CoroutineHandle _coroutineHandle;
        private readonly Queue<TipsData> _queue = new Queue<TipsData>();
        private void Awake()
        {
            StopButton.onClick.AddListener(() =>
            {
                if (_coroutineHandle.IsRunning)
                {
                    _coroutineHandle.Stop();
                }
            });
            CanvasGroup.Switch(false);
        }

        public void ShowMessage(string message,float durationTime,bool isThroughAll,int throughCount)
        {
            TipsData tipsData = TipsData.Create(message,durationTime,isThroughAll,throughCount);
            _queue.Enqueue(tipsData);
            if (!_coroutineHandle.IsRunning)
            {
                _coroutineHandle = CoroutineController.Run(ShowMessage(),true,OnCoroutineStop);
            }
        }
        
        private void OnCoroutineStop(bool isNotAuto)
        {
            if (_queue.Count > 0)
            {
                _coroutineHandle = CoroutineController.Run(ShowMessage(),true,OnCoroutineStop);
            }
            else
            {
                CanvasGroup.Switch(false);
            }
        }

        private IEnumerator ShowMessage()
        {
            TipsData tipsData = _queue.Dequeue();
            CanvasGroup.Switch(true);
            MessageText.text = tipsData.Message;
            if (tipsData.DurationTime != 0)
            {
                yield return new WaitForSeconds(tipsData.DurationTime);
            }
            EventHandler.IsThroughAllEvent = tipsData.IsThroughAll;
            EventHandler.ThroughEventCount = tipsData.ThroughCount;
            CanvasGroup.Switch(false);
            ReferencePool.Release(tipsData);
        }

        private void OnDestroy()
        {
            StopButton.onClick.RemoveAllListeners();
            foreach (var tipsData in _queue)
            {
                ReferencePool.Release(tipsData);
            }
            _queue.Clear();
        }
        
        private class TipsData : IReference
        {
            public string Message { get; private set; }
            public float DurationTime { get; private set; }
            public bool IsThroughAll { get; private set; }
            public int ThroughCount { get; private set; }

            public static TipsData Create(string message,float durationTime,bool isThroughAll,int throughCount)
            {
                TipsData tipsData = ReferencePool.Acquire<TipsData>();
                tipsData.Message = message;
                tipsData.DurationTime = durationTime;
                tipsData.IsThroughAll = isThroughAll;
                tipsData.ThroughCount = throughCount;
                return tipsData;
            }
            
            public void Clear()
            {
                Message = null;
                DurationTime = 0;
                IsThroughAll = false;
                ThroughCount = 0;
            }
        }
    }
}