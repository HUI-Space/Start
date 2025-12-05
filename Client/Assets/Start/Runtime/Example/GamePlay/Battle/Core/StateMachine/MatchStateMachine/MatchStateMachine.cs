using System;
using System.Collections.Generic;

namespace Start
{
    public class MatchStateMachine 
    {
        private readonly MatchStateBase[] _states;
        
        public MatchStateMachine()
        {
            List<Type> types = AssemblyUtility.GetChildType(typeof(MatchStateBase));
            _states = new MatchStateBase[(int)EMatchState.Count];
            for (int i = 0; i < types.Count; i++)
            {
                var singleType = types[i];
                if (singleType.IsDefined(typeof(MatchStateAttribute), false))
                {
                    var attribute = (MatchStateAttribute)singleType.GetCustomAttributes(false)[0];
                    var state = Activator.CreateInstance(singleType) as MatchStateBase;
                    _states[(int)attribute.MatchState] = state;
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

        public virtual MatchStateBase GetState(int stateId)
        {
            return _states[stateId];
        }

        public virtual void Update(MatchEntity matchEntity)
        {
            MatchStateBase currentState = _states[matchEntity.State.CurrentState];
            MatchStateBase nextState = _states[matchEntity.State.NextState];
            if (CanChangeState(matchEntity) && nextState.TryEnter(matchEntity))
            {
                currentState.OnExit(matchEntity);
                matchEntity.State.CurrentState = matchEntity.State.NextState;
                matchEntity.State.NextState = 0;
                nextState.OnEnter(matchEntity);
                currentState = nextState;
            }

            if (currentState != null)
            {
                currentState.OnUpdate(matchEntity);
            }
        }

        private bool CanChangeState(MatchEntity matchEntity)
        {
            if (matchEntity.State.NextState != 0 && matchEntity.State.NextState != matchEntity.State.CurrentState)
            {
                return true;
            }
            return false;
        }
        
    }
}