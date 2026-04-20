/*-----------------------------------------------------------------
    以下代码为自动生成的代码，任何对以下代码的修改将在下次自动生成后被覆
    盖，不要对以下代码进行修改
-----------------------------------------------------------------*/

using System;
using System.Text;
using System.Collections.Generic;
namespace Start
{
    public class MatchPropertyComponent : IComponent<MatchPropertyComponent>
    {
        /// <summary>
        /// 属性
        /// </summary>
        public FP Property
        {
            get => _common.Property;
            set => _common.Property = value;
        }

        /// <summary>
        /// 属性索引
        /// </summary>
        private Int32[] array { get; set; }

        /// <summary>
        /// 属性值
        /// </summary>
        public List<FP> Value = new List<FP>();

        /// <summary>
        /// 属性索引
        /// </summary>
        public Dictionary<Int32, FP> Index = new Dictionary<Int32, FP>();

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Auto, Pack = 4)]
        private struct Common
        {
            public FP Property;

            public Common(int no)
            {
                Property = default;
            }
        }

        private Common _common = new Common();

        public void CopyTo(MatchPropertyComponent component)
        {
            component._common = _common;
            if (array != null)
            {
                Array.Copy(array, component.array, array.Length);
            }
            component.Value.Clear();
            component.Value.AddRange(Value);

            foreach (KeyValuePair<Int32,FP> item in Index)
            {
                component.Index.Add(item.Key, item.Value);
            }

        }

        public void SerializeToLog(StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine($"[MatchPropertyComponent]");
            stringBuilder.AppendLine($"Property : {Property}");
            if (array != null)
            {
                stringBuilder.AppendLine($"array (Count: {array.Length})");
                for (int i = 0; i < array.Length; i++)
                {
                    stringBuilder.AppendLine($"   [{i}] : {array[i]}");
                }
            }
            stringBuilder.AppendLine($"Value (Count : {Value.Count})");
            for (int i = 0; i < Value.Count; i++)
            {
               stringBuilder.AppendLine($"   [{i}] : {Value[i]}");
            }
            stringBuilder.AppendLine($"Index (Count : {Index.Count})");
            foreach (var item in Index)
            {
               stringBuilder.AppendLine($"   [{item.Key}] : {item.Value}");
            }
        }
    }
}
