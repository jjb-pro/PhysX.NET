#pragma once
#include "InternalVehicleSceneQueryData.h"
#include "Foundation.h"
#include "Scene.h"
#include "BatchQuery.h"
#include "RaycastQueryResult.h"
#include "VehicleWheels.h"

namespace PhysX {
	PxQueryHitType::Enum WheelSceneQueryPreFilterBlocking
	(PxFilterData filterData0, PxFilterData filterData1,
		const void* constantBlock, PxU32 constantBlockSize,
		PxHitFlags& queryFlags)
	{
		//filterData0 is the vehicle suspension query.
		//filterData1 is the shape potentially hit by the query.
		PX_UNUSED(filterData0);
		PX_UNUSED(constantBlock);
		PX_UNUSED(constantBlockSize);
		PX_UNUSED(queryFlags);
		return ((0 == (filterData1.word3 & 0xffff0000)) ? PxQueryHitType::eNONE : PxQueryHitType::eBLOCK);
	}

	public ref class VehicleSceneQueryData
	{
		public:
			VehicleSceneQueryData(Foundation^ foundation, const UINT maxNumVehicles, const UINT maxNumWheelsPerVehicle, const UINT maxNumHitPointsPerWheel, const UINT numVehiclesInBatch);
			
			BatchQuery^ SetUpBatchedSceneQuery(Scene^ scene, const UINT batchId);

			RaycastQueryResult^ InvokeSuspensionRaycasts(BatchQuery^ batchQuery, array<VehicleWheels^>^ vehicles);

		private:
			InternalVehicleSceneQueryData* _nativeObject;
	};
}