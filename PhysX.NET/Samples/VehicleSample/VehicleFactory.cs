using System.Numerics;

namespace PhysX.Samples.VehicleSample;

public static class VehicleFactory
{
    /// <summary>
    /// Tire model friction for each combination of drivable surface type and tire type.
    /// </summary>
    private static readonly float[][] tireFrictionMultipliers =
    [   //  normal      worn
        [   1.00f,      0.1f    ] // tarmac 
    ];

    private enum DrivableSurfaceType : uint
    {
        Tarmac,
        MaxNumber
    }

    private enum TireType
    {
        Normal = 0,
        Worn,
        MaxNumber
    }

    public static VehicleDrivableSurfaceToTireFrictionPairs CreateFrictionPairs(Material material)
    {
        VehicleDrivableSurfaceToTireFrictionPairs surfaceTirePairs = new((uint)TireType.MaxNumber, (uint)DrivableSurfaceType.MaxNumber);
        surfaceTirePairs.SetUp((uint)TireType.MaxNumber, (uint)DrivableSurfaceType.MaxNumber, [material], [(uint)DrivableSurfaceType.Tarmac]);

        for (uint i = 0; i < (uint)DrivableSurfaceType.MaxNumber; i++)
        {
            for (uint j = 0; j < (uint)DrivableSurfaceType.MaxNumber; j++)
            {
                surfaceTirePairs.SetTypePairFriction(i, j, tireFrictionMultipliers[i][j]);
            }
        }

        return surfaceTirePairs;
    }

    public static RigidStatic CreateDrivablePlane(Physics physics, Material material, FilterData simFilterData)
    {
        // add a plane to the scene
        var groundPlane = physics.CreateRigidStatic();
        groundPlane.Name = "Ground Plane";
        groundPlane.GlobalPose = Matrix4x4.CreateFromAxisAngle(new Vector3(0, 0, 1), (float)Math.PI / 2);

        var planeGeom = new PlaneGeometry();

        // get the plane shape so we can set query and simulation filter data
        var shape = RigidActorExt.CreateExclusiveShape(groundPlane, planeGeom, material, null);

        // set the query filter data of the ground plane so that the vehicle raycasts can hit the ground
        shape.QueryFilterData = new FilterData()
        {
            Word3 = 4294901760// (uint)-65536;
        };

        // set the simulation filter data of the ground plane so that it collides with the chassis of a vehicle but not the wheels
        shape.SimulationFilterData = simFilterData;

        return groundPlane;
    }

    [Flags]
    private enum CollisionFlags : uint
    {
        Ground = 1 << 0,
        Wheel = 1 << 1,
        Chassis = 1 << 2,
        Obstacle = 1 << 3,
        DrivableObstacle = 1 << 4,

        AgainstGround = Chassis | Obstacle | DrivableObstacle,
        AgainstWheel = Wheel | Chassis | Obstacle,
        AgainstChassis = Ground | Wheel | Chassis | Obstacle | DrivableObstacle,
        AgainstObstacle = Ground | Wheel | Chassis | Obstacle | DrivableObstacle,
        AgainstDrivableObstacle = Ground | Chassis | Obstacle | DrivableObstacle
    }

