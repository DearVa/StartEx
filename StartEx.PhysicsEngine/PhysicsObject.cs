using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using Avalonia;
using ReactiveUI;
using StartEx.PhysicsEngine.DataTypes;
using StartEx.PhysicsEngine.Interfaces;

namespace StartEx.PhysicsEngine;

/// <summary>
/// 表现出物理特征
/// </summary>
public abstract class PhysicsObject : ReactiveObject {
	/// <summary>
	/// 实际的位置
	/// </summary>
	public Vector3 Position {
		get => positionProperty.Current;
		set => positionProperty.Current = value;
	}

	public Vector3 TargetPosition {
		get => positionProperty.Target;
		set => positionProperty.Target = value;
	}

	public double PositionElasticity {
		get => positionProperty.Elasticity;
		set => positionProperty.Elasticity = value;
	}

	public double PositionDamping {
		get => positionProperty.Damping;
		set => positionProperty.Damping = value;
	}

	private readonly PhysicsProperty positionProperty;

	/// <summary>
	/// 实际的大小
	/// </summary>
	public Vector3 Size {
		get => sizeProperty.Current;
		set => sizeProperty.Current = value;
	}

	public Vector3 TargetSize {
		get => sizeProperty.Target;
		set => sizeProperty.Target = value;
	}

	public double SizeElasticity {
		get => sizeProperty.Elasticity;
		set => sizeProperty.Elasticity = value;
	}

	public double SizeDamping {
		get => sizeProperty.Damping;
		set => sizeProperty.Damping = value;
	}

	private readonly PhysicsProperty sizeProperty;

	protected PhysicsObject() {
		positionProperty = new PhysicsProperty(PositionChangingHandler);
		sizeProperty = new PhysicsProperty(Vector3.One, Vector3.One, SizeChangingHandler);

		AvaloniaLocator.Current.GetRequiredService<IPhysicsScene>().Register(this);
	}

	~PhysicsObject() {
		AvaloniaLocator.Current.GetRequiredService<IPhysicsScene>().Unregister(this);
	}

	public void UpdatePhysics(TimeSpan deltaTime) {
		positionProperty.Update(deltaTime);
		sizeProperty.Update(deltaTime);
	}

	private void PositionChangingHandler(Vector3 oldValue, Vector3 newValue) {
		lock (positionChangingObservers) {
			foreach (var observer in positionChangingObservers) {
				observer.OnNext(new Vector3ChangingArgs(this, oldValue, newValue));
			}
		}
	}

	private void SizeChangingHandler(Vector3 oldValue, Vector3 newValue) {
		lock (sizeChangingObservers) {
			foreach (var observer in sizeChangingObservers) {
				observer.OnNext(new Vector3ChangingArgs(this, oldValue, newValue));
			}
		}
	}

	public record struct Vector3ChangingArgs(PhysicsObject Sender, Vector3 OldValue, Vector3 NewValue);

	private readonly List<IObserver<Vector3ChangingArgs>> positionChangingObservers = new();
	private readonly List<IObserver<Vector3ChangingArgs>> sizeChangingObservers = new();

	public IDisposable SubscribePositionChanging(IObserver<Vector3ChangingArgs> observer) {
		lock (positionChangingObservers) {
			positionChangingObservers.Add(observer);
		}

		return Disposable.Create(() => {
			lock (positionChangingObservers) {
				positionChangingObservers.Remove(observer);
			}
		});
	}

	public IDisposable SubscribeSizeChanging(IObserver<Vector3ChangingArgs> observer) {
		lock (sizeChangingObservers) {
			sizeChangingObservers.Add(observer);
		}

		return Disposable.Create(() => {
			lock (sizeChangingObservers) {
				sizeChangingObservers.Remove(observer);
			}
		});
	}
}