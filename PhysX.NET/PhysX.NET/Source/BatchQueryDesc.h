#pragma once
#include "BatchQueryPreFilterShader.h"
#include "BatchQueryPostFilterShader.h"
#include "BatchQueryMemory.h"

namespace PhysX {
	public ref class BatchQueryDesc
	{
		public:
			BatchQueryDesc(UINT maxRaycastsPerExecute, UINT maxSweepsPerExecute, UINT maxOverlapsPerExecute);
			~BatchQueryDesc();
			!BatchQueryDesc();

		private:
			PxBatchQueryDesc* _nativeObject;

		public:
			property BatchQueryMemory^ QueryMemory;
			property BatchQueryPreFilterShader^ PreFilterShader;
			/// <summary>
			/// The custom postFilter shader to use for filtering.
			/// </summary>
			property BatchQueryPostFilterShader^ PostFilterShader;
	};
}