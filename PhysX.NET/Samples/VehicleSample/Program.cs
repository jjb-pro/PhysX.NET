using PhysX.Samples.VehicleSample;

// you can remove the constant USE_VEHICLE4W in project properties to use the no drive vehicle sample

var sample = new VehicleSample();

sample.InitializePhysics();
sample.SetUpVehicle();

while (!sample.HasCompleted)
    sample.StepPhysics();