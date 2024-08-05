#pragma once
#include "WheelQueryResult.h"

namespace PhysX
{
	public ref class VehicleWheelQueryResult
	{
		public:
			VehicleWheelQueryResult(array<WheelQueryResult^>^ wheelQueryResults);

		internal:
			static PxVehicleWheelQueryResult* ToUnmanaged(VehicleWheelQueryResult^ managed);

		public:
			property array<WheelQueryResult^>^ WheelQueryResults;
	};
}