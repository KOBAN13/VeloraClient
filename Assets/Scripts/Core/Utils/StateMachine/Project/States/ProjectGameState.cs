using Core.Utils.StateMachine.Abstract.States;
using UnityEngine;

namespace Core.Utils.StateMachine.Project.States
{
    public sealed class ProjectGameState : IState
    {
        public void Enter()
        {
            Debug.LogError("Enter GameState");
        }

        public void Exit()
        {
        }
    }
}