using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public static MapGenerator mapGenerator;

	public GameObject chunkPrefab;

	private static Dictionary<int, Dictionary<int, GameObject>> map;

	private int[,] grid = new int[128, 128], renderGrid = new int[128, 128];

	void Start () {
		mapGenerator = this; // there should only be one mapGenerator existing at a time
		map = new Dictionary<int, Dictionary<int, GameObject>>();
		GenerateChunks();
		ReconstructMesh();

		GenerateGrid();
	}

	void Update() {
		if(Input.GetMouseButtonDown(0))
			GenerateGrid();
	}

	void GenerateGrid() {
		for(int x = 0; x < grid.GetLength(0); x++) {
			for(int z = 0; z < grid.GetLength(1); z++) {
				grid[x, z] = (int)(Random.value * 255);
				renderGrid[x, z] = 0;
			}
		}
		//AddToGrid(renderGrid, grid, 0.05f);
		AddToGrid(renderGrid, ZoomGrid(grid, 2), 0.1f);
		AddToGrid(renderGrid, ZoomGrid(grid, 4), 0.2f);
		AddToGrid(renderGrid, ZoomGrid(grid, 8), 0.3f);
		AddToGrid(renderGrid, ZoomGrid(grid, 16), 0.4f);
	}

	int[,] ZoomGrid(int[,] grid, int scale) {
		int w = grid.GetLength(0), h = grid.GetLength(1);
		int[,] gridOut = new int[w, h];

		for(int x = 0; x < w; x++) {
			for(int z = 0; z < h; z++) {
				int zoomX1 = (int)(x / (float)scale);
				int zoomZ1 = (int)(z / (float)scale);
				int zoomX2 = (zoomX1 + 1) % 128;
				int zoomZ2 = (zoomZ1 + 1) % 128;

				float tx = (x % scale) / (float)scale;
				float tz = (z % scale) / (float)scale;

				float interpX1 = Interpolate(grid[zoomX1, zoomZ1], grid[zoomX2, zoomZ1], tx);
				float interpX2 = Interpolate(grid[zoomX1, zoomZ2], grid[zoomX2, zoomZ2], tx);
				float interpZ = Interpolate(interpX1, interpX2, tz);

				gridOut[x, z] = (int)interpZ;
			}
		}
		return gridOut;
	}

	float Interpolate(float val1, float val2, float t) {
		return val1 * (1-t) + val2 * t;
	}

	void AddToGrid(int[,] grid, int[,] adding, float scale) {
		for(int x = 0; x < grid.GetLength(0); x++) {
			for(int z = 0; z < grid.GetLength(1); z++) {
				grid[x, z] += (int)(adding[x, z] * scale);
			}
		}
	}

	void OnDrawGizmos() {
		// Draw the grid
		for(int x = 0; x < 128; x++) {
			for(int z = 0; z < 128; z++) {
				float value = renderGrid[x, z] / 255.0f;
				Gizmos.color = new Color(value, value, value);
				Gizmos.DrawCube(new Vector3(x / 128.0f, 0, z / 128.0f), Vector3.one / 128.0f);
			}
		}

		/*
		// Draws cubes to show the chunk locations
		Gizmos.color = Color.white;
		if(map != null) {
			foreach(Dictionary<int, GameObject> mapX in map.Values) {
				foreach(GameObject obj in mapX.Values) {
					if(obj == null)
						continue;
					Chunk chunk = obj.GetComponent<Chunk>();
					Gizmos.DrawCube(chunk.ToWorldCoordinates(0, 0, 0), Vector3.one * 0.8f);
				}
			}
		}
		*/
	}

	/**
	 * Instantiates the chunk prefab at a specific location.
	 */
	public static GameObject InitChunk(int x, int z) {
		Vector3 pos = new Vector3(x * Chunk.CHUNK_SIZE, 0f, z * Chunk.CHUNK_SIZE);
		GameObject obj = Instantiate(mapGenerator.chunkPrefab, pos, Quaternion.identity);
		obj.GetComponent<Chunk>().InitChunk(x, z);
		return obj;
	}

	/**
	 * Gets the chunk and chunk coordinates (x, z), and returns null if it doesn't exist.
	 */
	public static GameObject GetChunk(int x, int z) {
		if(!map.ContainsKey(x))
			return null;
		Dictionary<int, GameObject> mapX = map[x];
		if(!mapX.ContainsKey(z))
			return null;
		return mapX[z];
	}

	/**
     * Gets the chunk at chunk coordinates (x, z), and creates the chunk if it doesn't already exist.
     */
	public static GameObject GetCreateChunk(int x, int z) {
		if(!map.ContainsKey(x))
			map[x] = new Dictionary<int, GameObject>();
		Dictionary<int, GameObject> mapX = map[x];
		if(!mapX.ContainsKey(z)) {
			mapX[z] = InitChunk(x, z);
		}
		return mapX[z];
	}

	/**
	 * Generates chunks in a square on the x-z plane with 2*num+1 being the length of a side.
     */
	public static void GenerateSquareChunks(int num) {
		for(int x = -num; x < num; x++) {
			for(int z = -num; z < num; z++) {
				GetCreateChunk(x, z);
			}
		}
	}

	/**
	 * The method called during map generation to generate the chunks.
	 * TODO: Replace this method with auto-generating chunks
	 */
	public static void GenerateChunks() {
		GenerateSquareChunks(2);
	}

	/**
	 * Reconstructs the mesh for all of the chunks in the map.
	 */
	public static void ReconstructMesh() {
		if(map == null)
			return;
		foreach(Dictionary<int, GameObject> mapX in map.Values) {
			foreach(GameObject obj in mapX.Values) {
				obj.GetComponent<Chunk>().ReconstructMesh();
			}
		}
	}

	/**
	 * Returns the tile id at the given world coordinates. Will return 0 for air and -1 for undefined.
	 */
	public static int GetTileAt(int x, int y, int z) {
		int chunkX = Mathf.FloorToInt((float)x / Chunk.CHUNK_SIZE), modX = x % Chunk.CHUNK_SIZE;
		int chunkZ = Mathf.FloorToInt((float)z / Chunk.CHUNK_SIZE), modZ = z % Chunk.CHUNK_SIZE;

		// Fixes an issue with mod and negative numbers
		if(chunkX < 0) modX += -chunkX * Chunk.CHUNK_SIZE;
		if(chunkZ < 0) modZ += -chunkZ * Chunk.CHUNK_SIZE;

		GameObject chunkObj = GetChunk(chunkX, chunkZ);
		if(chunkObj == null)
			return -1;
		return chunkObj.GetComponent<Chunk>().GetLocalTileAt(modX, y, modZ);
	}

	/**
	 * Returns true if the tile is not air nor undefined.
	 */
	public static bool IsTileBlock(int x, int y, int z) {
		int id = GetTileAt(x, y, z);
		return id > 0;
	}
}
