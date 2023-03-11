using System;

namespace StartEx.PhysicsEngine.DataTypes; 

/// <summary>
/// 包含了一组物理量
/// </summary>
public class PhysicsVector3 {
	public Vector3 Target {
		get => target;
		set {
			if ((target - value).SqrMagnitude <= Vector3.Epsilon) {
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
	public double Elasticity { get; set; } = 120d;

	/// <summary>
	/// 阻尼。
	/// </summary>
	public double Damping { get; set; } = 15d;

	public bool IsSleeping { get; private set; }

	public PhysicsVector3() { }

	public PhysicsVector3(Vector3 target) {
		Target = target;
	}

	public Vector3 Update(Vector3 current, TimeSpan deltaTime) {
		var ds = deltaTime.TotalSeconds;

		Acceleration = (Target - current) * Elasticity - Velocity * Damping;
		if (Acceleration.SqrMagnitude < Vector3.Epsilon) {
			IsSleeping = true;
			return Target;
		}

		Velocity += Acceleration * ds;
		return current + Velocity * ds;
	}
}