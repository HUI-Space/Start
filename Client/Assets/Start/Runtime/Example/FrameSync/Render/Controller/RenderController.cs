using System.Collections.Generic;
using UnityEngine;

namespace Start
{
    public class RenderController : SingletonBase<RenderController> 
    {
        /// <summary>
        /// 渲染实体
        /// </summary>
        public MatchEntity RenderMatchEntity { get; private set; }
        
        private List<PlayerController> _playerList = new List<PlayerController>();

        public void InitializeData(List<PlayerController> playerController)
        {
            RenderMatchEntity = MatchController.Instance.SpawnMatchEntity();
            _playerList.Clear();
            _playerList.AddRange(playerController);
        }
        
        public void DeInitializeData()
        {
            MatchController.Instance.UnSpawnMatchEntity(RenderMatchEntity);
            foreach (PlayerController playerController in _playerList)
            {
                GameObject.Destroy(playerController.gameObject);
            }
            RenderMatchEntity = null;
        }
        
        /// <summary>
        /// 逻辑更新
        /// </summary>
        /// <param name="matchEntity">实体</param>
        public void LogicUpdate(MatchEntity matchEntity)
        {
            //拷贝到渲染实体
            matchEntity.CopyTo(RenderMatchEntity);
        }
        
        /// <summary>
        /// 渲染更新
        /// </summary>
        public void RenderUpdate()
        {
            if (RenderMatchEntity == null)
            {
                return;
            }
            for (int i = 0; i < RenderMatchEntity.PlayerList.Count; i++)
            {
                PlayerController playerController = _playerList[i];
                playerController.RenderUpdate(RenderMatchEntity,RenderMatchEntity.PlayerList[i]);
            }
        }
    }
}