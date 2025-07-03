#include "StdAfx.h"
#include "VehicleNoDrive.h"

PhysX::VehicleNoDrive::VehicleNoDrive(PhysX::Physics^ physics, RigidDynamic^ actor, VehicleWheelsSimData^ wheelsData) : VehicleWheels(Create(physics, actor, wheelsData), physics)
{

}

PxVehicleNoDrive* VehicleNoDrive::Create(PhysX::Physics^ physics, RigidDynamic^ actor, VehicleWheelsSimData^ wheelsData)
{
	ThrowIfNullOrDisposed(physics, "physics");
	ThrowIfNullOrDisposed(actor, "actor");
	ThrowIfNull(wheelsData, "wheelsData");

	PxVehicleWheelsSimData* wd = wheelsData->UnmanagedPointer;

	return PxVehicleNoDrive::create(physics->UnmanagedPointer, actor->UnmanagedPointer, *wd);
}

void VehicleNoDrive::SetToRestState()
{
    NativeNoDrivePointer->setToRestState();
}

void VehicleNoDrive::SetBrakeTorque(unsigned int wheelId, float brakeTorque)
{
    NativeNoDrivePointer->setBrakeTorque(wheelId, brakeTorque);
}

void VehicleNoDrive::SetDriveTorque(unsigned int wheelId, float driveTorque)
{
    NativeNoDrivePointer->setDriveTorque(wheelId, driveTorque);
}

void VehicleNoDrive::SetSteerAngle(unsigned int wheelId, float steerAngle)
{
    NativeNoDrivePointer->setSteerAngle(wheelId, steerAngle);
}

float VehicleNoDrive::GetBrakeTorque(unsigned int wheelId)
{
    return NativeNoDrivePointer->getBrakeTorque(wheelId);
}

float VehicleNoDrive::GetDriveTorque(unsigned int wheelId)
{
    return NativeNoDrivePointer->getDriveTorque(wheelId);
}

float VehicleNoDrive::GetSteerAngle(unsigned int wheelId)
{
    return NativeNoDrivePointer->getSteerAngle(wheelId);
}