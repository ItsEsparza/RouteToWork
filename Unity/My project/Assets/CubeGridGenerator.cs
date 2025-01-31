using UnityEngine;

public class CubeGridGenerator : MonoBehaviour
{
    private const int GRID_SIZE = 100;
    private const float CUBE_SIZE = 0.95f;
    private const float SPACING = 1f;

    void Start()
    {
        for (int x = 0; x < GRID_SIZE; x++)
        {
            for (int y = 0; y < GRID_SIZE; y++)
            {
                Vector3 position = new Vector3(
                    x * SPACING - (GRID_SIZE * SPACING / 2), 
                    y * SPACING - (GRID_SIZE * SPACING / 2), 
                    0
                );

                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = position;
                cube.transform.localScale = new Vector3(CUBE_SIZE, CUBE_SIZE, CUBE_SIZE);
                cube.transform.parent = this.transform;
            }
        }
    }
}