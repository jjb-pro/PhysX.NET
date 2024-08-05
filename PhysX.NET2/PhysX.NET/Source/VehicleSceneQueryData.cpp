#include "StdAfx.h"
#include "VehicleSceneQueryData.h"

PhysX::VehicleSceneQueryData::VehicleSceneQueryData(Foundation^ foundation, const UINT maxNumVehicles, const UINT maxNumWheelsPerVehicle, const UINT maxNumHitPointsPerWheel, const UINT numVehiclesInBatch)
{
	_nativeObject = InternalVehicleSceneQueryData::allocate(maxNumVehicles, maxNumWheelsPerVehicle, maxNumHitPointsPerWheel, numVehiclesInBatch, WheelSceneQueryPreFilterBlocking, NULL, foundation->UnmanagedPointer->getAllocatorCallback());
}

BatchQuery^ PhysX::VehicleSceneQueryData::SetUpBatchedSceneQuery(Scene^ scene, const UINT batchId)
{
	return gcnew BatchQuery(InternalVehicleSceneQueryData::setUpBatchedSceneQuery(batchId, *_nativeObject, scene->UnmanagedPointer));
}

RaycastQueryResult^ PhysX::VehicleSceneQueryData::InvokeSuspensionRaycasts(BatchQuery^ batchQuery, array<VehicleWheels^>^ vehicles)
{
	PxRaycastQueryResult* raycastResults = _nativeObject->getRaycastQueryResultBuffer(0);
	UINT raycastResultsSize = _nativeObject->getQueryResultBufferSize();

	PxVehicleWheels** r = new PxVehicleWheels * [vehicles->Length];
	for (int i = 0; i < vehicles->Length; i++)
	{
		r[i] = vehicles[i]->UnmanagedPointer;
	}

	PxVehicleSuspensionRaycasts(batchQuery->UnmanagedPointer, vehicles->Length, r, raycastResultsSize, raycastResults);

	delete[] r;

	return RaycastQueryResult::ToManaged(*raycastResults);
}