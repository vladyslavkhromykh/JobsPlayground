using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct GenerateMovementAxisJob : IJobParallelFor
{
    [WriteOnly]
    public NativeArray<float3> Axises;
    
    public void Execute(int index)
    {
        Random random = new Random((uint) index + 1);
        Axises[index] = random.NextFloat3(0.0f, 1.0f);
    }
}