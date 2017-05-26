using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public static MapGenerator mapGenerator;

	public GameObject chunkPrefab;

	private static Dictionary<int, Dictionary<int, GameObject>> map;
	private static TerrainGenerator terrainGen;

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
		terrainGen = new TerrainGenerator();
		terrainGen.GenerateTerrain(0, 0);
	}

	void OnDrawGizmos() {
		// Draw the grid
		for(int x = 0; x < 128; x++) {
			for(int z = 0; z < 128; z++) {
				float value = terrainGen.GetTerrainGrid(0, 0)[x, z] / 255.0f;
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
