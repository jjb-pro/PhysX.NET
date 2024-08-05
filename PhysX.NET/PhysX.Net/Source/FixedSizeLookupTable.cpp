#include "StdAfx.h"
#include "FixedSizeLookupTable.h"

PhysX::FixedSizeLookupTable::FixedSizeLookupTable(array<float>^ pairs)
{
	if (pairs->Length != 2 * 8)
		throw gcnew ArgumentException("The lookup table has a fixed size of 8 pairs (16 elemments in total).", "pairs");

	_pairs = pairs;
}

PxFixedSizeLookupTable<8>* PhysX::FixedSizeLookupTable::ToUnmanaged(FixedSizeLookupTable^ lookupTable)
{
	float* p = new float[lookupTable->_pairs->Length];
	Util::AsUnmanagedArray(lookupTable->_pairs, p, lookupTable->_pairs->Length);
	
	PxFixedSizeLookupTable<8> gSteerVsForwardSpeedTable(p, 4);

	delete[] p;

	return &gSteerVsForwardSpeedTable;
}