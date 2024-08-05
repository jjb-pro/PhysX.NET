#include "StdAfx.h"
#include "VehicleSDK.h"
#include "Physics.h"
#include "VehicleSDKNotInitializedException.h"
#include "VehicleWheelsSimData.h"
#include "VehicleDriveSimData4W.h"
#include "VehicleWheels.h"
#include "RaycastQueryResult.h"
#include "BatchQuery.h"
#include "VehicleWheelsDynData.h"
#include "VehicleConcurrentUpdateData.h"

VehicleSDK::VehicleSDK(PhysX::Physics^ physics)
{
	ThrowIfNullOrDisposed(physics, "physics");

	_physics = physics;

	ObjectTable::Instance->AddObjectOwner(this, physics);
}
VehicleSDK::~VehicleSDK()
{
	this->!VehicleSDK();
}
VehicleSDK::!VehicleSDK()
{
	if (this->Disposed)
		return;

	Close();

	_physics = nullptr;
}
bool VehicleSDK::Disposed::get()
{
	return (_physics == nullptr);
}

void VehicleSDK::EnsureInitalized()
{
	if (!_initalized)
		throw gcnew VehicleSDKNotInitializedException("The vehicle SDK has not been initalized. Call Physics::VehicleSDK::Initalize() before any vehicle related methods.");
}

bool VehicleSDK::Initalize()
{
	bool initalized = PxInitVehicleSDK(*_physics->UnmanagedPointer);

	if (initalized)
		_initalized = true;

	return initalized;
}
void VehicleSDK::Close()
{
	if (_initalized)
		PxCloseVehicleSDK();
}

void VehicleSDK::Vehicle4WEnable3WDeltaMode(VehicleWheelsSimData^ suspensionWheelTireData, VehicleWheelsDynData^ wheelsDynData, VehicleDriveSimData4W^ coreData)
{
	throw gcnew NotImplementedException();
	//PxVehicle4WEnable3WDeltaMode(
	//	*suspensionWheelTireData->UnmanagedPointer, 
	//	*coreData->UnmanagedPointer);
}

void VehicleSDK::Vehicle4WEnable3WTadpoleMode(VehicleWheelsSimData^ suspWheelTireData, VehicleWheelsDynData^ wheelsDynData, VehicleDriveSimData4W^ coreData)
{
	throw gcnew NotImplementedException();
	//PxVehicle4WEnable3WTadpoleMode(*suspWheelTireData->UnmanagedPointer, wheelsDynData, *coreData->UnmanagedPointer);
}

void VehicleSDK::VehicleComputeTireForceDefault(float tireFriction, float longSlip, float latSlip, float camber,
	float wheelOmega, float wheelRadius, float recipWheelRadius, float restTireLoad, float normalisedTireLoad,
	float tireLoad, float gravity, float recipGravity, float wheelTorque, float tireLongForceMag,
	float tireLatForceMag, float tireAlignMoment)
{
	throw gcnew NotImplementedException();
}

array<float>^ PhysX::VehicleSDK::VehicleComputeSprungMasses(array<Vector3>^ sprungMassCoordinates, Vector3 centreOfMass, float totalMass, UINT gravityDirection)
{
	ThrowIfNull(sprungMassCoordinates, "sprungMassCoordinates");
	//ThrowIfNull(sprungMasses, "sprungMasses");

	PxVec3* v = new PxVec3[sprungMassCoordinates->Length];
	for (int i = 0; i < sprungMassCoordinates->Length; i++)
	{
		v[i] = UV(sprungMassCoordinates[i]);
	}

	PxReal* r = new PxReal[sprungMassCoordinates->Length];
	//for (int i = 0; i < sprungMasses->Length; i++)
	//{
	//	r[i] = sprungMasses[i];
	//}

	PxVehicleComputeSprungMasses(sprungMassCoordinates->Length, v, UV(centreOfMass), totalMass, gravityDirection, r);

	array<float>^ result = gcnew array<float>(sprungMassCoordinates->Length);
	for (int i = 0; i < sprungMassCoordinates->Length; i++) {
		result[i] = r[i];
	}

	// clean up the unmanaged memory
	delete[] r;

	return result;
}

void VehicleSDK::VehicleSetBasisVectors(Vector3 up, Vector3 forward)
{
	PxVehicleSetBasisVectors(UV(up), UV(forward));
}

