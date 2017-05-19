using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {

	public static int CHUNK_SIZE = 64;

	private int x;
	private int z;
	private int[,] tiles;

	void Start() {
		tiles = new int[CHUNK_SIZE, CHUNK_SIZE];
	}

	/**
	 * Sets the coordinates of the chunk.
	 */
	public void SetCoords(int x, int z) {
		this.x = x;
		this.z = z;
	}

	public int GetX () {
		return x;
	}

	public int GetZ() {
		return z;
	}

	void Update() {
		
	}
}