    public static VehicleDescription InitVehicleDescription(Material chassisMaterial, Material wheelMaterial)
    {
        // set up the chassis mass, dimensions, moment of inertia, and center of mass offset
        // the moment of inertia is just the moment of inertia of a cuboid but modified for easier steering
        // center of mass offset is 0.65m above the base of the chassis and 0.25m towards the front
        const float chassisMass = 1500.0f;
        Vector3 chassisDims = new(2.5f, 2.0f, 5.0f);
        Vector3 chassisMOI = new
            ((chassisDims.Y * chassisDims.Y + chassisDims.Z * chassisDims.Z) * chassisMass / 12.0f,
             (chassisDims.X * chassisDims.X + chassisDims.Z * chassisDims.Z) * 0.8f * chassisMass / 12.0f,
             (chassisDims.X * chassisDims.X + chassisDims.Y * chassisDims.Y) * chassisMass / 12.0f);
        Vector3 chassisCMOffset = new(0.0f, -chassisDims.Y * 0.5f + 0.65f, 0.25f);

        // set up the wheel mass, radius, width, moment of inertia, and number of wheels
        // moment of inertia is just the moment of inertia of a cylinder
        const float wheelMass = 20.0f;
        const float wheelRadius = 0.5f;
        const float wheelWidth = 0.4f;
        const float wheelMOI = 0.5f * wheelMass * wheelRadius * wheelRadius;
        const uint nbWheels = 6;

        return new(chassisMass, chassisDims, chassisMOI, chassisCMOffset, chassisMaterial, new((uint)CollisionFlags.Chassis, (uint)CollisionFlags.AgainstChassis, 0, 0), wheelMass, wheelWidth, wheelRadius, wheelMOI, nbWheels, wheelMaterial, new((uint)CollisionFlags.Wheel, (uint)CollisionFlags.AgainstWheel, 0, 0));
    }

    public static VehicleDrive4W CreateVehicle4W(Physics physics, Cooking cooking, VehicleDescription vehicleDecription)
    {
        Vector3 chassisDims = vehicleDecription.chassisDims;
        float wheelWidth = vehicleDecription.wheelWidth;
        float wheelRadius = vehicleDecription.wheelRadius;
        uint numWheels = vehicleDecription.numWheels;

        FilterData chassisSimFilterData = vehicleDecription.chassisSimFilterData;
        FilterData wheelSimFilterData = vehicleDecription.wheelSimFilterData;

        // construct an actor with shapes for the chassis and wheels
        // set the rigid body mass, moment of inertia, and center of mass offset
        RigidDynamic veh4WActor;
        {
            // construct a convex mesh for a cylindrical wheel
            ConvexMesh wheelMesh = CreateWheelMesh(physics, cooking, wheelWidth, wheelRadius);
            // assume all wheels are identical for simplicity
            ConvexMesh[] wheelConvexMeshes = new ConvexMesh[VehicleSDK.MaxWheelNumber];
            Material[] wheelMaterials = new Material[VehicleSDK.MaxWheelNumber];

            // set the meshes and materials for the driven wheels
            for (uint i = (uint)VehicleWheelOrdering.FrontLeft; i <= (uint)VehicleWheelOrdering.RearRight; i++)
            {
                wheelConvexMeshes[i] = wheelMesh;
                wheelMaterials[i] = vehicleDecription.wheelMaterial;
            }

            // set the meshes and materials for the non-driven wheels
            for (uint i = (uint)VehicleWheelOrdering.RearRight + 1; i < numWheels; i++)
            {
                wheelConvexMeshes[i] = wheelMesh;
                wheelMaterials[i] = vehicleDecription.wheelMaterial;
            }

            // chassis just has a single convex shape for simplicity
            ConvexMesh chassisConvexMesh = CreateChassisMesh(physics, cooking, chassisDims);

            // rigid body data
            VehicleChassisData rigidBodyData = new()
            {
                MomentOfInertia = vehicleDecription.chassisMOI,
                Mass = vehicleDecription.chassisMass,
                CenterOfMassOffset = vehicleDecription.chassisCMOffset
            };

            veh4WActor = CreateVehicleActor(physics, rigidBodyData, wheelMaterials, wheelConvexMeshes, wheelSimFilterData, [vehicleDecription.chassisMaterial], [chassisConvexMesh], chassisSimFilterData);
        }

        // set up the sim data for the wheels
        VehicleWheelsSimData wheelsSimData;
        {
            // compute the wheel center offsets from the origin
            float frontZ = chassisDims.Z * 0.3f;
            float rearZ = -chassisDims.Z * 0.3f;
            var wheelCenterActorOffsets = ComputeWheelCenterActorOffsets4W(frontZ, rearZ, chassisDims, wheelWidth, wheelRadius, numWheels);

            //Set up the simulation data for all wheels.
            wheelsSimData = SetupWheelsSimulationData(physics, vehicleDecription.wheelMass, vehicleDecription.wheelMOI, wheelRadius, wheelWidth, numWheels, wheelCenterActorOffsets, vehicleDecription.chassisCMOffset, vehicleDecription.chassisMass);
        }

        //Set up the sim data for the vehicle drive model.
        VehicleDriveSimData4W driveSimData = new();
        {
            //Diff
            VehicleDifferential4WData diff = new() { Type = VehicleDifferentialType.LimitedSlip4WheelDrive };
            driveSimData.SetDifferentialData(diff);

            //Engine - approx 6000 rpm
            VehicleEngineData engine = new()
            {
                PeakTorque = 500.0f,
                MaxOmega = 600.0f
            };
            driveSimData.SetEngineData(engine);

            //Gears
            VehicleGearsData gears = new()
            {
                SwitchTime = 0.5f
            };
            driveSimData.SetGearsData(gears);

            //Clutch
            VehicleClutchData clutch = new()
            {
                Strength = 10.0f
            };
            driveSimData.SetClutchData(clutch);

            //Ackermann steer accuracy
            VehicleAckermannGeometryData ackermann = new()
            {
                Accuracy = 1.0f,
                AxleSeparation = wheelsSimData.GetWheelCentreOffset((int)VehicleWheelOrdering.FrontLeft).Z - wheelsSimData.GetWheelCentreOffset((int)VehicleWheelOrdering.RearLeft).Z,
                FrontWidth = wheelsSimData.GetWheelCentreOffset((int)VehicleWheelOrdering.FrontRight).X - wheelsSimData.GetWheelCentreOffset((int)VehicleWheelOrdering.FrontLeft).X,
                RearWidth = wheelsSimData.GetWheelCentreOffset((int)VehicleWheelOrdering.RearRight).X - wheelsSimData.GetWheelCentreOffset((int)VehicleWheelOrdering.RearLeft).X
            };
            driveSimData.SetAckermannGeometryData(ackermann);
        }

        // create a vehicle from the wheels and drive sim data
        var vehDrive4W = new VehicleDrive4W(physics, veh4WActor, wheelsSimData, driveSimData, (int)numWheels - 4);

        return vehDrive4W;
    }

