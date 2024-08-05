#include "stdafx.h"
#include "VehicleDrivableSurfaceToTireFrictionPairs.h"
#include "Material.h"
#include "VehicleEnum.h"

PhysX::VehicleDrivableSurfaceToTireFrictionPairs::VehicleDrivableSurfaceToTireFrictionPairs(const PxU32 maxNumberTireTypes, const PxU32 maxNumberSurfaceTypes)
{
	_tireFrictionPairs = PxVehicleDrivableSurfaceToTireFrictionPairs::allocate(maxNumberTireTypes, maxNumberSurfaceTypes);
}

PhysX::VehicleDrivableSurfaceToTireFrictionPairs::~VehicleDrivableSurfaceToTireFrictionPairs()
{
	this->!VehicleDrivableSurfaceToTireFrictionPairs();
}

PhysX::VehicleDrivableSurfaceToTireFrictionPairs::!VehicleDrivableSurfaceToTireFrictionPairs()
{
	if (nullptr != _tireFrictionPairs) {
		_tireFrictionPairs->release();
		_tireFrictionPairs = nullptr;
	}
}

void PhysX::VehicleDrivableSurfaceToTireFrictionPairs::SetUp(PxU32 numberTireTypes, PxU32 numberSurfaceTypes, array<Material^>^ drivableSurfaceMaterials, array<UINT>^ drivableSurfaceType)
{
	const PxMaterial** v = new const PxMaterial * [drivableSurfaceMaterials->Length];
	for (int i = 0; i < drivableSurfaceMaterials->Length; i++)
	{
		v[i] = drivableSurfaceMaterials[i]->UnmanagedPointer;
	}

	// ToDo: is this right?
	PxVehicleDrivableSurfaceType* r = new PxVehicleDrivableSurfaceType[drivableSurfaceType->Length];
	for (int i = 0; i < drivableSurfaceType->Length; i++)
	{
		r[i].mType = drivableSurfaceType[i];
	}

	_tireFrictionPairs->setup(numberTireTypes, numberSurfaceTypes, v, r);

	delete[] v, r;
}

void PhysX::VehicleDrivableSurfaceToTireFrictionPairs::SetTypePairFriction(PxU32 surfaceType, PxU32 tireType, PxReal value)
{
	_tireFrictionPairs->setTypePairFriction(surfaceType, tireType, value);
}

PxVehicleDrivableSurfaceToTireFrictionPairs* PhysX::VehicleDrivableSurfaceToTireFrictionPairs::UnmanagedPointer::get()
{
	return _tireFrictionPairs;
}