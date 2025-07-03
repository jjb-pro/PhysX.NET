using PhysX.VisualDebugger;
using System.Numerics;
using static PhysX.Samples.VehicleSample.VehicleFactory;

namespace PhysX.Samples.VehicleSample;

public class VehicleSample
{
    // physics
    private Foundation foundation;
    private Physics physics;

    private Pvd pVD;

    private Scene scene;
    private Material defaultMaterial;
    private Cooking cooking;

    // vehicle
    private BatchQuery vehicleBatchQuery;
    private VehicleSceneQueryData vehicleSceneQueryData;
    private VehicleDrivableSurfaceToTireFrictionPairs vehicleFrictionPairs;

    private RigidStatic groundPlane;

#if USE_VEHICLE4W
    public bool HasCompleted => inputHelper.VehicleOrderComplete;
    private VehicleDrive4W vehicle;
#else
    public bool HasCompleted => _frameCount >= 500;
    private VehicleNoDrive vehicle;
#endif

    // helper class for setting input
    private VehicleInputHelper inputHelper;

    private uint _frameCount = 0;

    // initialize physics like always
    public void InitializePhysics()
    {
        foundation = new Foundation(new PhysXErrorCallback());
        pVD = new Pvd(foundation);
        pVD.Connect("localhost", connectionType: InstrumentationFlag.All);

        physics = new Physics(foundation, true, pVD);
        var sceneDesc = new SceneDesc
        {
            Gravity = new Vector3(0.0f, -9.81f, 0.0f),
            FilterShader = new VehicleFilterShader()
        };

        scene = physics.CreateScene(sceneDesc);
        defaultMaterial = physics.CreateMaterial(0.5f, 0.5f, 0.6f);

        cooking = physics.CreateCooking();
    }

    // set up the vehicle physics and the vehicle with the help of VehicleFactory
    public void SetUpVehicle()
    {
        // initialize vehicle SDK
        physics.VehicleSDK.Initalize();
        physics.VehicleSDK.VehicleSetBasisVectors(new(0, 1, 0), new(0, 0, 1));
        physics.VehicleSDK.VehicleSetUpdateMode(VehicleUpdateMode.VelocityChange);

        // create the batched scene queries for the suspension raycasts
        vehicleSceneQueryData = new(foundation, 1, VehicleSDK.MaxWheelNumber, 1, 1);
        vehicleBatchQuery = vehicleSceneQueryData.SetUpBatchedSceneQuery(scene, 0);

        // create the friction table for each combination of tire and surface type
        vehicleFrictionPairs = CreateFrictionPairs(defaultMaterial);

        // create a plane to drive on
        FilterData groundPlaneSimFilterData = new(1, 28, 0, 0);
        groundPlane = CreateDrivablePlane(physics, defaultMaterial, groundPlaneSimFilterData);
        scene.AddActor(groundPlane);

        // create a vehicle that will drive on the plane
        VehicleDescription vehicleDescription = InitVehicleDescription(defaultMaterial, defaultMaterial);
#if USE_VEHICLE4W
        vehicle = CreateVehicle4W(physics, cooking, vehicleDescription);
#else
        vehicle = CreateVehicleNoDrive(physics, cooking, vehicleDescription);
#endif
        var startTransform = new Transform(Quaternion.Identity, new(0, vehicleDescription.chassisDims.Y * 0.5f + vehicleDescription.wheelRadius + 1.0f + 0f, 0));
        vehicle.Actor.GlobalPose = startTransform.ToMatrix();
        scene.AddActor(vehicle.Actor);

        // set the vehicle to rest in first gear and to use auto-gears
        vehicle.SetToRestState();

#if USE_VEHICLE4W
        vehicle.DriveDynData.ForceGearChange((int)VehicleGears.First);
        vehicle.DriveDynData.UseAutoGears = true;

        inputHelper = new(vehicle);
#endif
    }

    private bool isVehicleInAir = false;

    public void StepPhysics()
    {
        float timeStep = 1.0f / 60.0f;
        _frameCount++;

#if USE_VEHICLE4W
        // cycle through the driving modes to demonstrate how to accelerate/reverse/brake/turn etc.
        inputHelper.IncrementDrivingMode(timeStep);

        // update the control inputs for the vehicle
        inputHelper.UpdateControlInputs(timeStep, isVehicleInAir);
#else
        vehicle.SetSteerAngle(2, 10);
        vehicle.SetSteerAngle(3, 10);

        vehicle.SetDriveTorque(2, 1000);
        vehicle.SetDriveTorque(3, 1000);
#endif

        // raycasts
        var vehicles = new VehicleWheels[] { vehicle };
        _ = vehicleSceneQueryData.InvokeSuspensionRaycasts(vehicleBatchQuery, vehicles);

        // vehicle update
        var vehicleQueryResults = physics.VehicleSDK.VehicleUpdates(timeStep, scene.Gravity, vehicleFrictionPairs, vehicles);

        // work out if the vehicle is in the air
        isVehicleInAir = !vehicle.Actor.IsSleeping && physics.VehicleSDK.VehicleIsInAir(vehicleQueryResults[0]);

        // scene update
        scene.Simulate(timeStep);
        scene.FetchResults(true);
    }
}