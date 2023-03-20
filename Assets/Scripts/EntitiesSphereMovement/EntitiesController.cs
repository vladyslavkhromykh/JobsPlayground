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

    public NativeArray<float3> Axises;

    private Transform[] entities;
    private JobHandle jobHandle;
    
    private void Awake()
    {
        entities = SpawnEntities();
    }

    private void Start()
    {
        Axises = new NativeArray<float3>(EntitiesCount, Allocator.Persistent);
        GenerateMovementAxisJob generateMovementAxis = new GenerateMovementAxisJob
        {
            Axises = Axises
        };
        JobHandle generateMovementAxisHandle = generateMovementAxis.Schedule(EntitiesCount, 0);
        generateMovementAxisHandle.Complete();
        
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
        
        jobHandle = job.Schedule(EntitiesCount, 0, generateMovementAxisHandle);
        jobHandle.Complete();

        for (int i = 0; i < positions.Length; i++)
        {
            entities[i].transform.position = positions[i];
        }

        positions.Dispose();
    }

    private void OnDestroy()
    {
        Axises.Dispose();
    }
    
    private void Update()
    {
        TransformAccessArray transformAccessArray = new TransformAccessArray(EntitiesCount);
        transformAccessArray.SetTransforms(entities);

        MoveEntitiesAroundTheSphereParallelJob job = new MoveEntitiesAroundTheSphereParallelJob
        {
            Data = new MoveEntitiesAroundTheSphereData
            {
                Axises = Axises,
                DeltaTime = Time.deltaTime,
                RotationSpeed = 1.0f,
                SphereCenter = SphereCenter
            }
        };

        JobHandle handle = job.Schedule(transformAccessArray, jobHandle);
        handle.Complete();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(SphereCenter, SphereRadius);
    }

    public Transform[] SpawnEntities()
    {
        List<GameObject> entities = new List<GameObject>();
        for (int i = 0; i < EntitiesCount; i++)
        {
            entities.Add(Instantiate(EntityPrefab, transform));
        }

        return entities.Select((entity) => entity.transform).ToArray();
    }
}