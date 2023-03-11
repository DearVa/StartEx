using System;

namespace StartEx.PhysicsEngine.DataTypes;

// Representation of 3D vectors and points.
public struct Vector3 : IEquatable<Vector3> {
	public const double Epsilon = 1e-5d;
	public const double EpsilonNormalSqrt = 1e-15d;

	// X component of the vector.
	public double X { get; set; }

	// Y component of the vector.
	public double Y { get; set; }

	// Z component of the vector.
	public double Z { get; set; }

	// Linearly interpolates between two vectors.
	public static Vector3 Lerp(Vector3 a, Vector3 b, double t) {
		t = Math.Clamp(t, 0, 1);
		return new Vector3(
			a.X + (b.X - a.X) * t,
			a.Y + (b.Y - a.Y) * t,
			a.Z + (b.Z - a.Z) * t
		);
	}

	// Linearly interpolates between two vectors without clamping the interpolant
	public static Vector3 LerpUnclamped(Vector3 a, Vector3 b, double t) {
		return new Vector3(
			a.X + (b.X - a.X) * t,
			a.Y + (b.Y - a.Y) * t,
			a.Z + (b.Z - a.Z) * t
		);
	}

	// Moves a point /current/ in a straight line towards a /target/ point.
	public static Vector3 MoveTowards(Vector3 current, Vector3 target, double maxDistanceDelta) {
		var toVector = target - current;
		var dist = toVector.Magnitude;
		if (dist <= maxDistanceDelta || dist < double.Epsilon) {
			return target;
		}
		return current + toVector / dist * maxDistanceDelta;
	}

	// Access the X, Y, Z components using [0], [1], [2] respectively.
	public double this[int index] {
		get {
			return index switch {
				0 => X,
				1 => Y,
				2 => Z,
				_ => throw new IndexOutOfRangeException("Invalid Vector3 index!")
			};
		}

		set {
			switch (index) {
				case 0:
					X = value;
					break;
				case 1:
					Y = value;
					break;
				case 2:
					Z = value;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid Vector3 index!");
			}
		}
	}

	// Creates a new vector with given X, Y, Z components.
	public Vector3(double x, double y, double z) {
		X = x;
		Y = y;
		Z = z;
	}

	// Creates a new vector with given X, Y components and sets /Z/ to zero.
	public Vector3(double x, double y) {
		X = x;
		Y = y;
		Z = 0F;
	}

	// Set X, Y and Z components of an existing Vector3.
	public void Set(double newX, double newY, double newZ) {
		X = newX;
		Y = newY;
		Z = newZ;
	}

	// Multiplies two vectors component-wise.
	public static Vector3 Scale(Vector3 a, Vector3 b) {
		return new Vector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
	}

	// Multiplies every component of this vector by the same component of /scale/.
	public void Scale(Vector3 scale) {
		X *= scale.X;
		Y *= scale.Y;
		Z *= scale.Z;
	}

	// Cross Product of two vectors.
	public static Vector3 Cross(Vector3 lhs, Vector3 rhs) {
		return new Vector3(
			lhs.Y * rhs.Z - lhs.Z * rhs.Y,
			lhs.Z * rhs.X - lhs.X * rhs.Z,
			lhs.X * rhs.Y - lhs.Y * rhs.X);
	}

	// used to allow Vector3s to be used as keys in hash tables
	public override int GetHashCode() {
		return X.GetHashCode() ^ (Y.GetHashCode() << 2) ^ (Z.GetHashCode() >> 2);
	}

	// also required for being able to use Vector3s as keys in hash tables
	public override bool Equals(object? other) {
		return other is Vector3 vector3 && Equals(vector3);
	}

	public bool Equals(Vector3 other) {
		return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
	}

	// Reflects a vector off the plane defined by a normal.
	public static Vector3 Reflect(Vector3 inDirection, Vector3 inNormal) {
		return -2F * Dot(inNormal, inDirection) * inNormal + inDirection;
	}

	// *undoc* --- we have normalized property now
	public static Vector3 Normalize(Vector3 value) {
		var mag = value.Magnitude;
		if (mag > Epsilon) {
			return value / mag;
		}
		return Zero;
	}

	// Makes this vector have a ::ref::Magnitude of 1.
	public void Normalize() {
		var mag = Magnitude;
		if (mag > Epsilon) {
			this /= mag;
		} else {
			this = Zero;
		}
	}

	// Returns this vector with a ::ref::Magnitude of 1 (RO).
	public Vector3 normalized => Normalize(this);

	// Dot Product of two vectors.
	public static double Dot(Vector3 lhs, Vector3 rhs) {
		return lhs.X * rhs.X + lhs.Y * rhs.Y + lhs.Z * rhs.Z;
	}

	// Projects a vector onto another vector.
	public static Vector3 Project(Vector3 vector, Vector3 onNormal) {
		var sqrMag = Dot(onNormal, onNormal);
		if (sqrMag < Epsilon) {
			return Zero;
		}
		return onNormal * Dot(vector, onNormal) / sqrMag;
	}

