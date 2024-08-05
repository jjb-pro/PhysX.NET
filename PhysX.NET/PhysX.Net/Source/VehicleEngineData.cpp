#include "StdAfx.h"
#include "VehicleEngineData.h"

PhysX::VehicleEngineData::VehicleEngineData()
{
	MomentOfInertia = 1.0f;
	PeakTorque = 500.0f;
	MaxOmega = 600.0f;
	DampingRateFullThrottle = 0.15f;
	DampingRateZeroThrottleClutchEngaged = 2.0f;
	DampingRateZeroThrottleClutchDisengaged = 0.35f;
	
	//mTorqueCurve.addPair(0.0f, 0.8f);
	//mTorqueCurve.addPair(0.33f, 1.0f);
	//mTorqueCurve.addPair(1.0f, 0.8f);
}

PxVehicleEngineData VehicleEngineData::ToUnmanaged(VehicleEngineData^ data)
{
	ThrowIfNull(data, "data");

	PxVehicleEngineData d;
		d.mMOI = data->MomentOfInertia;
		d.mPeakTorque = data->PeakTorque;
		d.mMaxOmega = data->MaxOmega;
		d.mDampingRateFullThrottle = data->DampingRateFullThrottle;
		d.mDampingRateZeroThrottleClutchEngaged = data->DampingRateZeroThrottleClutchEngaged;
		d.mDampingRateZeroThrottleClutchDisengaged = data->DampingRateZeroThrottleClutchDisengaged;

	return d;
}
VehicleEngineData^ VehicleEngineData::ToManaged(PxVehicleEngineData data)
{
	auto d = gcnew VehicleEngineData();
		d->MomentOfInertia = data.mMOI;
		d->PeakTorque = data.mPeakTorque;
		d->MaxOmega = data.mMaxOmega;
		d->DampingRateFullThrottle = data.mDampingRateFullThrottle;
		d->DampingRateZeroThrottleClutchEngaged = data.mDampingRateZeroThrottleClutchEngaged;
		d->DampingRateZeroThrottleClutchDisengaged = data.mDampingRateZeroThrottleClutchDisengaged;

	return d;
}