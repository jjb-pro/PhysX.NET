#include "StdAfx.h"
#include "VehicleGearsData.h"

VehicleGearsData::VehicleGearsData()
{
	FinalRatio = 4.0f;
	NumberOfRatios = 7;
	SwitchTime = 0.5f;
	this->Ratios = gcnew array<float>(PxVehicleGearsData::eGEARSRATIO_COUNT);
	Ratios[PxVehicleGearsData::eREVERSE] = -4.0f;
	Ratios[PxVehicleGearsData::eNEUTRAL] = 0.0f;
	Ratios[PxVehicleGearsData::eFIRST] = 4.0f;
	Ratios[PxVehicleGearsData::eSECOND] = 2.0f;
	Ratios[PxVehicleGearsData::eTHIRD] = 1.5f;
	Ratios[PxVehicleGearsData::eFOURTH] = 1.1f;
	Ratios[PxVehicleGearsData::eFIFTH] = 1.0f;

	for (PxU32 i = PxVehicleGearsData::eSIXTH; i < PxVehicleGearsData::eGEARSRATIO_COUNT; ++i)
		Ratios[i] = 0.f;
}

PxVehicleGearsData VehicleGearsData::ToUnmanaged(VehicleGearsData^ gearsData)
{
	ThrowIfNull(gearsData, "gearsData");

	if (gearsData->Ratios == nullptr)
		throw gcnew ArgumentException(String::Format("The Ratios property must be an instance of an array and of length {0}", MaximumNumberOfGearRatios));

	PxVehicleGearsData d;
		Util::AsUnmanagedArray(gearsData->Ratios, &d.mRatios, PxVehicleGearsData::eGEARSRATIO_COUNT);
		d.mFinalRatio = gearsData->FinalRatio;
		d.mNbRatios = gearsData->NumberOfRatios;
		d.mSwitchTime = gearsData->SwitchTime;

	return d;
}
VehicleGearsData^ VehicleGearsData::ToManaged(PxVehicleGearsData gearsData)
{
	VehicleGearsData^ d = gcnew VehicleGearsData();
		d->Ratios = Util::AsManagedArray<float>(&gearsData.mRatios, PxVehicleGearsData::eGEARSRATIO_COUNT);
		d->FinalRatio = gearsData.mFinalRatio;
		d->NumberOfRatios = gearsData.mNbRatios;
		d->SwitchTime = gearsData.mSwitchTime;

	return d;
}