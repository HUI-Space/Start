namespace Start
{
    public class PlayerEntity : IReusable
    {
        public int Id;
        public InputComponent Input = new InputComponent();
        public StateComponent State = new StateComponent();
        public static PlayerEntity Copy(PlayerEntity playerEntity)
        {
            PlayerEntity newPlayerEntity = RecyclableObjectPool.Acquire<PlayerEntity>();
            return newPlayerEntity;
        }
        
        public void Reset()
        {
            
        }
    }
}