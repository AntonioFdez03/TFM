using UnityEngine;

[RequireComponent(typeof(Terrain))]
[ExecuteAlways]
public class RegionTerrainGenerator : MonoBehaviour
{
    [Header("Large Regions")]
    public float regionScale = 3f;     // Muy bajo = regiones enormes
    [Range(0f, 1f)]
    public float threshold = 0.75f;
    [Range(0.2f, 0.45f)]
    public float transitionWidth = 0.35f;

    [Header("Elevation")]
    public float maxHeight = 3f;

    [Header("Height Variation Inside Elevated Areas")]
    public float elevationDetailScale = 6f;   // Bajo = variaciones amplias
    public float elevationVariation = 0.6f;   // Qué tanto cambia la altura

    [Header("Seed")]
    public int seed = 1234;

    public bool autoUpdate = true;

    void OnValidate()
    {
        if (autoUpdate)
            GenerateTerrain();
    }

    public void GenerateTerrain()
    {
        Terrain terrain = GetComponent<Terrain>();
        TerrainData terrainData = terrain.terrainData;

        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;

        float[,] heights = new float[width, height];

        float offsetX = seed * 0.01f;
        float offsetY = seed * 0.02f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xNorm = (float)x / width;
                float yNorm = (float)y / height;

                // Ruido grande para regiones
                float regionNoise = Mathf.PerlinNoise(
                    xNorm * regionScale + offsetX,
                    yNorm * regionScale + offsetY
                );

                // Máscara suave
                float regionMask = Mathf.InverseLerp(
                    threshold - transitionWidth,
                    threshold + transitionWidth,
                    regionNoise
                );

                // Ruido interno para variar altura en zonas elevadas
                float elevationNoise = Mathf.PerlinNoise(
                    xNorm * elevationDetailScale + offsetX,
                    yNorm * elevationDetailScale + offsetY
                );

                float variedHeight = Mathf.Lerp(
                    0f,
                    maxHeight,
                    elevationNoise * elevationVariation
                );

                // Combinar región con variación
                float finalHeight = regionMask * variedHeight;

                heights[x, y] = finalHeight / terrainData.size.y;
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }
}