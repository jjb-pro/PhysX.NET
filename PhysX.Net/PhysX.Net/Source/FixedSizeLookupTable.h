#pragma once
#include "common\PxCoreUtilityTypes.h"

namespace PhysX
{
	ref struct FixedSizeLookupTablePair;

	/// <summary>
	/// A lookup table that can hold eight pairs.
	/// </summary>
	public ref class FixedSizeLookupTable
	{
		public:
			FixedSizeLookupTable(array<float>^ pairs);

		internal:
			static PxFixedSizeLookupTable<8>* ToUnmanaged(FixedSizeLookupTable^ lookupTable);

		private:
			array<float>^ _pairs;
	};
};