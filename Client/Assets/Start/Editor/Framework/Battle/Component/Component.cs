using System.Collections.Generic;

namespace Start.Editor
{
    public class Component
    {
        public class MatchRuntimePropertyComponent : IComponent
        {
            [Comment("ID")] 
            public int Id;

            [Comment("ID列表")] 
            public int[] Ids;

            [Comment("子ID列表")] 
            public List<int> ChildIds;
        }
    }
}