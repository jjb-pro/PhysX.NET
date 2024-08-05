using System.Numerics;

namespace PhysX.Samples.VehicleSample;

public readonly struct VehicleDescription(float chassisMass, Vector3 chassisDims, Vector3 chassisMOI, Vector3 chassisCMOffset, Material chassisMaterial, FilterData chassisSimFilterData, float wheelMass, float wheelWidth, float wheelRadius, float wheelMOI, uint numWheels, Material wheelMaterial, FilterData wheelSimFilterData)
{
    public readonly float chassisMass = chassisMass;
    public readonly Vector3 chassisDims = chassisDims;
    public readonly Vector3 chassisMOI = chassisMOI;
    public readonly Vector3 chassisCMOffset = chassisCMOffset;
    public readonly Material chassisMaterial = chassisMaterial;
    public readonly FilterData chassisSimFilterData = chassisSimFilterData;  // word0 = collide type, word1 = collide against types, word2 = PxPairFlags

    public readonly float wheelMass = wheelMass;
    public readonly float wheelWidth = wheelWidth;
    public readonly float wheelRadius = wheelRadius;
    public readonly float wheelMOI = wheelMOI;
    public readonly uint numWheels = numWheels;
    public readonly Material wheelMaterial = wheelMaterial;
    public readonly FilterData wheelSimFilterData = wheelSimFilterData;    // word0 = collide type, word1 = collide against types, word2 = PxPairFlags
}