#pragma once
#include "ConvexMesh.h"
#include "Physics.h"
#include "Cooking.h"
#include "ConvexMeshDesc.h"

namespace PhysX
{
	namespace CustomExtensions
	{
		public ref class CustomExtensions
		{
			public:
				static ConvexMesh^ CreateConvexMesh(Physics^ physics, Cooking^ cooking, ConvexMeshDesc^ description);
		};
	}
}