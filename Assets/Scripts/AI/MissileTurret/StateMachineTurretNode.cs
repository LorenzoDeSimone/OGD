using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProcessState { Inactive, Aim, Stopped }

public enum Command { Find, End, Exit }

public class TurretStateMachine
{
    class StateTransition
    {
        readonly ProcessState CurrentState;
        readonly Command Command;

        public StateTransition(ProcessState currentState, Command command)
        {
            CurrentState = currentState;
            Command = command;
        }

        public override int GetHashCode()
        {
            return 17 + 31 * CurrentState.GetHashCode() + 31 * Command.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            StateTransition other = obj as StateTransition;
            return other != null && this.CurrentState == other.CurrentState && this.Command == other.Command;
        }
    }

    Dictionary<StateTransition, ProcessState> transitions;
    public ProcessState CurrentState { get; private set; }

    public TurretStateMachine()
    {
        CurrentState = ProcessState.Inactive;
        transitions = new Dictionary<StateTransition, ProcessState>
            {
                { new StateTransition(ProcessState.Inactive, Command.Find), ProcessState.Aim },
                { new StateTransition(ProcessState.Aim, Command.Exit), ProcessState.Stopped },
                { new StateTransition(ProcessState.Stopped, Command.Exit), ProcessState.Inactive }
            };
    }

    public ProcessState GetNext(Command command)
    {
        StateTransition transition = new StateTransition(CurrentState, command);
        ProcessState nextState;
        if (!transitions.TryGetValue(transition, out nextState))
            throw new Exception("Invalid transition: " + CurrentState + " -> " + command);
        return nextState;
    }

    public ProcessState MoveNext(Command command)
    {
        CurrentState = GetNext(command);
        return CurrentState;
    }
}