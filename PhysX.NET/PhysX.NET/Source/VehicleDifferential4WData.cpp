#include "StdAfx.h"
#include "VehicleDifferential4WData.h"

PhysX::VehicleDifferential4WData::VehicleDifferential4WData()
{
	FrontRearSplit = 0.45f;
	FrontLeftRightSplit = 0.5f;
	RearLeftRightSplit = 0.5f;
	CentreBias = 1.3f;
	FrontBias = 1.3f;
	RearBias = 1.3f;
	Type = VehicleDifferentialType::LimitedSlip4WheelDrive;
}

PxVehicleDifferential4WData VehicleDifferential4WData::ToUnmanaged(VehicleDifferential4WData^ data)
{
	PxVehicleDifferential4WData d;
		d.mFrontRearSplit = data->FrontRearSplit;
		d.mFrontLeftRightSplit = data->FrontLeftRightSplit;
		d.mRearLeftRightSplit = data->RearLeftRightSplit;
		d.mCentreBias = data->CentreBias;
		d.mFrontBias = data->FrontBias;
		d.mRearBias = data->RearBias;
		d.mType = ToUnmanagedEnum(PxVehicleDifferential4WData, data->Type);
	
	return d;
}
VehicleDifferential4WData^ VehicleDifferential4WData::ToManaged(PxVehicleDifferential4WData data)
{
	VehicleDifferential4WData^ d = gcnew VehicleDifferential4WData();
		d->FrontRearSplit = data.mFrontRearSplit;
		d->FrontLeftRightSplit = data.mFrontLeftRightSplit;
		d->RearLeftRightSplit = data.mRearLeftRightSplit;
		d->CentreBias = data.mCentreBias;
		d->FrontBias = data.mFrontBias;
		d->RearBias = data.mRearBias;
		d->Type = ToManagedEnum(VehicleDifferentialType, data.mType);

	return d;
}