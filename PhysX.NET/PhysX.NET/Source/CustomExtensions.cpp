#include "stdafx.h"
#include "CustomExtensions.h"

ConvexMesh^ PhysX::CustomExtensions::CustomExtensions::CreateConvexMesh(Physics^ physics, Cooking^ cooking, ConvexMeshDesc^ description)
{
    PxConvexMeshDesc convexDesc = ConvexMeshDesc::ToUnmanaged(description);
    PxConvexMesh* convexMesh = nullptr;
    PxDefaultMemoryOutputStream buf;

    if (cooking->UnmanagedPointer->cookConvexMesh(convexDesc, buf))
    {
        PxDefaultMemoryInputData id(buf.getData(), buf.getSize());
        convexMesh = physics->UnmanagedPointer->createConvexMesh(id);
    }

    return gcnew PhysX::ConvexMesh(convexMesh, physics);
}