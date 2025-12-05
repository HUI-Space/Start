using System;
using System.Collections.Generic;

namespace Start
{
    public class PlayerStateMachine : StateMachine<PlayerEntity>
    {
        public PlayerStateMachine()
        {
            List<Type> types = AssemblyUtility.GetChildType(typeof(PlayerStateBase));
            _states = new StateBase<PlayerEntity>[(int)EPlayerState.Count];
            for (int i = 0; i < types.Count; i++)
            {
                var singleType = types[i];
                if (singleType.IsDefined(typeof(PlayerStateAttribute), false))
                {
                    var attribute = (PlayerStateAttribute)singleType.GetCustomAttributes(false)[0];
                    var state = Activator.CreateInstance(singleType) as StateBase<PlayerEntity>;
                    _states[(int)attribute.PlayerState] = state;
                }
#if UNITY_EDITOR
                var fields = singleType.GetFields();
                if (fields.Length > 0)
                {
                    UnityEngine.Debug.LogErrorFormat("State:{0} has filed!", singleType);
                }

                var properties = singleType.GetProperties();
                if (properties.Length > 1)
                {
                    UnityEngine.Debug.LogErrorFormat("State:{0} has property!", singleType);
                }
#endif
            }
        }
        

        public override void OnUpdate(MatchEntity matchEntity, PlayerEntity playerEntity)
        {
            var currentState = _states[playerEntity.State.CurrentState];
            if (currentState != null)
            {
                currentState.OnUpdate(matchEntity, playerEntity);
            }
        }
        
        public override void OnLateUpdate(MatchEntity matchEntity, PlayerEntity playerEntity)
        {
            var currentState = _states[playerEntity.State.CurrentState];
            if (currentState != null)
            {
                currentState.OnUpdate(matchEntity, playerEntity);
            }
        }
        
        public void ChangeState(MatchEntity matchEntity, PlayerEntity playerEntity)
        {
            var nextState = GetState(playerEntity.State.NextState);
            if (CanChangeState(playerEntity) && nextState !=null && nextState.TryEnter(matchEntity, playerEntity))
            {
                var currentState = GetState(playerEntity.State.CurrentState);
                currentState.OnExit(matchEntity, playerEntity);
                playerEntity.State.CurrentState = playerEntity.State.NextState;
                playerEntity.State.NextState = 0;
                nextState.OnEnter(matchEntity, playerEntity);
            }
        }

        public void ForceChangeState(MatchEntity matchEntity, PlayerEntity playerEntity)
        {
            var nextState = GetState(playerEntity.State.ForceState);
            if (CanChangeState(playerEntity) && nextState != null && nextState.TryEnter(matchEntity, playerEntity))
            {
                var currentState = GetState(playerEntity.State.CurrentState);
                currentState.OnExit(matchEntity, playerEntity);
                playerEntity.State.CurrentState = playerEntity.State.ForceState;
                playerEntity.State.ForceState = 0;
                nextState.OnEnter(matchEntity, playerEntity);
            }
        }
        
        private bool CanChangeState(PlayerEntity playerEntity)
        {
            if (playerEntity.State.NextState != 0 && playerEntity.State.NextState != playerEntity.State.CurrentState)
            {
                return true;
            }
            return false;
        }
    }
}