using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * A class containing all properties regarding a chunk's mesh for the sake of organization.
 */
public class ChunkMesh {

	private List<Vector3> vertices;
	private List<int> triangles;

	public ChunkMesh() {
		vertices = new List<Vector3>();
		triangles = new List<int>();
	}

	public void Clear() {
		vertices.Clear();
		triangles.Clear();
	}

	public Mesh CreateMesh() {
		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();
		return mesh;
	}

	// TODO: Remove duplicate vertices?
	public void AddRightFace(int x, int y, int z, ref int startIndex) {
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

	public void AddLeftFace(int x, int y, int z, ref int startIndex) {
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

	public void AddTopFace(int x, int y, int z, ref int startIndex) {
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

	public void AddBottomFace(int x, int y, int z, ref int startIndex) {
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

	public void AddFrontFace(int x, int y, int z, ref int startIndex) {
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

	public void AddBackFace(int x, int y, int z, ref int startIndex) {
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
