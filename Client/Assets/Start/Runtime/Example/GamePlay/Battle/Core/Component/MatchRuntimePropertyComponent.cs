

namespace Start
{
    public class MatchRuntimePropertyComponent : IComponent<MatchRuntimePropertyComponent>
    {
        
        
        public static MatchRuntimePropertyComponent Create()
        {
            MatchRuntimePropertyComponent component = ReferencePool.Acquire<MatchRuntimePropertyComponent>();
            return component;
        }
        
        public void CopyTo(MatchRuntimePropertyComponent component)
        {
            
        }

        public void Clear()
        {
            
        }
    }
}