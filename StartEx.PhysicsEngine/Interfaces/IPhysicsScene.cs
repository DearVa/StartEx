namespace StartEx.PhysicsEngine.Interfaces; 

public interface IPhysicsScene {
	void Register(PhysicsObject physicsObject);

	void Unregister(PhysicsObject physicsObject);
}