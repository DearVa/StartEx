using System;
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
		get => position;
		set => this.RaiseAndSetIfChanged(ref position, value);
	}

	private Vector3 position;
	private readonly PhysicsVector3 positionVector3 = new();

	public Vector3 TargetPosition {
		get => positionVector3.Target;
		set => positionVector3.Target = value;
	}

	/// <summary>
	/// 实际的大小
	/// </summary>
	public Vector3 Size {
		get => size;
		set => this.RaiseAndSetIfChanged(ref size, value);
	}

	private Vector3 size = Vector3.One;
	private readonly PhysicsVector3 sizeVector3 = new(Vector3.One);

	public Vector3 TargetSize {
		get => sizeVector3.Target;
		set => sizeVector3.Target = value;
	}


	protected PhysicsObject() {
		AvaloniaLocator.Current.GetRequiredService<IPhysicsScene>().Register(this);
	}

	~PhysicsObject() {
		AvaloniaLocator.Current.GetRequiredService<IPhysicsScene>().Unregister(this);
	}

	public void UpdatePhysics(TimeSpan deltaTime) {
		if (!positionVector3.IsSleeping) {
			Position = positionVector3.Update(position, deltaTime);
		}

		if (!sizeVector3.IsSleeping) {
			Size = sizeVector3.Update(size, deltaTime);
		}
	}
}