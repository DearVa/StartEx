using System;
using System.Runtime.CompilerServices;
using ReactiveUI;
using StartEx.Core.Interfaces;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using StartEx.Core.Extensions;

namespace StartEx.Core.ViewModels; 

public abstract class ViewModelBase : ReactiveObject, IActivatableViewModel {
	public ViewModelActivator Activator { get; } = new();

	protected ViewModelBase() {
		this.WhenActivated(OnActivatedInternal);
	}

	private void OnActivatedInternal(CompositeDisposable disposables) {
		OnActivated(disposables);
		OnActivatedAsync(disposables).Detach();
	}

	protected virtual void OnActivated(CompositeDisposable disposables) { }

	protected virtual Task OnActivatedAsync(CompositeDisposable disposables) => Task.CompletedTask;

	protected bool RaiseAndSetIfChanged<T>(ref T field, T value, [CallerMemberName] string? propertyName = null) {
		if (EqualityComparer<T>.Default.Equals(field, value)) {
			return false;
		}

		this.RaisePropertyChanging(propertyName);
		field = value;
		this.RaisePropertyChanged(propertyName);
		return true;
	}
}

public class BusyViewModelBase : ViewModelBase {
	public bool IsBusy {
		get => isBusy;
		set => RaiseAndSetIfChanged(ref isBusy, value);
	}

	private bool isBusy;
}

public class StateMachineViewModelBase<TState> : ViewModelBase, IStateMachine<TState>
	where TState : struct, Enum {

	public TState Current {
		get => current;
		set {
			var oldState = current;
			if (RaiseAndSetIfChanged(ref current, value)) {
				StateChanged?.Invoke(oldState, value);
			}
		}
	}

	protected TState current;

	public event IStateMachine<TState>.StateChangeHandler? StateChanged;
}