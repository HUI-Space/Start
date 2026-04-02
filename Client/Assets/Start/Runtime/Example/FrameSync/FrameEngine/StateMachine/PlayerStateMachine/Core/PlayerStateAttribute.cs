namespace Start
{
    public class PlayerStateAttribute: System.Attribute
    {
        public EPlayerState PlayerState;
        
        public PlayerStateAttribute(EPlayerState playerState)
        {
            PlayerState = playerState;
        }
    }
}