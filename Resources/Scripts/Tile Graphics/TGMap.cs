using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class TGMap : MonoBehaviour
{
	public static int _mapSizeWidth = 0;
	public static int _mapSizeHeight = 0;
    public int mapSizeWidth = 100;
	public int mapSizeHeight = 50;
	public int seed = 10;
	[Range(0f, 1f)]
	public float noiseFactor = 0.725f;

    public float tileSize = 1.0f;

    public Texture2D terrainTiles;
    public int tileResolution;

    public int minRoomSize = 4;
    public int maxRoomSize = 20;
    public int totalRooms = 20;

	void Awake()
	{
		_mapSizeWidth = mapSizeWidth;
		_mapSizeHeight = mapSizeHeight;
	}

    void Start()
    {
        BuildMesh();
    }

    // get the tile sprite sheet, chop it up and throw it into the array
    Color[][] ChopUpTiles()
    {
        int numTilesPerRow = terrainTiles.width / tileResolution;
        int numRows = terrainTiles.height / tileResolution;

        Color[][] tiles = new Color[numTilesPerRow * numRows][];

        for (int y = 0; y < numRows; y++)
        {
            for (int x = 0; x < numTilesPerRow; x++)
            {
                tiles[y * numTilesPerRow + x] = terrainTiles.GetPixels(x * tileResolution, y * tileResolution, tileResolution, tileResolution);
            }
        }

        return tiles;
    }

    // create the map texture to be thrown into the UV
    void BuildTexture()
    {
		TDMap map = new TDMap(mapSizeWidth, mapSizeHeight, minRoomSize, maxRoomSize, totalRooms, seed, noiseFactor);

        int textWidth = mapSizeWidth * tileResolution;
        int textHeight = mapSizeHeight * tileResolution;
        Texture2D texture = new Texture2D(textWidth, textHeight);

        Color[][] tiles = ChopUpTiles();

        for (int y = 0; y < mapSizeHeight; y++)
        {
            for (int x = 0; x < mapSizeWidth; x++)
            {
                Color[] p = tiles[ map.GetTileAt(x, y) ];
                texture.SetPixels(x * tileResolution, y * tileResolution, tileResolution, tileResolution, p);
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        MeshRenderer mesh_renderer = GetComponent<MeshRenderer>();
        mesh_renderer.sharedMaterials[0].mainTexture = texture;

        Debug.Log("Done Texture.");
    }

    // create the mesh and all UV 
    public void BuildMesh()
    {
        // Declare mesh points
        int numTiles = mapSizeWidth * mapSizeHeight;
        int numTriangles = numTiles * 2;
        int vertSizeWidth = mapSizeWidth + 1;
        int vertSizeHeight = mapSizeHeight + 1;
        int numVerts = vertSizeWidth * vertSizeHeight;

        // Generate mesh data
        Vector3[] vertices = new Vector3[numVerts];
        Vector3[] normals = new Vector3[numVerts];
        Vector2[] uv = new Vector2[numVerts];

        int[] triangles = new int[numTriangles * 3];

        int x, z;
        for (z = 0; z < mapSizeHeight; z++)
        {
            for (x = 0; x < mapSizeWidth; x++)
            {
                vertices[z * vertSizeWidth + x] = new Vector3(x * tileSize, 0, z * tileSize);
                normals[z * vertSizeWidth + x] = Vector3.up;
                uv[z * vertSizeWidth + x] = new Vector2((float)x / mapSizeWidth, (float)z / mapSizeHeight); // add a '1f -' to the z / mapsize if faces are flipped
                // x = 0, uv.x = 0
                // x=101, uv.x = 1
                // uv.x = (float)x / vertSizeWidth
            }
        }

        Debug.Log("Done with Verts.");

        for (z = 0; z < mapSizeHeight; z++)
        {
            for (x = 0; x < mapSizeWidth; x++)
            {
                int squareIndex = z * mapSizeWidth + x;
                int triOffset = squareIndex * 6;
                triangles[triOffset + 0] = z * vertSizeWidth + x +                  0;
                triangles[triOffset + 1] = z * vertSizeWidth + x + vertSizeWidth +  0;
                triangles[triOffset + 2] = z * vertSizeWidth + x + vertSizeWidth +  1;

                triangles[triOffset + 3] = z * vertSizeWidth + x +                  0;
                triangles[triOffset + 4] = z * vertSizeWidth + x + vertSizeWidth +  1;
                triangles[triOffset + 5] = z * vertSizeWidth + x +                  1;
            }
        }

        Debug.Log("Done with triangles.");

        // Create new Mesh and populate
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv; // MAKE SURE YOU SET THE GOD DAMNED UV HOLY CRAP THIS WAS DRIVING ME CRAZY

        // Assign mesh to:
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        BuildTexture();
    }

}