    private static ConvexMesh CreateWheelMesh(Physics physics, Cooking cooking, float width, float radius)
    {
        var vertices = new Vector3[2 * 16];
        for (uint i = 0; i < 16; i++)
        {
            float cosTheta = MathF.Cos(i * MathF.PI * 2.0f / 16.0f);
            float sinTheta = MathF.Sin(i * MathF.PI * 2.0f / 16.0f);
            float y = radius * cosTheta;
            float z = radius * sinTheta;
            vertices[2 * i + 0] = new(-width / 2.0f, y, z);
            vertices[2 * i + 1] = new(+width / 2.0f, y, z);
        }

        return CreateConvexMesh(physics, cooking, vertices);
    }

    private static ConvexMesh CreateChassisMesh(Physics physics, Cooking cooking, Vector3 dimension)
    {
        float x = dimension.X * 0.5f;
        float y = dimension.Y * 0.5f;
        float z = dimension.Z * 0.5f;
        Vector3[] vertices =
        [
            new(x,y,-z),
            new(x,y,z),
            new(x,-y,z),
            new(x,-y,-z),
            new(-x,y,-z),
            new(-x,y,z),
            new(-x,-y,z),
            new(-x,-y,-z)
        ];

        return CreateConvexMesh(physics, cooking, vertices);
    }

    private static ConvexMesh CreateConvexMesh(Physics physics, Cooking cooking, Vector3[] vertices)
    {
        // Create descriptor for convex mesh
        var convexDescription = new ConvexMeshDesc() { Flags = ConvexFlag.ComputeConvex };
        convexDescription.SetPositions(vertices);
        convexDescription.SetTriangles(Array.Empty<uint>());

        MemoryStream buffer = new();

        var result = cooking.CookConvexMesh(convexDescription, buffer);

        if (result != ConvexMeshCookingResult.Success)
            throw new Exception($"Could not cook convex mesh!");

        buffer.Position = 0;

        return physics.CreateConvexMesh(buffer);
    }

