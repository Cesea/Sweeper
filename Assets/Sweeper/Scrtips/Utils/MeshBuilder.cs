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
            Vector2[] localUVs = new Vector2[4];

            int[] localTriangles = new int[6];

            if (list.Count == 2)
            {
                NodeSideInfo current = list[0];
                NodeSideInfo next = list[1];

                Side currentRelativeSide = Node.GetRelativeSide(current._node, next._node);
                {
                    int startIndex = 0;
                    switch (currentRelativeSide)
                    {
                        case Side.Left:
                        case Side.Right:
                        case Side.Top:
                        case Side.Bottom:
                            {
                                startIndex = 0;
                            }
                            break;
                        case Side.Front:
                        case Side.Back:
                            {
                                startIndex = 1;
                            }
                            break;
                    }
                    localUVs[0] = inputUV[0, (startIndex + 0) % 4];
                    localUVs[1] = inputUV[0, (startIndex + 1) % 4];
                    localUVs[2] = inputUV[0, (startIndex + 2) % 4];
                    localUVs[3] = inputUV[0, (startIndex + 3) % 4];
                }

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

                uvs.AddRange(localUVs);
                vertices.AddRange(localVertices);
                normals.AddRange(localNormals);
                triangles.AddRange(localTriangles);
            }
            else if(list.Count > 2)
            {
                Side prevRelativeSide = Node.GetRelativeSide(list[0]._node, list[1]._node);
                for (int i = 0; i < list.Count - 1; ++i)
                {
                    NodeSideInfo current = list[i];
                    NodeSideInfo next = list[i + 1];

                    Side currentRelativeSide = Node.GetRelativeSide(current._node, next._node);

                    #region In Case RelativeSide is different
                    if (prevRelativeSide != currentRelativeSide)
                    {
                        int startIndex = 0;

                        if (prevRelativeSide == Side.Left)
                        {
                            switch (currentRelativeSide)
                            {
                                case Side.Front: { startIndex = 3; } break;
                                case Side.Back: { startIndex = 2; } break;
                            }
                        }
                        else if (prevRelativeSide == Side.Right)
                        {
                            switch (currentRelativeSide)
                            {
                                case Side.Front: { startIndex = 0; } break;
                                case Side.Back: { startIndex = 1; } break;
                            }
                        }
                        else if (prevRelativeSide == Side.Front)
                        {
                            switch (currentRelativeSide)
                            {
                                case Side.Left: { startIndex = 1; } break;
                                case Side.Right: { startIndex = 2; } break;
                                case Side.Top: { startIndex = 1; }break;
                            }
                        }
                        else if (prevRelativeSide == Side.Back)
                        {
                            switch (currentRelativeSide)
                            {
                                case Side.Left: { startIndex = 0; } break;
                                case Side.Right: { startIndex = 3; } break;
                            }
                        }
                        localUVs[0] = inputUV[1, (startIndex + 0) % 4];
                        localUVs[1] = inputUV[1, (startIndex + 1) % 4];
                        localUVs[2] = inputUV[1, (startIndex + 2) % 4];
                        localUVs[3] = inputUV[1, (startIndex + 3) % 4];
                    }
                    #endregion
                    #region In case RelativeSide is same
                    else
                    {
                        int startIndex = 0;
                        switch (currentRelativeSide)
                        {
                            case Side.Left:
                            case Side.Right:
                                {
                                    startIndex = 0;
                                }break;
                            case Side.Top:
                            case Side.Bottom:
                                {
                                    startIndex = 1;
                                }
                                break;
                            case Side.Front:
                            case Side.Back:
                                {
                                    startIndex = 1;
                                }
                                break;
                        }
                        localUVs[0] = inputUV[0, (startIndex + 0) % 4];
                        localUVs[1] = inputUV[0, (startIndex + 1) % 4];
                        localUVs[2] = inputUV[0, (startIndex + 2) % 4];
                        localUVs[3] = inputUV[0, (startIndex + 3) % 4];
                    }
                    #endregion

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
                    uvs.AddRange(localUVs);

                    prevRelativeSide = currentRelativeSide;
                }
            }
            #region Last arrow
            {
                NodeSideInfo last = null;
                NodeSideInfo prev = null;
                Side relativeSide = Side.Right;
                if (list.Count == 1)
                {
                    last = list[0];
                }
                else
                {
                    last = list[list.Count - 1];
                    prev = list[list.Count - 2];
                    Vector3Int diff = last._node.BoardPosition - prev._node.BoardPosition;
                    relativeSide = BoardManager.NormalToSide(diff.ToVector3());
                }

                int startIndex = 0;
                switch (relativeSide)
                {
                    case Side.Right: { startIndex = 0; } break;
                    case Side.Left: { startIndex = 2; } break;
                    case Side.Front: { startIndex = 1; } break;
                    case Side.Back: { startIndex = 3; } break;
                    case Side.Top: { startIndex = 1; } break;
                    case Side.Bottom: { startIndex = 3; } break;
                }
                localUVs[0] = inputUV[2, (startIndex + 0) % 4];
                localUVs[1] = inputUV[2, (startIndex + 1) % 4];
                localUVs[2] = inputUV[2, (startIndex + 2) % 4];
                localUVs[3] = inputUV[2, (startIndex + 3) % 4];

                Vector3 lastOffset = BoardManager.SideToVector3Offset(last._side);


                BuildQuadData(last._side,
                                BoardManager.Instance.NodeRadius,
                                -lastOffset,
                                ref localVertices,
                                ref localNormals,
                                ref localTriangles);

                for (int j = 0; j < 4; ++j)
                {
                    localVertices[j] += last.GetWorldPosition() + lastOffset * 0.1f;
                }
                for (int j = 0; j < 6; ++j)
                {
                    localTriangles[j] += 4 * (list.Count - 1);
                }

                vertices.AddRange(localVertices);
                normals.AddRange(localNormals);
                triangles.AddRange(localTriangles);
                uvs.AddRange(localUVs);
            }
            #endregion


            mesh.vertices = vertices.ToArray();
            mesh.normals = normals.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}