	// Projects a vector onto a plane defined by a normal orthogonal to the plane.
	public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal) {
		return vector - Project(vector, planeNormal);
	}

	// Returns the angle in degrees between /from/ and /to/. This is always the smallest
	public static double Angle(Vector3 from, Vector3 to) {
		// sqrt(a) * sqrt(b) = sqrt(a * b) -- valid for real numbers
		var denominator = Math.Sqrt(from.SqrMagnitude * to.SqrMagnitude);
		if (denominator < EpsilonNormalSqrt) {
			return 0F;
		}

		var dot = Math.Clamp(Dot(from, to) / denominator, -1F, 1F);
		return Math.Acos(dot) * 180d / Math.PI;
	}

	// The smaller of the two possible angles between the two vectors is returned, therefore the result will never be greater than 180 degrees or smaller than -180 degrees.
	// If you imagine the from and to vectors as lines on a piece of paper, both originating from the same point, then the /axis/ vector would point up out of the paper.
	// The measured angle between the two vectors would be positive in a clockwise direction and negative in an anti-clockwise direction.
	public static double SignedAngle(Vector3 from, Vector3 to, Vector3 axis) {
		var unsignedAngle = Angle(from, to);
		double sign = Math.Sign(Dot(axis, Cross(from, to)));
		return unsignedAngle * sign;
	}

	// Returns the distance between /a/ and /b/.
	public static double Distance(Vector3 a, Vector3 b) {
		var vec = new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		return Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z);
	}

	// Returns a copy of /vector/ with its Magnitude clamped to /maxLength/.
	public static Vector3 ClampMagnitude(Vector3 vector, double maxLength) {
		if (vector.SqrMagnitude > maxLength * maxLength) {
			return vector.normalized * maxLength;
		}
		return vector;
	}

	// Returns the length of this vector (RO).
	public double Magnitude => Math.Sqrt(X * X + Y * Y + Z * Z);

	// Returns the squared length of this vector (RO).
	public double SqrMagnitude => X * X + Y * Y + Z * Z;

	// Returns a vector that is made from the smallest components of two vectors.
	public static Vector3 Min(Vector3 lhs, Vector3 rhs) {
		return new Vector3(Math.Min(lhs.X, rhs.X), Math.Min(lhs.Y, rhs.Y), Math.Min(lhs.Z, rhs.Z));
	}

	// Returns a vector that is made from the largest components of two vectors.
	public static Vector3 Max(Vector3 lhs, Vector3 rhs) {
		return new Vector3(Math.Max(lhs.X, rhs.X), Math.Max(lhs.Y, rhs.Y), Math.Max(lhs.Z, rhs.Z));
	}

	public static Vector3 Zero => new(0d, 0d, 0d);
	public static Vector3 One => new(1d, 1d, 1d);
	public static Vector3 Up => new(0d, 1d, 0d);
	public static Vector3 Down => new(0d, -1d, 0d);
	public static Vector3 Left => new(-1d, 0d, 0d);
	public static Vector3 Right => new(1d, 0d, 0d);
	public static Vector3 Forward => new(0d, 0d, 1d);
	public static Vector3 Back => new(0d, 0d, -1d);
	public static Vector3 PositiveInfinity => new(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
	public static Vector3 NegativeInfinity => new(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);

	// Adds two vectors.
	public static Vector3 operator +(Vector3 a, Vector3 b) {
		return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
	}

	// Subtracts one vector from another.
	public static Vector3 operator -(Vector3 a, Vector3 b) {
		return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
	}

	// Negates a vector.
	public static Vector3 operator -(Vector3 a) {
		return new Vector3(-a.X, -a.Y, -a.Z);
	}

	// Multiplies a vector by a number.
	public static Vector3 operator *(Vector3 a, double d) {
		return new Vector3(a.X * d, a.Y * d, a.Z * d);
	}

	// Multiplies a vector by a number.
	public static Vector3 operator *(double d, Vector3 a) {
		return new Vector3(a.X * d, a.Y * d, a.Z * d);
	}

	// Divides a vector by a number.
	public static Vector3 operator /(Vector3 a, double d) {
		return new Vector3(a.X / d, a.Y / d, a.Z / d);
	}

	// Returns true if the vectors are equal.
	public static bool operator ==(Vector3 lhs, Vector3 rhs) {
		// Returns false in the presence of NaN values.
		return (lhs - rhs).SqrMagnitude < Epsilon * Epsilon;
	}

	// Returns true if vectors are different.
	public static bool operator !=(Vector3 lhs, Vector3 rhs) {
		// Returns true in the presence of NaN values.
		return !(lhs == rhs);
	}

	public override string ToString() {
		return $"({X:F1}, {Y:F1}, {Z:F1})";
	}

	public string ToString(string format) {
		return $"({X.ToString(format)}, {Y.ToString(format)}, {Z.ToString(format)})";
	}
}