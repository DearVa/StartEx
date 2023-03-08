using System;

namespace StartEx.Core.Interfaces; 

public interface IStateMachine<TState> where TState : struct, Enum {
	TState Current { get; }

	public delegate void StateChangeHandler(TState oldState, TState newState);

	event StateChangeHandler? StateChanged;
}