namespace PhysX.Samples.VehicleSample;

public class VehicleInputHelper(VehicleDrive4W vehicle)
{
    private readonly bool UseMimicKeyInputs = false;

    private readonly VehicleDrive4WRawInputData vehicleInputData = new();

    private enum DriveMode
    {
        AccelerationForward,
        AccelerationReverse,
        HardTurnLeft,
        HandbrakeTurnLeft,
        HardTurnRight,
        HandbrakeTurnRight,
        Brake,
        None
    };

    private readonly DriveMode[] driveModeOrder = [
        DriveMode.Brake,
        DriveMode.AccelerationForward,
        DriveMode.Brake,
        DriveMode.AccelerationReverse,
        DriveMode.Brake,
        DriveMode.HardTurnLeft,
        DriveMode.Brake,
        DriveMode.HardTurnRight,
        DriveMode.AccelerationForward,
        DriveMode.HandbrakeTurnLeft,
        DriveMode.AccelerationForward,
        DriveMode.HandbrakeTurnRight,
        DriveMode.None
    ];

    private const float ModeLifetime = 4.0f;
    private float modeTimer = 0.0f;
    private uint orderProgress = 0;

    public bool VehicleOrderComplete { get; private set; } = false;

    private readonly VehicleKeySmoothingData gKeySmoothingData = new()
    {
        RiseRates = [
        6.0f,	//rise rate eANALOG_INPUT_ACCEL
		    6.0f,	//rise rate eANALOG_INPUT_BRAKE		
		    6.0f,	//rise rate eANALOG_INPUT_HANDBRAKE	
		    2.5f,	//rise rate eANALOG_INPUT_STEER_LEFT
		    2.5f,	//rise rate eANALOG_INPUT_STEER_RIGHT
	    ],
        FallRates = [
        10.0f,	//fall rate eANALOG_INPUT_ACCEL
		    10.0f,	//fall rate eANALOG_INPUT_BRAKE		
		    10.0f,	//fall rate eANALOG_INPUT_HANDBRAKE	
		    5.0f,	//fall rate eANALOG_INPUT_STEER_LEFT
		    5.0f    //fall rate eANALOG_INPUT_STEER_RIGHT
    ]
    };

    private readonly VehiclePadSmoothingData gPadSmoothingData = new()
    {
        RiseRates = [
            6.0f,	//rise rate eANALOG_INPUT_ACCEL
		    6.0f,	//rise rate eANALOG_INPUT_BRAKE		
		    6.0f,	//rise rate eANALOG_INPUT_HANDBRAKE	
		    2.5f,	//rise rate eANALOG_INPUT_STEER_LEFT
		    2.5f,	//rise rate eANALOG_INPUT_STEER_RIGHT
	    ],
        FallRates = [
            10.0f,	//fall rate eANALOG_INPUT_ACCEL
		    10.0f,	//fall rate eANALOG_INPUT_BRAKE		
		    10.0f,	//fall rate eANALOG_INPUT_HANDBRAKE	
		    5.0f,	//fall rate eANALOG_INPUT_STEER_LEFT
		    5.0f    //fall rate eANALOG_INPUT_STEER_RIGHT
        ]
    };

    private readonly FixedSizeLookupTable steerVsForwardSpeedTable = new(
        [
            0.0f, 0.75f,
                5.0f, 0.75f,
                30.0f, 0.125f,
                120.0f, 0.1f,
                float.MaxValue, float.MaxValue,
                float.MaxValue, float.MaxValue,
                float.MaxValue, float.MaxValue,
                float.MaxValue, float.MaxValue
        ]
    );

    public void IncrementDrivingMode(float timeStep)
    {
        modeTimer += timeStep;

        if (modeTimer <= ModeLifetime)
            return;

        // if the mode just completed was AccelerationReverse then switch back to forward gears
        if (DriveMode.AccelerationReverse == driveModeOrder[orderProgress])
            vehicle.DriveDynData.ForceGearChange((int)VehicleGears.First);

        // increment to next driving mode
        modeTimer = 0.0f;
        orderProgress++;
        ReleaseAllControls();

        // if we are at the end of the list of driving modes then start again
        if (DriveMode.None == driveModeOrder[orderProgress])
        {
            orderProgress = 0;
            VehicleOrderComplete = true;
        }

        // start driving in the selected mode
        DriveMode driveMode = driveModeOrder[orderProgress];
        switch (driveMode)
        {
            case DriveMode.AccelerationForward:
                StartAccelerateForwardsMode();
                break;
            case DriveMode.AccelerationReverse:
                StartAccelerateReverseMode();
                break;
            case DriveMode.HardTurnLeft:
                StartTurnHardLeftMode();
                break;
            case DriveMode.HandbrakeTurnLeft:
                StartHandbrakeTurnLeftMode();
                break;
            case DriveMode.HardTurnRight:
                StartTurnHardRightMode();
                break;
            case DriveMode.HandbrakeTurnRight:
                StartHandbrakeTurnRightMode();
                break;
            case DriveMode.Brake:
                StartBrakeMode();
                break;
            case DriveMode.None:
                break;
        };

        // if the mode about to start is AccelerationReverse then switch to reverse gears
        if (DriveMode.AccelerationReverse == driveModeOrder[orderProgress])
            vehicle.DriveDynData.ForceGearChange((int)VehicleGears.Reverse);
    }