    private static RigidDynamic CreateVehicleActor(Physics physics, VehicleChassisData chassisData, Material[] wheelMaterials, ConvexMesh[] wheelConvexMeshes, FilterData wheelSimFilterData, Material[] chassisMaterials, ConvexMesh[] chassisConvexMeshes, FilterData chassisSimFilterData)
    {
        // we need a rigid body actor for the vehicle
        // don't forget to add the actor to the scene after setting up the associated vehicle
        RigidDynamic vehActor = physics.CreateRigidDynamic();

        // wheel and chassis query filter data
        // optional: cars don't drive on other cars
        var wheelQryFilterData = new FilterData();
        SetupNonDrivableSurface(ref wheelQryFilterData);
        var chassisQryFilterData = new FilterData();
        SetupNonDrivableSurface(ref chassisQryFilterData);

        // add all the wheel shapes to the actor
        for (uint i = 0; i < 6; i++)//wheelConvexMeshes.Length; i++) // change number of wheels here!
        {
            var geom = new ConvexMeshGeometry(wheelConvexMeshes[i]);
            var wheelShape = RigidActorExt.CreateExclusiveShape(vehActor, geom, wheelMaterials[0]);
            wheelShape.QueryFilterData = wheelQryFilterData;
            wheelShape.SimulationFilterData = wheelSimFilterData;
            wheelShape.LocalPose = Matrix4x4.Identity;
        }

        // add the chassis shapes to the actor
        for (uint i = 0; i < chassisConvexMeshes.Length; i++)
        {
            Shape chassisShape = RigidActorExt.CreateExclusiveShape(vehActor, new ConvexMeshGeometry(chassisConvexMeshes[i]), chassisMaterials[i]);
            chassisShape.QueryFilterData = chassisQryFilterData;
            chassisShape.SimulationFilterData = chassisSimFilterData;
            chassisShape.LocalPose = Matrix4x4.Identity;
        }

        vehActor.Mass = chassisData.Mass;
        vehActor.MassSpaceInertiaTensor = chassisData.MomentOfInertia;
        vehActor.CenterOfMassLocalPose = Matrix4x4.CreateFromQuaternion(Quaternion.Identity) * Matrix4x4.CreateTranslation(chassisData.CenterOfMassOffset);

        return vehActor;
    }

    private static Vector3[] ComputeWheelCenterActorOffsets4W(float wheelFrontZ, float wheelRearZ, Vector3 chassisDims, float wheelWidth, float wheelRadius, uint numWheels)
    {
        // chassisDims.z is the distance from the rear of the chassis to the front of the chassis
        // the front has z = 0.5 * chassisDims.z and the rear has z = -0.5 * chassisDims.z
        // compute a position for the front wheel and the rear wheel along the z-axis
        // compute the separation between each wheel along the z-axis
        float numLeftWheels = numWheels / 2.0f;
        float deltaZ = (wheelFrontZ - wheelRearZ) / (numLeftWheels - 1.0f);

        // set the outside of the left and right wheels to be flush with the chassis
        // set the top of the wheel to be just touching the underside of the chassis
        // begin by setting the rear-left/rear-right/front-left,front-right wheels
        var wheelCentreOffsets = new Vector3[numWheels];
        wheelCentreOffsets[(int)VehicleWheelOrdering.RearLeft] = new((-chassisDims.X + wheelWidth) * 0.5f, -(chassisDims.Y / 2 + wheelRadius), wheelRearZ + 0 * deltaZ * 0.5f);
        wheelCentreOffsets[(int)VehicleWheelOrdering.RearRight] = new((+chassisDims.X - wheelWidth) * 0.5f, -(chassisDims.Y / 2 + wheelRadius), wheelRearZ + 0 * deltaZ * 0.5f);
        wheelCentreOffsets[(int)VehicleWheelOrdering.FrontLeft] = new((-chassisDims.X + wheelWidth) * 0.5f, -(chassisDims.Y / 2 + wheelRadius), wheelRearZ + (numLeftWheels - 1) * deltaZ);
        wheelCentreOffsets[(int)VehicleWheelOrdering.FrontRight] = new((+chassisDims.X - wheelWidth) * 0.5f, -(chassisDims.Y / 2 + wheelRadius), wheelRearZ + (numLeftWheels - 1) * deltaZ);

        // set the remaining wheels
        for (uint i = 2, wheelCount = 4; i < numWheels - 2; i += 2, wheelCount += 2)
        {
            wheelCentreOffsets[wheelCount + 0] = new((-chassisDims.X + wheelWidth) * 0.5f, -(chassisDims.Y / 2 + wheelRadius), wheelRearZ + i * deltaZ * 0.5f);
            wheelCentreOffsets[wheelCount + 1] = new((+chassisDims.X - wheelWidth) * 0.5f, -(chassisDims.Y / 2 + wheelRadius), wheelRearZ + i * deltaZ * 0.5f);
        }

        return wheelCentreOffsets;
    }

