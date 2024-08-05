#include "stdafx.h"
#include "BatchQueryDesc.h"

PhysX::BatchQueryDesc::BatchQueryDesc(UINT maxRaycastsPerExecute, UINT maxSweepsPerExecute, UINT maxOverlapsPerExecute)
{
	_nativeObject = new PxBatchQueryDesc(maxRaycastsPerExecute, maxSweepsPerExecute, maxOverlapsPerExecute);
}

PhysX::BatchQueryDesc::~BatchQueryDesc()
{
	this->!BatchQueryDesc();
}

PhysX::BatchQueryDesc::!BatchQueryDesc()
{
	if (nullptr != _nativeObject)
	{
		delete _nativeObject;
		_nativeObject = nullptr;
	}
}