using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * TerrainGenerator
 * 
 * The generator has its own grid coordinate system that is independent of the one in the MapGenerator
 * class. Thus, it must be converted to the map coordinates after generating the terrain.
 * 
 * The generator also uses a pseudo-random number generator to generate the noise. The RNG values
 * rely on the specified seed (random by default) and the terrain coordinates, with the actual seed
 * having the value of "<seed>:<terrainx>.<terrainz>" .
 */
public class TerrainGenerator {

	private string terrainSeed;
	private Dictionary<string, int[,]> noiseGrid, terrainGrid;

	private static readonly int CHUNK_SIZE = 128; // Must be a power of 2

	public TerrainGenerator() : this("" + new System.Random().Next()) {}

	public TerrainGenerator(string seed) {
		terrainSeed = seed;
		noiseGrid = new Dictionary<string, int[,]>();
		terrainGrid = new Dictionary<string, int[,]>();
	}

	public int[,] GetTerrainGrid(int tx, int tz) {
		return terrainGrid[GetCoordKey(tx, tz)];
	}

	/*
	 * Generates the terrain for the specified terrain coordinates, and automatically returns if
	 * the terrain grids have already been created.
	 */
	public void GenerateTerrain(int tx, int tz) {
		string key = GetCoordKey(tx, tz);
		GenerateNoiseGrid(key);
		GenerateTerrainGrid(key, tx, tz);
	}

	private string GetCoordKey(int tx, int tz) {
		return tx + "." + tz;
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

		terrainGrid[key] = tGrid;
	}

	private int[,] ZoomGrid(int tx, int tz, int scale) {
		string zoomKey = GetCoordKey(tx / scale, tz / scale);

		// Generate zoom key's noise grid if it doesn't already exist
		GenerateNoiseGrid(zoomKey);

		// Find the right area to zoom onto
		int offsetX = MathHC.mod(tx, scale);
		int offsetZ = MathHC.mod(tz, scale);
		int offsetScale = CHUNK_SIZE / scale;
		return ZoomGrid_(noiseGrid[zoomKey], offsetX * offsetScale, offsetZ * offsetScale, scale);
	}

	/*
	 * A utility function that does the actual zooming computations for the ZoomGrid method.
	 * The scale parameter should be a power of 2.
	 */
	private int[,] ZoomGrid_(int[,] grid, int offsetX, int offsetZ, int scale) {
		int[,] gridOut = new int[CHUNK_SIZE, CHUNK_SIZE];

		for(int x = 0; x < CHUNK_SIZE; x++) {
			for(int z = 0; z < CHUNK_SIZE; z++) {
				int zoomX1 = (int)(x / (float)scale) + offsetX;
				int zoomZ1 = (int)(z / (float)scale) + offsetZ;
				int zoomX2 = (zoomX1 + 1) % CHUNK_SIZE;
				int zoomZ2 = (zoomZ1 + 1) % CHUNK_SIZE;

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

	private float Interpolate(float val1, float val2, float t) {
		return val1 * (1.0f-t) + val2 * t;
	}

	private void AddToGrid(int[,] grid, int[,] adding, float scale) {
		for(int x = 0; x < grid.GetLength(0); x++) {
			for(int z = 0; z < grid.GetLength(1); z++) {
				grid[x, z] += (int)(adding[x, z] * scale);
			}
		}
	}
}