    public static VehicleWheelsSimData SetupWheelsSimulationData(Physics physics, float wheelMass, float wheelMOI, float wheelRadius, float wheelWidth, uint numWheels, Vector3[] wheelCenterActorOffsets, Vector3 chassisCMOffset, float chassisMass)
    {
        // set up the wheels
        var wheels = new VehicleWheelData[numWheels];
        {
            // set up the wheel data structures with mass, moi, radius, width
            for (uint i = 0; i < numWheels; i++)
            {
                wheels[i] = new()
                {
                    Mass = wheelMass,
                    MomentOfInertia = wheelMOI,
                    Radius = wheelRadius,
                    Width = wheelWidth
                };
            }

            // enable the handbrake for the rear wheels only
            wheels[(int)VehicleWheelOrdering.RearLeft].MaxHandBrakeTorque = 4000.0f;
            wheels[(int)VehicleWheelOrdering.RearRight].MaxHandBrakeTorque = 4000.0f;
            // enable steering for the front wheels only
            wheels[(int)VehicleWheelOrdering.FrontLeft].MaxSteer = MathF.PI * 0.3333f;
            wheels[(int)VehicleWheelOrdering.FrontRight].MaxSteer = MathF.PI * 0.3333f;
        }

        // set up the tires
        var tires = new VehicleTireData[VehicleSDK.MaxWheelNumber];
        {
            // set up the tires
            for (uint i = 0; i < numWheels; i++)
            {
                tires[i] = new()
                {
                    LateralStiffnessX = 2.0f,
                    LateralStiffnessY = 0.3125f * (180.0f / MathF.PI),
                    LongitudinalStiffnessPerUnitGravity = 1000.0f,
                    CamberStiffnessPerUnitGravity = 0.1f * (180.0f / MathF.PI),
                    Type = (int)TireType.Normal
                };
            }
        }

        // set up the suspensions
        var suspensions = new VehicleSuspensionData[VehicleSDK.MaxWheelNumber];
        {
            // compute the mass supported by each suspension spring
            var suspSprungMasses = physics.VehicleSDK.VehicleComputeSprungMasses(wheelCenterActorOffsets, chassisCMOffset, chassisMass, 1);

            // set the suspension data
            for (uint i = 0; i < numWheels; i++)
            {
                suspensions[i] = new()
                {
                    MaxCompression = 0.3f,
                    MaxDroop = 0.1f,
                    SpringStrength = 35000.0f,
                    SpringDamperRate = 4500.0f,
                    SprungMass = suspSprungMasses[i]
                };
            }

            // set the camber angles
            const float camberAngleAtRest = 0.0f;
            const float camberAngleAtMaxDroop = 0.01f;
            const float camberAngleAtMaxCompression = -0.01f;
            for (uint i = 0; i < numWheels; i += 2)
            {
                suspensions[i + 0].CamberAtRest = camberAngleAtRest;
                suspensions[i + 1].CamberAtRest = -camberAngleAtRest;
                suspensions[i + 0].CamberAtMaxDroop = camberAngleAtMaxDroop;
                suspensions[i + 1].CamberAtMaxDroop = -camberAngleAtMaxDroop;
                suspensions[i + 0].CamberAtMaxCompression = camberAngleAtMaxCompression;
                suspensions[i + 1].CamberAtMaxCompression = -camberAngleAtMaxCompression;
            }
        }

        // set up the wheel geometry
        var suspTravelDirections = new Vector3[VehicleSDK.MaxWheelNumber];
        var wheelCentreCMOffsets = new Vector3[VehicleSDK.MaxWheelNumber];
        var suspForceAppCMOffsets = new Vector3[VehicleSDK.MaxWheelNumber];
        var tireForceAppCMOffsets = new Vector3[VehicleSDK.MaxWheelNumber];
        {
            // set the geometry data
            for (uint i = 0; i < numWheels; i++)
            {
                // vertical suspension travel
                suspTravelDirections[i] = -Vector3.UnitY;

                // wheel center offset is offset from rigid body center of mass
                wheelCentreCMOffsets[i] =
                    wheelCenterActorOffsets[i] - chassisCMOffset;

                // suspension force application point 0.3 metres below rigid body center of mass
                suspForceAppCMOffsets[i] = new Vector3(wheelCentreCMOffsets[i].X, -0.3f, wheelCentreCMOffsets[i].Z);

                // tire force application point 0.3 metres below rigid body center of mass
                tireForceAppCMOffsets[i] = new Vector3(wheelCentreCMOffsets[i].X, -0.3f, wheelCentreCMOffsets[i].Z);
            }
        }

        // set up the filter data of the raycast that will be issued by each suspension
        var queryFilterData = new FilterData();
        SetupNonDrivableSurface(ref queryFilterData);

        // Set the wheel, tire, suspension data, geometry data and query filter data
        var wheelsSimData = new VehicleWheelsSimData(wheels.Length);
        for (int i = 0; i < numWheels; i++)
        {
            wheelsSimData.SetWheelData(i, wheels[i]);
            wheelsSimData.SetTireData(i, tires[i]);
            wheelsSimData.SetSuspensionData(i, suspensions[i]);
            wheelsSimData.SetSuspensionTravelDirection(i, suspTravelDirections[i]);
            wheelsSimData.SetWheelCentreOffset(i, wheelCentreCMOffsets[i]);
            wheelsSimData.SetSuspensionForceApplicationPointOffset(i, suspForceAppCMOffsets[i]);
            wheelsSimData.SetTireForceApplicationPointOffset(i, tireForceAppCMOffsets[i]);
            wheelsSimData.SetSceneQueryFilterData(i, queryFilterData);
            wheelsSimData.SetWheelShapeMapping(i, i);
        }

        // add a front and rear anti-roll bar
        VehicleAntiRollBarData barFront = new()
        {
            Wheel0 = (int)VehicleWheelOrdering.FrontLeft,
            Wheel1 = (int)VehicleWheelOrdering.FrontRight,
            Stiffness = 10000.0f
        };
        wheelsSimData.AddAntiRollBarData(barFront);

        VehicleAntiRollBarData barRear = new()
        {
            Wheel0 = (int)VehicleWheelOrdering.RearLeft,
            Wheel1 = (int)VehicleWheelOrdering.RearRight,
            Stiffness = 10000.0f
        };
        wheelsSimData.AddAntiRollBarData(barRear);

        return wheelsSimData;
    }

    private enum SurfaceType : uint
    {
        Drivable = 0xffff0000,
        Undrivable = 0x0000ffff
    };

    public static void SetupNonDrivableSurface(ref FilterData filterData)
    {
        filterData.Word3 = (uint)SurfaceType.Undrivable;
    }
}