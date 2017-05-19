using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public GameObject chunkPrefab;
	private Dictionary<int, Dictionary<int, GameObject>> map;


	public GameObject InitChunk(int x, int z) {
		int hc = Chunk.CHUNK_SIZE / 2;
		Vector3 pos = new Vector3(x * Chunk.CHUNK_SIZE + hc, 0f, z * Chunk.CHUNK_SIZE + hc);
		GameObject obj = Instantiate(chunkPrefab, pos, Quaternion.identity);
		obj.GetComponent<Chunk>().SetCoords(x, z);
		return obj;
	}

	/**
     * Gets the chunk at chunk coordinates (x, z), and creates the chunk if it doesn't already exist.
     */
	public GameObject GetCreateChunk(int x, int z) {
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
	public void GenerateSquareChunks(int num) {
		for(int x = -num; x < num; x++) {
			for(int z = -num; z < num; z++) {
				GetCreateChunk(x, z);
			}
		}
	}

	public void GenerateChunks() {
		GenerateSquareChunks(2);
	}



	void Start () {
		map = new Dictionary<int, Dictionary<int, GameObject>>();
		GenerateChunks();
	}

	void Update () {
		
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.white;
		if(map != null) {
			foreach(Dictionary<int, GameObject> mapX in map.Values) {
				foreach(GameObject obj in mapX.Values) {
					if(obj == null)
						continue;
					Chunk chunk = obj.GetComponent<Chunk>();
					Vector3 pos = new Vector3(chunk.GetX() + 0.5f, 0, chunk.GetZ() + 0.5f);
					Gizmos.DrawCube(pos, Vector3.one * 0.8f);
				}
			}
		}
	}
}
