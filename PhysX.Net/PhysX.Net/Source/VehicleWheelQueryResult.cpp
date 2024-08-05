#include "stdafx.h"
#include "VehicleWheelQueryResult.h"

PhysX::VehicleWheelQueryResult::VehicleWheelQueryResult(array<WheelQueryResult^>^ wheelQueryResults)
{
	ThrowIfNull(wheelQueryResults, "wheelQueryResults");
	WheelQueryResults = wheelQueryResults;
}

PxVehicleWheelQueryResult* PhysX::VehicleWheelQueryResult::ToUnmanaged(VehicleWheelQueryResult^ managed)
{
	PxVehicleWheelQueryResult* unmanaged = new PxVehicleWheelQueryResult();

	PxWheelQueryResult* r = new PxWheelQueryResult[managed->WheelQueryResults->Length];

	unmanaged->nbWheelQueryResults = managed->WheelQueryResults->Length;
	unmanaged->wheelQueryResults = r;

	return unmanaged;
}