void VehicleSDK::VehicleSuspensionRaycasts(BatchQuery^ batchQuery, array<VehicleWheels^>^ vehicles, array<RaycastQueryResult^>^ sceneQueryResults)
{
	ThrowIfNull(batchQuery, "batchQuery");
	ThrowIfNull(vehicles, "vehicles");
	ThrowIfNull(sceneQueryResults, "sceneQueryResults");

	PxVehicleWheels** v = new PxVehicleWheels*[vehicles->Length];
	for (int i = 0; i < vehicles->Length; i++)
	{
		v[i] = vehicles[i]->UnmanagedPointer;
	}

	PxRaycastQueryResult* r = new PxRaycastQueryResult[sceneQueryResults->Length];
	for (int i = 0; i < sceneQueryResults->Length; i++)
	{
		PxRaycastQueryResult result = RaycastQueryResult::ToUnmanaged(sceneQueryResults[i]);

		r[i] = result;
	}

	PxVehicleSuspensionRaycasts(batchQuery->UnmanagedPointer, vehicles->Length, v, sceneQueryResults->Length, r);

	//

	for (int i = 0; i < sceneQueryResults->Length; i++)
	{
		delete r[i].touches;
	}

	delete[] r;
	delete[] v;
}

array<VehicleWheelQueryResult^>^ PhysX::VehicleSDK::VehicleUpdates(float timestep, Vector3 gravity, VehicleDrivableSurfaceToTireFrictionPairs^ vehicleDrivableSurfaceToTireFrictionPairs, array<VehicleWheels^>^ vehicles)
{
	PxVehicleWheels** r = new PxVehicleWheels * [vehicles->Length];
	PxVehicleWheelQueryResult* vehicleQueryResults = new PxVehicleWheelQueryResult[vehicles->Length];
	for (int i = 0; i < vehicles->Length; i++)
	{
		r[i] = vehicles[i]->UnmanagedPointer;
		vehicleQueryResults[i] = PxVehicleWheelQueryResult();
		vehicleQueryResults[i].wheelQueryResults = new PxWheelQueryResult[PX_MAX_NB_WHEELS];
		vehicleQueryResults[i].nbWheelQueryResults = r[i]->mWheelsSimData.getNbWheels();
	}

	PxVehicleUpdates(timestep, UV(gravity), *vehicleDrivableSurfaceToTireFrictionPairs->UnmanagedPointer, vehicles->Length, r, vehicleQueryResults);

	delete[] r;

	array<VehicleWheelQueryResult^>^ managedVehicleQueryResults = gcnew array<VehicleWheelQueryResult^>(vehicles->Length);
	for (int i = 0; i < vehicles->Length; i++)
	{
		array<WheelQueryResult^>^ wheelQueryResults = gcnew array<WheelQueryResult^>(PX_MAX_NB_WHEELS);
		for (int j = 0; j < PX_MAX_NB_WHEELS; j++)
		{
			wheelQueryResults[j] = WheelQueryResult::ToManaged(&vehicleQueryResults[i].wheelQueryResults[j]);
		}

		managedVehicleQueryResults[i] = gcnew VehicleWheelQueryResult(wheelQueryResults);
	}

	return managedVehicleQueryResults;
}

void PhysX::VehicleSDK::VehicleSetUpdateMode(VehicleUpdateMode updateMode)
{
	PxVehicleSetUpdateMode(static_cast<PxVehicleUpdateMode::Enum>(updateMode));
}

//void VehicleSDK::VehiclePostUpdates(VehicleConcurrentUpdateData^ concurrentUpdateData, array<VehicleWheels^>^ vehicles)
//{
//
//}

bool PhysX::VehicleSDK::VehicleIsInAir(VehicleWheelQueryResult^ vehicleWheelQueryResult)
{
	return PxVehicleIsInAir(*VehicleWheelQueryResult::ToUnmanaged(vehicleWheelQueryResult));
}

VehicleWheels^ VehicleSDK::VehicleUpdateCMassLocalPose(Matrix oldCMassLocalPose, Matrix newCMassLocalPose, VehicleGravityDirection gravityDirection, VehicleWheels^ vehicle)
{
	ThrowIfNullOrDisposed(vehicle, "vehicle");

	PxVehicleWheels* w = vehicle->UnmanagedPointer;

	PxVehicleUpdateCMassLocalPose
	(
		MathUtil::MatrixToPxTransform(oldCMassLocalPose),
		MathUtil::MatrixToPxTransform(newCMassLocalPose),
		(int)gravityDirection,
		w
	);

	// The fourth parameter is [in, out], se we'll find the managed object it represents
	auto outWheel = ObjectTable::Instance->GetObject<VehicleWheels^>((intptr_t)w);

	return outWheel;
}

//

PhysX::Physics^ VehicleSDK::Physics::get()
{
	return _physics;
}

UINT PhysX::VehicleSDK::MaxWheelNumber::get()
{
	return PX_MAX_NB_WHEELS;
}