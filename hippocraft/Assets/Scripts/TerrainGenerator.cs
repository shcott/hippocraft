using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator {

	private string terrainSeed;
	private Dictionary<string, int[,]> noiseGrid, terrainGrid;

	private static readonly int CHUNK_SIZE;

	public TerrainGenerator() {
		terrainSeed = "" + new System.Random().Next();
		noiseGrid = new Dictionary<string, int[,]>();
		terrainGrid = new Dictionary<string, int[,]>();
	}

	/**
	 * Generates the terrain for the specified terrain coordinates, and automatically returns if
	 * the terrain grids have already been created.
	 */
	public void GenerateTerrain(int tx, int tz) {
		string key = tx + "." + tz;
		GenerateNoiseGrid(key);
		GenerateTerrainGrid(key, tx, tz);
	}

	private void GenerateNoiseGrid(string key) {
		if(noiseGrid.ContainsKey(key))
			return;

		// Set up random instance and grid variable
		int seed = (terrainSeed + ":" + key).GetHashCode();
		System.Random rand = new System.Random(seed);
		int[,] nGrid = new int[CHUNK_SIZE, CHUNK_SIZE];

		// Fill grid with random noise
		for(int x = 0; x < CHUNK_SIZE; x++) {
			for(int z = 0; z < CHUNK_SIZE; z++) {
				nGrid[x, z] = rand.Next(0, 256);
			}
		}

		noiseGrid[key] = nGrid;
	}

	private void GenerateTerrainGrid(string key, int tx, int tz) {
		if(terrainGrid.ContainsKey(key))
			return;

		int[,] tGrid = new int[CHUNK_SIZE, CHUNK_SIZE];

		AddToGrid(tGrid, noiseGrid[key], 0.025f);
		AddToGrid(tGrid, ZoomGrid(tx, tz, 2), 0.075f);
		AddToGrid(tGrid, ZoomGrid(tx, tz, 4), 0.2f);
		AddToGrid(tGrid, ZoomGrid(tx, tz, 8), 0.3f);
		AddToGrid(tGrid, ZoomGrid(tx, tz, 16), 0.4f);
	}

	private int[,] ZoomGrid(int tx, int tz, int scale) {
		string zoomKey = GetZoomKey(tx, tz, scale);

		// Generate zoom key's noise grid if it doesn't already exist
		GenerateNoiseGrid(zoomKey);
		int[,] zoomGrid = noiseGrid[zoomKey];

		// TODO: Find the right area to zoom onto

	}

	private string GetZoomKey(int tx, int tz, int scale) {
		int worldX = tx * CHUNK_SIZE, worldZ = tz * CHUNK_SIZE;
		int zoomX = (worldX / scale) / CHUNK_SIZE;
		int zoomZ = (worldZ / scale) / CHUNK_SIZE;
		return zoomX + "." + zoomZ;
	}













	int[,] ZoomGridLOL(int[,] grid, int scale) {
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
}
