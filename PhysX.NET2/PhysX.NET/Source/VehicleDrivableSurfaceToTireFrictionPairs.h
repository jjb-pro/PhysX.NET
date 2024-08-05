#pragma once

#include "Material.h"
#include "VehicleEnum.h"

namespace PhysX
{
	public ref class VehicleDrivableSurfaceToTireFrictionPairs
	{
	public:
		VehicleDrivableSurfaceToTireFrictionPairs(const PxU32 MAX_NUM_TIRE_TYPES, const PxU32 MAX_NUM_SURFACE_TYPES);
		~VehicleDrivableSurfaceToTireFrictionPairs();
		!VehicleDrivableSurfaceToTireFrictionPairs();

		void SetUp(PxU32 numberTireTypes, PxU32 numberSurfaceTypes, array<Material^>^ drivableSurfaceMaterials, array<UINT>^ driveableSurfaceType);

		/// <summary>
		/// Set the friction for a specified pair of tire type and drivable surface type.
		/// </summary>
		/// <param name="surfaceType">Describes the surface type.</param>
		/// <param name="tireType">Describes the tire type.</param>
		/// <param name="value">Describes the friction coefficient for the combination of surface type and tire type.</param>
		void SetTypePairFriction(PxU32 surfaceType, PxU32 tireType, PxReal value);

	private:
		PxVehicleDrivableSurfaceToTireFrictionPairs* _tireFrictionPairs;

	internal:
		property PxVehicleDrivableSurfaceToTireFrictionPairs* UnmanagedPointer
		{
			PxVehicleDrivableSurfaceToTireFrictionPairs* get();
		}
	};
}