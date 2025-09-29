using System;
using System.Collections.Generic;

namespace Start
{
    /// <summary>
    /// 动态生成的组件类: 比赛运行是属性组件
    /// </summary>
    public class MatchRuntimePropertyComponent : IComponent<MatchRuntimePropertyComponent>
    {
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Auto, Pack = 4)]
        private struct Common
        {
            public Common(int no)
            {
            }
        }

        private Common _common = new Common();
        public void CopyTo(MatchRuntimePropertyComponent component)
        {
        }
        public void Clear()
        {
        }
    }
}
