#pragma once

namespace PhysX {
	public ref class BatchQueryMemory
	{
        public:
          /*  property RaycastQueryResult* userRaycastResultBuffer;
            property RaycastHit* userRaycastTouchBuffer;
            property SweepQueryResult* userSweepResultBuffer;
            property SweepHit* userSweepTouchBuffer;
            property OverlapQueryResult* userOverlapResultBuffer;
            property OverlapHit* userOverlapTouchBuffer;*/
            property UINT RaycastTouchBufferSize;
            property UINT SweepTouchBufferSize;
            property UINT OverlapTouchBufferSize;
	};
}