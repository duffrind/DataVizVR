using UnityEngine;
using System.Collections;

public class PlanesFactory {


	static public Mesh CreateHorizontalPlane( Vector2 meshSize, int meshVertexResolution )
	{
		int N_VERTICES = meshVertexResolution * meshVertexResolution;
		Vector2 DISTANCE_BETWEEN_VERTICES = new Vector2( meshSize.x / (float)(meshVertexResolution - 1.0f), meshSize.y / (float)(meshVertexResolution - 1.0f) );
		float DISTANCE_BETWEEN_UV = 1.0f / (float)(meshVertexResolution - 1.0f);

		Vector3[] vertices = new Vector3[N_VERTICES];
		Vector2[] uv = new Vector2[N_VERTICES];

		// Generate vertices and UV.
		for (int row=0; row<meshVertexResolution; row++) {
			for (int column=0; column<meshVertexResolution; column++) {
				int VERTEX_INDEX = row * meshVertexResolution + column;

				vertices[VERTEX_INDEX].x = -meshSize.x / 2.0f + column * DISTANCE_BETWEEN_VERTICES.x;
				vertices[VERTEX_INDEX].y = 0.0f;
				vertices[VERTEX_INDEX].z = meshSize.y / 2.0f - row * DISTANCE_BETWEEN_VERTICES.y;

				uv[VERTEX_INDEX].x = DISTANCE_BETWEEN_UV * column;
				uv[VERTEX_INDEX].y = 1.0f - DISTANCE_BETWEEN_UV * row;
			}
		}

		// Generate triangles
		int N_TRIANGLES = 2 * (meshVertexResolution - 1) * (meshVertexResolution - 1);
		int[] triangles = new int[N_TRIANGLES * 3];
		int triangleIndex = 0;
		for (int row=0; row<meshVertexResolution - 1; row++) {
			for (int column=0; column<meshVertexResolution - 1; column++) {
				triangles[triangleIndex] = GetVertexIndex( row, column, meshVertexResolution );
				triangles[triangleIndex + 1] = GetVertexIndex( row, column+1, meshVertexResolution ); 
				triangles[triangleIndex + 2] = GetVertexIndex( row+1, column, meshVertexResolution ); 

				triangles[triangleIndex + 3] = GetVertexIndex( row, column+1, meshVertexResolution );
				triangles[triangleIndex + 4] = GetVertexIndex( row+1, column+1, meshVertexResolution ); 
				triangles[triangleIndex + 5] = GetVertexIndex( row+1, column, meshVertexResolution ); 

				triangleIndex += 6;
			}
		}

		Mesh mesh = new Mesh ();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		return mesh;
	}



	static private int GetVertexIndex( int row, int column, int verticesPerRow )
	{
		return row * verticesPerRow + column;
	}
}
