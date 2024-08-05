#pragma once

#include "VehicleDrive.h"
#include "VehicleDifferential4WData.h"
#include "VehicleAckermannGeometryData.h"
#include "VehicleDriveSimData4W.h"
#include "VehicleWheelsSimData.h"
#include "FixedSizeLookupTable.h"
#include "VehiclePadSmoothingData.h"
#include "VehicleKeySmoothingData.h"
#include "VehicleDrive4WRawInputData.h"

namespace PhysX
{
	public ref class VehicleDrive4W : VehicleDrive
	{
	private:
		VehicleDriveSimData4W^ _simData;

	public:
		VehicleDrive4W(PhysX::Physics^ physics, RigidDynamic^ actor, VehicleWheelsSimData^ wheelsData, VehicleDriveSimData4W^ driveData, int numNonDrivenWheels);

	private:
		static PxVehicleDrive4W* Create(PhysX::Physics^ physics, RigidDynamic^ actor, VehicleWheelsSimData^ wheelsData, VehicleDriveSimData4W^ driveData, int numNonDrivenWheels);
		
	public:
		/// <summary>
		/// Set a vehicle to its rest state. Aside from the rigid body transform, this will set the vehicle and rigid body to the state they were in immediately after setup or create.
		/// </summary>
		void SetToRestState();

		void SmoothDigitalRawInputsAndSetAnalogInputs(VehicleKeySmoothingData^ keySmoothingData, FixedSizeLookupTable^ steerVsForwardSpeedTable, VehicleDrive4WRawInputData^ rawInputData, float timestep, bool vehicleIsInAir);
		void SmoothAnalogRawInputsAndSetAnalogInputs(VehiclePadSmoothingData^ padSmoothingData, FixedSizeLookupTable^ steerVsForwardSpeedTable, VehicleDrive4WRawInputData^ rawInputData, float timestep, bool vehicleIsInAir);

		property VehicleDriveSimData4W^ DriveSimData
		{
			VehicleDriveSimData4W^ get();
		}
		
	internal:
		property PxVehicleDrive4W* UnmanagedPointer
		{
			PxVehicleDrive4W* get();
		}
	};
};