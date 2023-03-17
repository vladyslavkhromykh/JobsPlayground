using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.Serialization;

public class EntitiesController : MonoBehaviour
{
    public GameObject EntityPrefab;
    
    public float3 SphereCenter;
    [Range(10.0f, 100.0f)] public float SphereRadius;

    [Range(1, 100000)] public int EntitiesCount;

    private async UniTaskVoid Start()
    {
        await MoveEntitiesAroundTheSphereAsync(await SpawnAtInitialPositionsAsync(await SpawnEntitiesAsync())) ;
    }

    private async UniTask<List<GameObject>> SpawnAtInitialPositionsAsync(List<GameObject> entities)
    {
        NativeArray<float3> positions = new NativeArray<float3>(EntitiesCount, Allocator.TempJob);
        GenerateInitialPositionsJobParallel job = new GenerateInitialPositionsJobParallel
        {
            Data = new GenerateInitialPositionsJobData
            {
                SphereCenter = SphereCenter,
                SphereRadius = SphereRadius,
                EntitiesCount = EntitiesCount,
                Positions = positions
            }
        };
        JobHandle jobHandle = job.Schedule(EntitiesCount, 0);
        await UniTask.WaitUntil(() => jobHandle.IsCompleted);
        jobHandle.Complete();

        for (int i = 0; i < positions.Length; i++)
        {
            entities[i].transform.position = positions[i];
        }

        positions.Dispose();

        return await UniTask.FromResult(entities);
    }

    private async UniTask MoveEntitiesAroundTheSphereAsync(List<GameObject> entities)
    {
        TransformAccessArray transformAccessArray = new TransformAccessArray(entities.Count);
        transformAccessArray.SetTransforms(entities.Select((entity) => entity.transform).ToArray());
        
        while (true)
        {
            MoveEntitiesAroundTheSphereParallelJob job = new MoveEntitiesAroundTheSphereParallelJob
            {
                Data = new MoveEntitiesAroundTheSphereData
                {
                    DeltaTime = Time.deltaTime,
                    RotationAxis = new float3(1, 1, 0),
                    RotationSpeed = 1.0f,
                    SphereCenter = SphereCenter,
                    SphereRadius = SphereRadius
                }
            };

            JobHandle jobHandle = job.Schedule(transformAccessArray);
            jobHandle.Complete();
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(SphereCenter, SphereRadius);
    }

    public async UniTask<List<GameObject>> SpawnEntitiesAsync()
    {
        List<GameObject> entities = new List<GameObject>();
        for (int i = 0; i < EntitiesCount; i++)
        {
            entities.Add(SpawnEntity());
        }

        return await UniTask.FromResult(entities);
    }

    public GameObject SpawnEntity()
    {
        return Instantiate(EntityPrefab, transform);
    }
}