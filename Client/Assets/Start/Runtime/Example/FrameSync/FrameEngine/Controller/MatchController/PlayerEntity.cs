namespace Start
{
    public class PlayerEntity : IObject
    {
        public int Id { get; set; } = -1;

        public MoveComponent Move { get; private set; } = new MoveComponent();

        public InputComponent Input { get; private set; } = new InputComponent();

        public StateComponent State { get; private set; } = new StateComponent();

        public TransformComponent Transform { get; private set; } = new TransformComponent();
        
        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="playerEntity">复制完成之后的对象</param>
        public void CopyTo(PlayerEntity playerEntity)
        {
            playerEntity.Id = Id;
            Input.CopyTo(playerEntity.Input);
            State.CopyTo(playerEntity.State);
            Move.CopyTo(playerEntity.Move);
            Transform.CopyTo(playerEntity.Transform);
        }

        public void OnRelease()
        {
            Id = 0;
            Move = null;
            Input = null;
            State = null;
            Transform = null;
        }
    }
}