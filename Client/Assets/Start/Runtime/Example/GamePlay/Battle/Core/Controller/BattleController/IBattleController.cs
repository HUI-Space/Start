namespace Start
{
    public interface IBattleController
    {
        bool Paused { get; }
        
        void LogicUpdate();
        
        void RenderUpdate();

        void Pause();
        
        void Resume();
    }
}