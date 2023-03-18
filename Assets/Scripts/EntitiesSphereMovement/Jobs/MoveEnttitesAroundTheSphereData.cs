using Unity.Collections;
using Unity.Mathematics;

public struct MoveEntitiesAroundTheSphereData
{
    [ReadOnly] public float3 SphereCenter;
    [ReadOnly] public float SphereRadius;
    [ReadOnly] public float3 RotationAxis;
    [ReadOnly] public float RotationSpeed;
    [ReadOnly] public float DeltaTime;
}