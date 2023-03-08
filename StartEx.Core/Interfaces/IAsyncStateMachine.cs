using System;
using System.Threading.Tasks;

namespace StartEx.Core.Interfaces;

public interface IAsyncStateMachine<TState> where TState : Enum {
	TState Current { get; }

	public delegate ValueTask StateChangeHandler(TState oldState, TState newState);

	event StateChangeHandler? StateChangedAsync;

	ValueTask SetCurrentStateAsync(TState newState);
}