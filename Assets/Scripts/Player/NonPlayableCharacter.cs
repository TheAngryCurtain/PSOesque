using UnityEngine;

public class NonPlayableCharacter : Character
{
    public abstract class State
    {
        public abstract void Enter(Character c);
        public abstract State Update(Character c);
        public abstract void Exit(Character c);
    }

    protected State m_PreviousState;
    protected State m_CurrentState;

    protected virtual void Update()
    {
        if (m_CurrentState != null)
        {
            if (m_PreviousState != null && m_CurrentState != m_PreviousState)
            {
                m_PreviousState.Exit(this);
                m_CurrentState.Enter(this);
            }

            m_PreviousState = m_CurrentState;
            m_CurrentState = m_CurrentState.Update(this);
        }
    }
}
