using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {

	public static readonly int CHUNK_SIZE = 16;
	public static readonly int CHUNK_HEIGHT = 128;

	private int chunkX;
	private int chunkZ;
	private int[,,] tiles;

	private List<Vector3> vertices;
	private List<int> triangles;

	/**
	 * Initializes the chunk's data. Must be manually called.
	 */
	public void InitChunk(int x, int z) {
		tiles = new int[CHUNK_SIZE, CHUNK_HEIGHT, CHUNK_SIZE];
		//InitTiles();

		vertices = new List<Vector3>();
		triangles = new List<int>();

		SetCoords(x, z);
		GenerateDefaultTiles();
	}

	/*private void InitTiles() {
		for(int x = 0; x < CHUNK_SIZE; x++) {
			for(int y = 0; y < CHUNK_HEIGHT; y++) {
				for(int z = 0; z < CHUNK_SIZE; z++) {
					tiles[x, y, z] = 0;
				}
			}
		}
	}*/

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

	/**
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

	/**
	 * Gets the tile at the specified local chunk coordinates. Returns -1 if the coordinate
	 * is out of bounds.
	 */
	public int GetLocalTileAt(int x, int y, int z) {
		if(x < 0 || x >= CHUNK_SIZE) return -1;
		if(y < 0 || y >= CHUNK_HEIGHT) return -1;
		if(z < 0 || z >= CHUNK_SIZE) return -1;
		return tiles[x, y, z];
	}

	/**
	 * Initializes the tiles in this chunk to be blocks for the bottom half of the height
	 * and air for the top half of the height.
	 */
	public void GenerateDefaultTiles() {
		for(int x = 0; x < CHUNK_SIZE; x++) {
			for(int y = 0; y < CHUNK_HEIGHT / 2; y++) {
				for(int z = 0; z < CHUNK_SIZE; z++) {
					tiles[x, y, z] = 1;
				}
			}
		}
	}

	/**
	 * Clears this chunk's mesh and reconstructs it with the new tile positions. Usually
	 * only called initially or when a tile has been changed.
	 */
	public void ReconstructMesh() {
		vertices.Clear();
		triangles.Clear();

		int startIndex = 0;
		for(int x = 0; x < CHUNK_SIZE; x++) {
			for(int y = 0; y < CHUNK_HEIGHT; y++) {
				for(int z = 0; z < CHUNK_SIZE; z++) {
					if(tiles[x, y, z] == 0)
						continue;
					if(!MapGenerator.IsTileBlock(x+1, y  , z  ))
						AddRightFace(x, y, z, ref startIndex);
					if(!MapGenerator.IsTileBlock(x-1, y  , z  ))
						AddLeftFace(x, y, z, ref startIndex);
					if(!MapGenerator.IsTileBlock(x  , y+1, z  ))
						AddTopFace(x, y, z, ref startIndex);
					if(!MapGenerator.IsTileBlock(x  , y-1, z  ))
						AddBottomFace(x, y, z, ref startIndex);
					if(!MapGenerator.IsTileBlock(x  , y  , z+1))
						AddBackFace(x, y, z, ref startIndex);
					if(!MapGenerator.IsTileBlock(x  , y  , z-1))
						AddFrontFace(x, y, z, ref startIndex);
				}
			}
		}

		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();
	}


	// TODO: Remove duplicate vertices?
	private void AddRightFace(int x, int y, int z, ref int startIndex) {
		vertices.Add(new Vector3(x+1, y  , z  ));
		vertices.Add(new Vector3(x+1, y  , z+1));
		vertices.Add(new Vector3(x+1, y+1, z+1));
		vertices.Add(new Vector3(x+1, y+1, z  ));

		triangles.Add(startIndex + 0);
		triangles.Add(startIndex + 2);
		triangles.Add(startIndex + 1);

		triangles.Add(startIndex + 0);
		triangles.Add(startIndex + 3);
		triangles.Add(startIndex + 2);

		startIndex += 4;
	}

	private void AddLeftFace(int x, int y, int z, ref int startIndex) {
		vertices.Add(new Vector3(x, y  , z+1));
		vertices.Add(new Vector3(x, y  , z  ));
		vertices.Add(new Vector3(x, y+1, z  ));
		vertices.Add(new Vector3(x, y+1, z+1));

		triangles.Add(startIndex + 0);
		triangles.Add(startIndex + 2);
		triangles.Add(startIndex + 1);

		triangles.Add(startIndex + 0);
		triangles.Add(startIndex + 3);
		triangles.Add(startIndex + 2);

		startIndex += 4;
	}

	private void AddTopFace(int x, int y, int z, ref int startIndex) {
		vertices.Add(new Vector3(x  , y+1, z  ));
		vertices.Add(new Vector3(x+1, y+1, z  ));
		vertices.Add(new Vector3(x+1, y+1, z+1));
		vertices.Add(new Vector3(x  , y+1, z+1));

		triangles.Add(startIndex + 0);
		triangles.Add(startIndex + 2);
		triangles.Add(startIndex + 1);

		triangles.Add(startIndex + 0);
		triangles.Add(startIndex + 3);
		triangles.Add(startIndex + 2);

		startIndex += 4;
	}

	private void AddBottomFace(int x, int y, int z, ref int startIndex) {
		vertices.Add(new Vector3(x  , y, z+1));
		vertices.Add(new Vector3(x+1, y, z+1));
		vertices.Add(new Vector3(x+1, y, z  ));
		vertices.Add(new Vector3(x  , y, z  ));

		triangles.Add(startIndex + 0);
		triangles.Add(startIndex + 2);
		triangles.Add(startIndex + 1);

		triangles.Add(startIndex + 0);
		triangles.Add(startIndex + 3);
		triangles.Add(startIndex + 2);

		startIndex += 4;
	}

	private void AddFrontFace(int x, int y, int z, ref int startIndex) {
		vertices.Add(new Vector3(x  , y  , z));
		vertices.Add(new Vector3(x+1, y  , z));
		vertices.Add(new Vector3(x+1, y+1, z));
		vertices.Add(new Vector3(x  , y+1, z));

		triangles.Add(startIndex + 0);
		triangles.Add(startIndex + 2);
		triangles.Add(startIndex + 1);

		triangles.Add(startIndex + 0);
		triangles.Add(startIndex + 3);
		triangles.Add(startIndex + 2);

		startIndex += 4;
	}

	private void AddBackFace(int x, int y, int z, ref int startIndex) {
		vertices.Add(new Vector3(x+1, y  , z+1));
		vertices.Add(new Vector3(x  , y  , z+1));
		vertices.Add(new Vector3(x  , y+1, z+1));
		vertices.Add(new Vector3(x+1, y+1, z+1));

		triangles.Add(startIndex + 0);
		triangles.Add(startIndex + 2);
		triangles.Add(startIndex + 1);

		triangles.Add(startIndex + 0);
		triangles.Add(startIndex + 3);
		triangles.Add(startIndex + 2);

		startIndex += 4;
	}
}
