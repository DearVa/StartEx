using System;

namespace StartEx.PhysicsEngine.DataTypes; 

/// <summary>
/// 包含了一组物理量
/// </summary>
internal class PhysicsProperty {
	private readonly Action<Vector3, Vector3> currentChanging;

	public Vector3 Current {
		get => current;
		set {
			if (current == value) {
				return;
			}

			currentChanging.Invoke(current, value);

			current = value;
			IsSleeping = false;
		}
	}

	private Vector3 current;

	public Vector3 Target {
		get => target;
		set {
			if (target == value) {
				return;
			}

			target = value;
			IsSleeping = false;
		}
	}

	private Vector3 target;

	public Vector3 Velocity { get; set; }

	public Vector3 Acceleration { get; private set; }

	/// <summary>
	/// 弹性。有多快的加速度想回到<see cref="Target"/>
	/// </summary>
	public double Elasticity { get; set; } = 500d;

	/// <summary>
	/// 阻尼。
	/// </summary>
	public double Damping { get; set; } = 25d;

	public bool IsSleeping { get; private set; }

	public PhysicsProperty(Action<Vector3, Vector3> currentChanging) {
		this.currentChanging = currentChanging;
	}

	public PhysicsProperty(Vector3 current, Vector3 target, Action<Vector3, Vector3> currentChanging)
		: this(currentChanging) {
		Current = current;
		Target = target;
	}

	public void Update(TimeSpan deltaTime) {
		if (IsSleeping) {
			return;
		}

		var ds = deltaTime.TotalSeconds;

		Acceleration = (Target - current) * Elasticity - Velocity * Damping;
		if (Acceleration.SqrMagnitude < Vector3.Epsilon) {
			IsSleeping = true;
			return;
		}

		Velocity += Acceleration * ds;
		Current = current + Velocity * ds;
	}
}