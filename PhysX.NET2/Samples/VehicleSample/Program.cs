using PhysX.Samples.VehicleSample;

var sample = new VehicleSample();

sample.InitializePhysics();
sample.SetUpVehicle();

while (!sample.HasCompleted)
    sample.StepPhysics();