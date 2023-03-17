using Unity.Collections;
using Unity.Mathematics;

public struct GenerateInitialPositionsJobData
{
    [ReadOnly] public float3 SphereCenter;
    [ReadOnly] public float SphereRadius;
    [ReadOnly] public int EntitiesCount;
    [WriteOnly] public NativeArray<float3> Positions;
}