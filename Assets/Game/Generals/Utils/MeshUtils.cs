using UnityEngine;

namespace Asce.Game.Utils
{
    /// <summary>
    ///     Provides utility methods for working with procedural meshes.
    /// </summary>
    public static class MeshUtils
    {
        /// <summary>
        ///     Sets mesh data including vertices, triangles, and UVs. Automatically recalculates normals and bounds.
        /// </summary>
        /// <param name="mesh"> The mesh to modify. </param>
        /// <param name="vertices"> Array of vertex positions. </param>
        /// <param name="triangles"> Array of triangle indices. </param>
        /// <param name="uv"> Optional array of UVs. If null or length mismatch, default UVs will be assigned. </param>
        public static void SetMeshData(Mesh mesh, Vector3[] vertices, int[] triangles, Vector2[] uv = null)
        {
            if (CheckIsNull(vertices, triangles)) return;

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = (uv == null || uv.Length != vertices.Length) ? new Vector2[vertices.Length] : uv;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }

        /// <summary>
        ///     Creates a new mesh with the given name, vertices, triangles, and UVs.
        /// </summary>
        /// <param name="name"> The name of the mesh (optional). </param>
        /// <param name="vertices"> Array of vertex positions. </param>
        /// <param name="triangles"> Array of triangle indices. </param>
        /// <param name="uv"> Optional array of UVs. If null or length mismatch, default UVs will be assigned. </param>
        /// <returns>
        ///     A new Mesh object, or null if input data is invalid.
        /// </returns>
        public static Mesh CreateMesh(string name, Vector3[] vertices, int[] triangles, Vector2[] uv = null)
        {
            if (CheckIsNull(vertices, triangles)) return null;
            
            Mesh mesh = new Mesh
            {
                name = name ?? "New Mesh",
                vertices = vertices,
                triangles = triangles,
                uv = (uv == null || uv.Length != vertices.Length) ? new Vector2[vertices.Length] : uv
            };

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        /// <summary>
        ///     Creates a new mesh with the vertices, triangles, and UVs.
        ///     <br/>
        ///     See <see cref="CreateMesh(string, Vector3[], int[], Vector2[])"/> for details."/>
        /// </summary>
        /// <param name="vertices"> Array of vertex positions. </param>
        /// <param name="triangles"> Array of triangle indices. </param>
        /// <param name="uv"> Optional array of UVs. If null or length mismatch, default UVs will be assigned. </param>
        /// <returns>
        ///     A new Mesh object, or null if input data is invalid.
        /// </returns>
        public static Mesh CreateMesh(Vector3[] vertices, int[] triangles, Vector2[] uv = null) => CreateMesh(null, vertices, triangles, uv);

        /// <summary>
        ///     Constructs a triangle index array for a grid mesh based on horizontal and vertical vertex counts.
        /// </summary>
        /// <param name="horizontalVertices"> The number of horizontal vertices in the grid. </param>
        /// <param name="verticalVertices"> The number of vertical vertices in the grid. </param>
        /// <returns>
        ///     An array of triangle indices suitable for a grid mesh.
        /// </returns>
        public static int[] ConstructTriangles(int horizontalVertices, int verticalVertices)
        {
            int[] triangles = new int[(horizontalVertices - 1) * (verticalVertices - 1) * 6];
            int index = 0;

            for (int y = 0; y < verticalVertices - 1; y++)
            {
                for (int x = 0; x < horizontalVertices - 1; x++)
                {
                    int topLeft = y * horizontalVertices + x;
                    int topRight = topLeft + 1;
                    int bottomLeft = topLeft + horizontalVertices;
                    int bottomRight = bottomLeft + 1;

                    // First triangele
                    triangles[index++] = topLeft;
                    triangles[index++] = bottomLeft;
                    triangles[index++] = bottomRight;

                    // Second triangle
                    triangles[index++] = topLeft;
                    triangles[index++] = bottomRight;
                    triangles[index++] = topRight;
                }
            }

            return triangles;
        }

        /// <summary>
        ///     Constructs a UV map for the provided vertex array, based on given width and height.
        /// </summary>
        /// <param name="vertices"> Array of mesh vertices. </param>
        /// <param name="width"> The total width of the mesh area.</param>
        /// <param name="height"> The total height of the mesh area. </param>
        /// <returns>
        ///     Array of UV coordinates corresponding to the vertices.
        /// </returns>
        public static Vector2[] ConstructUVs(Vector3[] vertices, float width, float height)
        {
            Vector2[] uvs = new Vector2[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vertex = vertices[i];
                uvs[i] = new Vector2((vertex.x + width * 0.5f) / width, (vertex.y + height * 0.5f) / height);
            }
            return uvs;
        }

        /// <summary>
        ///     Validates that the vertex and triangle arrays are not null.
        ///     <br/>
        ///     Logs errors if any data is invalid.
        /// </summary>
        /// <param name="vertices"> Array of vertices to check. </param>
        /// <param name="triangles"> Array of triangles to check. </param>
        /// <returns> True if either array is null, false otherwise. </returns>
        private static bool CheckIsNull(Vector3[] vertices, int[] triangles)
        {
            if (vertices == null)
            {
                Debug.LogError("Vertices cannot be null");
                return true;
            }

            if (triangles == null)
            {
                Debug.LogError("Triangles cannot be null");
                return true;
            }

            return false;
        }
    }
}