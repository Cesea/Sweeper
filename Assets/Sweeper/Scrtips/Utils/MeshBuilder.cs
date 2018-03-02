using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class MeshBuilder 
    {
        public static void BuildQuadData(Side side, float radius, Vector3 offset,
            ref Vector3[] vertices, ref Vector3[] normals, ref int[] triangles)
        {
            Vector3 p1 = new Vector3( -radius, -radius, -radius) + offset;
            Vector3 p2 = new Vector3(-radius, radius, -radius) + offset;
            Vector3 p3 = new Vector3(radius, radius, -radius) + offset;
            Vector3 p4 = new Vector3(radius, -radius, -radius) + offset;
            Vector3 p5 = new Vector3(-radius, -radius, radius) + offset;
            Vector3 p6 = new Vector3(-radius, radius, radius) + offset;
            Vector3 p7 = new Vector3(radius, radius, radius) + offset;
            Vector3 p8 = new Vector3(radius, -radius, radius) + offset;

            switch (side)
            {
                case Side.Bottom:
                    {
                        vertices = new Vector3[] { p1, p4, p8, p5 };
                        normals = new Vector3[] { Vector3.down, Vector3.down, Vector3.down, Vector3.down };
                        triangles = new int[] { 0, 1, 2, 0, 2, 3 };
                    }
                    break;
                case Side.Top:
                    {
                        vertices = new Vector3[] { p2, p6, p7, p3 };
                        normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };
                        triangles = new int[] { 0, 1, 2, 0, 2, 3 };
                    }
                    break;
                case Side.Left:
                    {
                        vertices = new Vector3[] { p1, p2, p6, p5 };
                        normals = new Vector3[] { Vector3.left, Vector3.left, Vector3.left, Vector3.left };
                        triangles = new int[] { 0, 3, 2, 0, 2, 1 };
                    }
                    break;
                case Side.Right:
                    {
                        vertices = new Vector3[] { p4, p3, p7, p8 };
                        normals = new Vector3[] { Vector3.right, Vector3.right, Vector3.right, Vector3.right };
                        triangles = new int[] { 0, 1, 2, 0, 2, 3 };
                    }
                    break;
                case Side.Front:
                    {
                        vertices = new Vector3[] { p5, p6, p7, p8 };
                        normals = new Vector3[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
                        triangles = new int[] { 0, 3, 2, 0, 2, 1 };
                    }
                    break;
                case Side.Back:
                    {
                        vertices = new Vector3[] { p1, p2, p3, p4 };
                        normals = new Vector3[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back };
                        triangles = new int[] { 0, 1, 2, 0, 2, 3 };
                    }
                    break;
            }


        }

        public static Mesh BuildQuad(Side side, float radius, Vector3 offset)
        {
            Mesh mesh = new Mesh();
            mesh.name = "Scripted Mesh";

            Vector3[] vertices = new Vector3[4];
            Vector3[] normals = new Vector3[4];

            int[] triangles = new int[6];

            BuildQuadData(side, radius, offset, ref vertices, ref normals, ref triangles);

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.triangles = triangles;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        public static Mesh BuildQuadsFromNodeInfoList(List<NodeSideInfo> list,Vector2[,] inputUV)
        {
            Mesh mesh = new Mesh();
            mesh.name = "ListMesh";

            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Vector3> normals = new List<Vector3>();
            List<int> triangles = new List<int>();

            Vector3[] localVertices = new Vector3[4];
            Vector3[] localNormals = new Vector3[4];

            int[] localTriangles = new int[6];

            if (list.Count == 1)
            {

                NodeSideInfo current = list[0];

                Vector3 offset = BoardManager.SideToVector3Offset(current._side);

                BuildQuadData(current._side,
                                BoardManager.Instance.NodeRadius,
                                -offset,
                                ref localVertices,
                                ref localNormals,
                                ref localTriangles);

                for (int j = 0; j < 4; ++j)
                {
                    localVertices[j] += current.GetWorldPosition() + offset * 0.1f;
                }
                vertices.AddRange(localVertices);
                normals.AddRange(localNormals);
                triangles.AddRange(localTriangles);
            }
            else
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    NodeSideInfo current; 
                    NodeSideInfo next;
                    if (i == list.Count - 1)
                    {
                        current = list[i];
                        next = list[i - 1];
                    }
                    else
                    {
                        current = list[i];
                        next = list[i + 1];
                    }

                    Vector3Int boardDiff = next._node.BoardPosition - current._node.BoardPosition;
                    Side currentRelativeSide = BoardManager.NormalToSide(boardDiff.ToVector3());

                    Vector3 offset = BoardManager.SideToVector3Offset(current._side);

                    BuildQuadData(current._side,
                                    BoardManager.Instance.NodeRadius,
                                    -offset,
                                    ref localVertices,
                                    ref localNormals,
                                    ref localTriangles);

                    for (int j = 0; j < 4; ++j)
                    {
                        localVertices[j] += current.GetWorldPosition() + offset * 0.1f;
                    }
                    for (int j = 0; j < 6; ++j)
                    {
                        localTriangles[j] += 4 * i;
                    }

                    vertices.AddRange(localVertices);
                    normals.AddRange(localNormals);
                    triangles.AddRange(localTriangles);
                }

            }

            mesh.vertices = vertices.ToArray();
            mesh.normals = normals.ToArray();
            mesh.triangles = triangles.ToArray();

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

    }
}
