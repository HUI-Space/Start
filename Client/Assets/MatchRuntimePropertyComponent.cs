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
            public int ID;
            public Common(int no)
            {
                ID = default;
            }
        }

        private Common _common = new Common();

        public int ID
        {
            get => _common.ID;
            set => _common.ID = value;
        }

        private AA _aa = new AA();
        public AA AA
        {
            get => _aa;
            set => _aa = value;
        }

        private Copy _copy = new Copy();
        public Copy Copy
        {
            get => _copy;
            set => _copy = value;
        }

        private int[] _ids = new int[6];
        public int[] IDs
        {
            get => _ids;
            set => _ids = value;
        }

        private List<int> _list = new List<int>();
        public List<int> List
        {
            get => _list;
            set => _list = value;
        }

        private Dictionary<int,FP> _dictionary = new Dictionary<int,FP>();
        public Dictionary<int,FP> Dictionary
        {
            get => _dictionary;
            set => _dictionary = value;
        }

        private List<Copy> _copylist = new List<Copy>();
        public List<Copy> CopyList
        {
            get => _copylist;
            set => _copylist = value;
        }

        private Dictionary<int,Copy> _copydictionary = new Dictionary<int,Copy>();
        public Dictionary<int,Copy> CopyDictionary
        {
            get => _copydictionary;
            set => _copydictionary = value;
        }


        public void CopyTo(MatchRuntimePropertyComponent component)
        {
            component._common = _common;
            component._aa = _aa;
            _copy.CopyTo(component._copy);
            Array.Copy(_ids, component._ids, _ids.Length);
            component._list.Clear();
            component._list.Capacity = _list.Count;
            component._list.AddRange(_list);

            component._dictionary.Clear();
            foreach (var item in _dictionary)
            {
                component._dictionary.Add(item.Key, item.Value);
            }
            component._copylist.Clear();
            component._copylist.Capacity = _copylist.Count;
            component._copylist.AddRange(_copylist);

            component._copydictionary.Clear();
            foreach (var item in _copydictionary)
            {
                component._copydictionary.Add(item.Key, item.Value);
            }
        }
    }
}
