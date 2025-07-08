namespace Start
{
    public class TALSettingModule
    {
        // 小人跳跃时，决定远近的一个参数
        public float Factor = 5;

        // 盒子随机最远的距离
        public float MaxDistance = 5;

        public bool OpenAudio;
        
        public void SetGameMode(bool normal)
        {
            if (normal)
            {
                Factor = 2;
                MaxDistance = 2;
            }
            else
            {
                Factor = 5;
                MaxDistance = 5;
            }
        }
        public void SetAudio(bool audio)
        {
            OpenAudio = audio;
        }
        
        
    }
}