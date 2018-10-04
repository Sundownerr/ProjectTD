
public interface IState
{
    void Enter();
    void Execute();
    void Exit();
}

public class StateMachine
{
    IState currentState;

    public void ChangeState(IState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter();
    }

    public void Update()
    {
        if (currentState != null)
        {
            currentState.Execute();
        }
    }
}

