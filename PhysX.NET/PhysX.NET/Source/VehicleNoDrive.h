#pragma once

#include "Physics.h"
#include "VehicleWheels.h"
#include "VehicleWheelsSimData.h"

namespace PhysX
{
	public ref class VehicleNoDrive : VehicleWheels
	{
	public:
		VehicleNoDrive(PhysX::Physics^ physics, RigidDynamic^ actor, VehicleWheelsSimData^ wheelsData);

		// Set vehicle to rest state
		void SetToRestState();

		// Setters for wheel controls
		void SetBrakeTorque(unsigned int wheelId, float brakeTorque);
		void SetDriveTorque(unsigned int wheelId, float driveTorque);
		void SetSteerAngle(unsigned int wheelId, float steerAngle);

		// Getters for wheel controls
		float GetBrakeTorque(unsigned int wheelId);
		float GetDriveTorque(unsigned int wheelId);
		float GetSteerAngle(unsigned int wheelId);

	private:
		static PxVehicleNoDrive* Create(PhysX::Physics^ physics, RigidDynamic^ actor, VehicleWheelsSimData^ wheelsData);

		property PxVehicleNoDrive* NativeNoDrivePointer
		{
			PxVehicleNoDrive* get() { return static_cast<PxVehicleNoDrive*>(this->UnmanagedPointer); }
		}
	};
}