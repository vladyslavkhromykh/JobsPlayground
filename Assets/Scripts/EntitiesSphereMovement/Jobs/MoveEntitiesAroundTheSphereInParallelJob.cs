using Unity.Burst;
using Unity.Mathematics;
using UnityEngine.Jobs;

[BurstCompile]
public struct MoveEntitiesAroundTheSphereParallelJob : IJobParallelForTransform
{
    public MoveEntitiesAroundTheSphereData Data;

    public void Execute(int index, TransformAccess transform)
    {
        float3 position = transform.position;
        quaternion rotation = transform.rotation;

        // Move to pivot point
        float3 toPivot = position - Data.SphereCenter;

        // Apply rotation around the pivot point
        quaternion rot = quaternion.AxisAngle(math.normalize(Data.RotationAxis), Data.RotationSpeed * Data.DeltaTime);
        toPivot = math.mul(rot, toPivot);

        // Move back from pivot point
        float3 newPosition = Data.SphereCenter + toPivot;

        // Apply the rotation
        transform.position = newPosition;
        transform.rotation = math.mul(rot, rotation);
    }
}