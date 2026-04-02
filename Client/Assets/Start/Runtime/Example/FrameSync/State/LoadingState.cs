using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Start
{
    public class LoadingState : AsyncFsmState<BattleManager>
    {
        protected override async Task OnEnter()
        {
            try
            {
                BattleData battleData = Fsm.GetData<BattleData>(nameof(BattleData));
                //1.打开Loading界面 
                await UIActions.ShowLoading();
                //关闭所有UI
                await UIActions.CloseAllUI();
                //2.加载场景
                AsyncOperationHandle<Scene> asyncOperationHandle = SceneManager.Instance.LoadSceneAsync<Scene>(battleData.SceneName, false);
                await asyncOperationHandle.Task;
                await UIActions.UpdateLoadingProgress(0.3f);
                //3.加载玩家
                List<PlayerController> playerList = new List<PlayerController>();
                for (int i = 0; i < battleData.PlayerData.Count; i++)
                {
                    PlayerData playerData = battleData.PlayerData[i];
                    AsyncOperationHandle<GameObject> handle = ResourceManager.Instance.LoadAsset<GameObject>(playerData.ModelPath);
                    await handle.Task;
                    GameObject player = GameObject.Instantiate(handle.Result);
                    PlayerController playerController = player.GetComponent<PlayerController>();
                    playerList.Add(playerController);
                }
            
                await UIActions.UpdateLoadingProgress(0.6f);
                
                //5.打开战斗界面
                await UIActions.OpenUI(nameof(BattlePanel));
                await UIActions.UpdateLoadingProgress(0.9f);
                //6.初始化战斗数据
                MatchEntity matchEntity = MatchController.Instance.SpawnMatchEntity();
                matchEntity.DeltaTime = new FP(battleData.FrameInterval) / new FP(1000);
                matchEntity.State.CurrentState = (int)EMatchState.Ready;
                for (int i = 0; i < battleData.PlayerData.Count; i++)
                {
                    PlayerEntity playerEntity = MatchController.Instance.SpawnPlayerEntity();
                    playerEntity.Id = battleData.PlayerData[i].Id;
                    playerEntity.State.CurrentState = (int)EPlayerState.Idle;
                    playerEntity.Move.Speed = new FP(5);
                    matchEntity.PlayerList.Add(playerEntity);
                }
                
                InputController.Instance.InitializeData(BattleConst.ReplayPath);
                MatchController.Instance.InitializeData(matchEntity);
                ReplayController.Instance.InitializeData();
                RenderController.Instance.InitializeData(playerList);
                
                await UIActions.UpdateLoadingProgress(1f);
                ChangeState<PlayingState>();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}