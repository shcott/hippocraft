using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Chunk.
 * 
 * A chunk of tiles. The MapGenerator loads these tiles in chunks, and the tiles are rendered as an
 * individual chunk as well.
 */
public class Chunk : MonoBehaviour {

	public static readonly int CHUNK_SIZE = 16;
	public static readonly int CHUNK_HEIGHT = 128;

	private int chunkX;
	private int chunkZ;
	private int[,,] tiles;
	private ChunkMesh chunkMesh;

	/*
	 * Initializes the chunk's data. Must be manually called.
	 */
	public void InitChunk(int x, int z) {
		tiles = new int[CHUNK_SIZE, CHUNK_HEIGHT, CHUNK_SIZE];
		chunkMesh = new ChunkMesh();

		SetCoords(x, z);
		//GenerateDefaultTiles();
		MapGenerator.GetTerrainGen().GenerateTerrainForChunk(x, z, tiles);
	}

	public void SetCoords(int x, int z) {
		chunkX = x;
		chunkZ = z;
	}

	public int GetX() {
		return chunkX;
	}

	public int GetZ() {
		return chunkZ;
	}

	/*
	 * A helper method to convert local tile coordinates to the actual world coordinates.
	 */
	public Vector3 ToWorldCoordinates(float x, float y, float z) {
		Vector3 vec = new Vector3();
		vec.x = chunkX * CHUNK_SIZE + x;
		vec.y = y;
		vec.z = chunkZ * CHUNK_SIZE + z;
		return vec;
	}

	public Vector3 ToWorldCoordinates(Vector3 coord) {
		return ToWorldCoordinates(coord.x, coord.y, coord.z);
	}

	/*
	 * Gets the tile at the specified local chunk coordinates. Returns -1 if the coordinate
	 * is out of bounds.
	 */
	public int GetLocalTileAt(int x, int y, int z) {
		if(x < 0 || x >= CHUNK_SIZE) return -1;
		if(y < 0 || y >= CHUNK_HEIGHT) return -1;
		if(z < 0 || z >= CHUNK_SIZE) return -1;
		return tiles[x, y, z];
	}

	/*
	 * Initializes the tiles in this chunk to be blocks for the bottom half of the height
	 * and air for the top half of the height.
	 */
	public void GenerateDefaultTiles() {
		for(int x = 0; x < CHUNK_SIZE; x++) {
			for(int y = 0; y < x +1; y++) {
				for(int z = 0; z < CHUNK_SIZE; z++) {
					tiles[x, y, z] = 1;
				}
			}
		}
	}

	/*
	 * Clears this chunk's mesh and reconstructs it with the new tile positions. Usually
	 * only called initially or when a tile has been changed.
	 */
	public void ReconstructMesh() {
		chunkMesh.Clear();

		int startIndex = 0;
		for(int x = 0; x < CHUNK_SIZE; x++) {
			for(int y = 0; y < CHUNK_HEIGHT; y++) {
				for(int z = 0; z < CHUNK_SIZE; z++) {
					if(tiles[x, y, z] == 0)
						continue;
					Vector3 w = ToWorldCoordinates(x, y, z);
					int wx = (int)w.x, wy = (int)w.y, wz = (int)w.z;
						
					if(!MapGenerator.IsTileBlock(wx+1, wy  , wz  ))
						chunkMesh.AddRightFace(x, y, z, ref startIndex);
					if(!MapGenerator.IsTileBlock(wx-1, wy  , wz  ))
						chunkMesh.AddLeftFace(x, y, z, ref startIndex);
					if(!MapGenerator.IsTileBlock(wx  , wy+1, wz  ))
						chunkMesh.AddTopFace(x, y, z, ref startIndex);
					if(!MapGenerator.IsTileBlock(wx  , wy-1, wz  ))
						chunkMesh.AddBottomFace(x, y, z, ref startIndex);
					if(!MapGenerator.IsTileBlock(wx  , wy  , wz+1))
						chunkMesh.AddBackFace(x, y, z, ref startIndex);
					if(!MapGenerator.IsTileBlock(wx  , wy  , wz-1))
						chunkMesh.AddFrontFace(x, y, z, ref startIndex);
				}
			}
		}

		Mesh mesh = chunkMesh.CreateMesh();
		GetComponent<MeshFilter>().mesh = mesh;
		GetComponent<MeshCollider>().sharedMesh = mesh;
	}
}
