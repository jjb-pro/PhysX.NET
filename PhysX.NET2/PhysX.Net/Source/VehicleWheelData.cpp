#include "StdAfx.h"
#include "VehicleWheelData.h"

PhysX::VehicleWheelData::VehicleWheelData()
{
	Radius = 0.0f; // must be filled out
	Width = 0.0f;
	Mass = 20.0f;
	MomentOfInertia = 0.0f; // must be filled out
	DampingRate = 0.25f;
	MaxBrakeTorque = 1500.0f;
	MaxHandBrakeTorque = 0.0f;
	MaxSteer = 0.0f;
	ToeAngle = 0.0f;
}

VehicleWheelData^ VehicleWheelData::ToManaged(PxVehicleWheelData wheelData)
{
	VehicleWheelData^ d = gcnew VehicleWheelData();
		d->_reciprocalRadius = wheelData.getRecipRadius();
		d->_recipMomentOfInertia = wheelData.getRecipMOI();
		d->Radius = wheelData.mRadius;
		d->Width = wheelData.mWidth;
		d->Mass = wheelData.mMass;
		d->MomentOfInertia = wheelData.mMOI;
		d->DampingRate = wheelData.mDampingRate;
		d->MaxBrakeTorque = wheelData.mMaxBrakeTorque;
		d->MaxHandBrakeTorque = wheelData.mMaxHandBrakeTorque;
		d->MaxSteer = wheelData.mMaxSteer;
		d->ToeAngle = wheelData.mToeAngle;

	return d;
}
PxVehicleWheelData VehicleWheelData::ToUnmanaged(VehicleWheelData^ wheelData)
{
	PxVehicleWheelData d;
		//d._reciprocalRadius = wheelData->getRecipRadius();
		//d._recipMomentOfInertia = wheelData->getRecipMOI();
		d.mRadius = wheelData->Radius;
		d.mWidth = wheelData->Width;
		d.mMass = wheelData->Mass;
		d.mMOI = wheelData->MomentOfInertia;
		d.mDampingRate = wheelData->DampingRate;
		d.mMaxBrakeTorque = wheelData->MaxBrakeTorque;
		d.mMaxHandBrakeTorque = wheelData->MaxHandBrakeTorque;
		d.mMaxSteer = wheelData->MaxSteer;
		d.mToeAngle = wheelData->ToeAngle;

	return d;
}

float VehicleWheelData::ReciprocalRadius::get()
{
	return _reciprocalRadius;
}

float VehicleWheelData::RecipMomentOfInertia::get()
{
	return _recipMomentOfInertia;
}