using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ChunkLoader.
 * 
 * Should be attached to the player. Determines which chunks should be loaded or removed based off
 * of their distance to the player.
 */
public class ChunkLoader : MonoBehaviour {

	public int loadRange = 6;

	private Chunk currentChunk;

	void Start() {
		currentChunk = null;
	}

	void Update() {
		if(!MapGenerator.GetMapInit())
			return;

		// Calculate current chunk
		int cx = (int)gameObject.transform.position.x / Chunk.CHUNK_SIZE;
		int cz = (int)gameObject.transform.position.z / Chunk.CHUNK_SIZE;
		GameObject chunkObj = MapGenerator.GetChunk(cx, cz);
		Chunk chunk = chunkObj != null ? chunkObj.GetComponent<Chunk>() : null;

		if(chunk != null && chunk != currentChunk) {
			// Add new chunks
			for(int x = cx - loadRange; x <= cx + loadRange; x++) {
				for(int z = cz - loadRange; z <= cx + loadRange; z++) {
					bool created;
					GameObject obj = MapGenerator.GetCreateChunk(x, z, out created);

					// If chunk was just created, must construct the mesh
					if(created) {
						obj.GetComponent<Chunk>().ReconstructMesh();
						Debug.Log(x + ", " + z);
					}
				}
			}

			// Remove chunks that are too far away
			foreach(Dictionary<int, GameObject> mapX in MapGenerator.GetMap().Values) {
				foreach(int key in new List<int>(mapX.Keys)) {
					GameObject obj = mapX[key];
					Chunk chunk_ = obj.GetComponent<Chunk>();
					if(Mathf.Abs(cx - chunk_.GetX()) >= loadRange) {
						Destroy(obj);
						mapX.Remove(key);
					} else if(Mathf.Abs(cz - chunk_.GetZ()) >= loadRange) {
						Destroy(obj);
						mapX.Remove(key);
					}
				}
			}


			currentChunk = chunk;
		}
	}
}
