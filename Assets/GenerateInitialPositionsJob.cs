using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public struct GenerateInitialPositionsJob : IJob
{
    public GenerateInitialPositionsJobData Data;
    
    public void Execute()
    {
        for (int i = 0; i < Data.EntitiesCount; i++)
        {
            // Seed the random generator with a unique value per index (e.g., using index and a large prime number)
            uint seed = (uint)((i+1) * 10007);
            Random random = new Random(seed);

            // Generate two random angles (in radians)
            float theta = random.NextFloat(0, 2 * math.PI); // Azimuthal angle: 0 to 2π
            float phi = random.NextFloat(-math.PI / 2, math.PI / 2); // Polar angle: -π/2 to π/2

            // Convert spherical coordinates to Cartesian coordinates
            float x = Data.SphereRadius * math.cos(phi) * math.cos(theta);
            float y = Data.SphereRadius * math.cos(phi) * math.sin(theta);
            float z = Data.SphereRadius * math.sin(phi);

            // Add the center coordinates
            float3 position = new float3(x, y, z) + Data.SphereCenter;

            // Store the random position in the output array
            Data.Positions[i] = position;
        }

    }
}