#include "StdAfx.h"
#include "VehicleDrive4W.h"
#include "Physics.h"

VehicleDrive4W::VehicleDrive4W(PhysX::Physics^ physics, RigidDynamic^ actor, VehicleWheelsSimData^ wheelsData, VehicleDriveSimData4W^ driveData, int numNonDrivenWheels)
	: VehicleDrive(Create(physics, actor, wheelsData, driveData, numNonDrivenWheels), physics)
{
	_simData = gcnew VehicleDriveSimData4W(&this->UnmanagedPointer->mDriveSimData);
}

PxVehicleDrive4W* VehicleDrive4W::Create(PhysX::Physics^ physics, RigidDynamic^ actor, VehicleWheelsSimData^ wheelsData, VehicleDriveSimData4W^ driveData, int numNonDrivenWheels)
{
	ThrowIfNullOrDisposed(physics, "physics");
	ThrowIfNullOrDisposed(actor, "actor");
	ThrowIfNull(wheelsData, "wheelsData");
	ThrowIfNull(driveData, "driveData");

	PxVehicleWheelsSimData* wd = wheelsData->UnmanagedPointer;
	PxVehicleDriveSimData4W* dd = driveData->UnmanagedPointer;
	
	return PxVehicleDrive4W::create(physics->UnmanagedPointer, actor->UnmanagedPointer, *wd, *dd, numNonDrivenWheels);
}

void PhysX::VehicleDrive4W::SetToRestState()
{
	UnmanagedPointer->setToRestState();
}

void PhysX::VehicleDrive4W::SmoothDigitalRawInputsAndSetAnalogInputs(VehicleKeySmoothingData^ keySmoothingData, FixedSizeLookupTable^ steerVsForwardSpeedTable, VehicleDrive4WRawInputData^ rawInputData, float timestep, bool vehicleIsInAir)
{
	PxVehicleDrive4WSmoothDigitalRawInputsAndSetAnalogInputs(VehicleKeySmoothingData::ToUnmanaged(keySmoothingData), *FixedSizeLookupTable::ToUnmanaged(steerVsForwardSpeedTable), *rawInputData->UnmanagedPointer, timestep, vehicleIsInAir, *UnmanagedPointer);
}

void PhysX::VehicleDrive4W::SmoothAnalogRawInputsAndSetAnalogInputs(VehiclePadSmoothingData^ padSmoothingData, FixedSizeLookupTable^ steerVsForwardSpeedTable, VehicleDrive4WRawInputData^ rawInputData, float timestep, bool vehicleIsInAir)
{
	PxVehicleDrive4WSmoothAnalogRawInputsAndSetAnalogInputs(VehiclePadSmoothingData::ToUnmanaged(padSmoothingData), *FixedSizeLookupTable::ToUnmanaged(steerVsForwardSpeedTable), *rawInputData->UnmanagedPointer, timestep, vehicleIsInAir, *UnmanagedPointer);
}

VehicleDriveSimData4W^ VehicleDrive4W::DriveSimData::get()
{
	return _simData;
}

PxVehicleDrive4W* VehicleDrive4W::UnmanagedPointer::get()
{
	return (PxVehicleDrive4W*)VehicleDrive::UnmanagedPointer;
}