using System;
using System.Collections.Generic;
using Avalonia.Threading;
using StartEx.PhysicsEngine.Interfaces;

namespace StartEx.PhysicsEngine;

public class PhysicsScene : IPhysicsScene {
	private readonly HashSet<PhysicsObject> physicsObjects = new();
	private readonly TimeSpan period = TimeSpan.FromMilliseconds(20);
	private readonly IDisposable timer;

	public PhysicsScene() {
		timer = DispatcherTimer.Run(TimerCallback, period);
	}

	private bool TimerCallback() {
		lock (timer) {
			foreach (var physicsObject in physicsObjects) {
				physicsObject.UpdatePhysics(period);
			}
		}

		return true;
	}

	public void Register(PhysicsObject physicsObject) {
		lock (timer) {
			physicsObjects.Add(physicsObject);
		}
	}

	public void Unregister(PhysicsObject physicsObject) {
		lock (timer) {
			physicsObjects.Remove(physicsObject);
		}
	}
}