    private void StartAccelerateForwardsMode()
    {
        if (UseMimicKeyInputs)
        {
            vehicleInputData.SetDigitalAcceleration(true);
        }
        else
        {
            vehicleInputData.SetAnalogAcceleration(1.0f);
        }
    }

    private void StartAccelerateReverseMode()
    {
        vehicle.DriveDynData.ForceGearChange((int)VehicleGears.Reverse);

        if (UseMimicKeyInputs)
        {
            vehicleInputData.SetDigitalAcceleration(true);
        }
        else
        {
            vehicleInputData.SetAnalogAcceleration(1.0f);
        }
    }

    private void StartBrakeMode()
    {
        if (UseMimicKeyInputs)
        {
            vehicleInputData.SetDigitalBrake(true);
        }
        else
        {
            vehicleInputData.SetAnalogBrake(1.0f);
        }
    }

    private void StartTurnHardLeftMode()
    {
        if (UseMimicKeyInputs)
        {
            vehicleInputData.SetDigitalAcceleration(true);
            vehicleInputData.SetDigitalSteerLeft(true);
        }
        else
        {
            vehicleInputData.SetAnalogAcceleration(1.0f);
            vehicleInputData.SetAnalogSteer(-1.0f);
        }
    }

    private void StartTurnHardRightMode()
    {
        if (UseMimicKeyInputs)
        {
            vehicleInputData.SetDigitalAcceleration(true);
            vehicleInputData.SetDigitalSteerRight(true);
        }
        else
        {
            vehicleInputData.SetAnalogAcceleration(1.0f);
            vehicleInputData.SetAnalogSteer(1.0f);
        }
    }

    private void StartHandbrakeTurnLeftMode()
    {
        if (UseMimicKeyInputs)
        {
            vehicleInputData.SetDigitalSteerLeft(true);
            vehicleInputData.SetDigitalHandbrake(true);
        }
        else
        {
            vehicleInputData.SetAnalogSteer(-1.0f);
            vehicleInputData.SetAnalogHandbrake(1.0f);
        }
    }

    private void StartHandbrakeTurnRightMode()
    {
        if (UseMimicKeyInputs)
        {
            vehicleInputData.SetDigitalSteerRight(true);
            vehicleInputData.SetDigitalHandbrake(true);
        }
        else
        {
            vehicleInputData.SetAnalogSteer(1.0f);
            vehicleInputData.SetAnalogHandbrake(1.0f);
        }
    }

    private void ReleaseAllControls()
    {
        if (UseMimicKeyInputs)
        {
            vehicleInputData.SetDigitalAcceleration(false);
            vehicleInputData.SetDigitalSteerLeft(false);
            vehicleInputData.SetDigitalSteerRight(false);
            vehicleInputData.SetDigitalBrake(false);
            vehicleInputData.SetDigitalHandbrake(false);
        }
        else
        {
            vehicleInputData.SetAnalogAcceleration(0.0f);
            vehicleInputData.SetAnalogSteer(0.0f);
            vehicleInputData.SetAnalogBrake(0.0f);
            vehicleInputData.SetAnalogHandbrake(0.0f);
        }
    }

    public void UpdateControlInputs(float timeStep, bool isVehicleInAir)
    {
        if (UseMimicKeyInputs)
            vehicle.SmoothDigitalRawInputsAndSetAnalogInputs(gKeySmoothingData, steerVsForwardSpeedTable, vehicleInputData, timeStep, isVehicleInAir);
        else
            vehicle.SmoothAnalogRawInputsAndSetAnalogInputs(gPadSmoothingData, steerVsForwardSpeedTable, vehicleInputData, timeStep, isVehicleInAir);
    }
}