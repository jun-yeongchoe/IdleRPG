public interface IEnemyState
{
    void Enter();
    void Update();
    void Exit();

    bool IsTerminal { get; }
}