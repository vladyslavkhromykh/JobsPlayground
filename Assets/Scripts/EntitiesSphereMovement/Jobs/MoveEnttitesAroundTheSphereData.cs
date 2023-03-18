using Unity.Collections;
using Unity.Mathematics;

public struct MoveEntitiesAroundTheSphereData
{
    [ReadOnly] public NativeArray<float3> Axises;
    [ReadOnly] public float3 SphereCenter;
    [ReadOnly] public float RotationSpeed;
    [ReadOnly] public float DeltaTime;
}