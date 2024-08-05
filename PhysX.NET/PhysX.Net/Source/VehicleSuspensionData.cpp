#include "StdAfx.h"
#include "VehicleSuspensionData.h"

PxVehicleSuspensionData VehicleSuspensionData::ToUnmanaged(VehicleSuspensionData^ data)
{
	ThrowIfNull(data, "data");

	PxVehicleSuspensionData d;
		d.mSpringStrength = data->SpringStrength;
		d.mSpringDamperRate = data->SpringDamperRate;
		d.mMaxCompression = data->MaxCompression;
		d.mMaxDroop = data->MaxDroop;
		d.mSprungMass = data->SprungMass;
		d.mCamberAtRest = data->CamberAtRest;
		d.mCamberAtMaxDroop = data->CamberAtMaxDroop;
		d.mCamberAtMaxCompression = data->CamberAtMaxCompression;

	return d;
}
VehicleSuspensionData^ VehicleSuspensionData::ToManaged(PxVehicleSuspensionData data)
{
	VehicleSuspensionData^ d = gcnew VehicleSuspensionData();
		d->SpringStrength = data.mSpringStrength;
		d->SpringDamperRate = data.mSpringDamperRate;
		d->MaxCompression = data.mMaxCompression;
		d->MaxDroop = data.mMaxDroop;
		d->SprungMass = data.mSprungMass;
		d->CamberAtRest = data.mCamberAtRest;
		d->CamberAtMaxDroop = data.mCamberAtMaxDroop;
		d->CamberAtMaxCompression = data.mCamberAtMaxCompression;

	return d;
}