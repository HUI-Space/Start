namespace Start
{
    public class MatchStateAttribute : System.Attribute
    {
        public EMatchState MatchState;
        
        public MatchStateAttribute(EMatchState matchState)
        {
            MatchState = matchState;
        }
    }
}