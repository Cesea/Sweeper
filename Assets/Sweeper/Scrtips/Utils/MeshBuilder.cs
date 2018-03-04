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

        public static void BuildLineQuadData(Vector3 v1, Vector3 v2, 
            Side quadSide,
            float lineWidth, int quadIndex, 
            ref Vector3[] verties, ref Vector3[] normals, ref int[] triangles)
        {
            Vector3 diff = v2 - v1;
            Side relativeSide = BoardManager.NormalToSide(diff);

            bool counterClockWise = false;

            Vector3 offsetDelta = Vector3.zero;
            switch (relativeSide)
            {
                case Side.Left:
                    {
                        offsetDelta = new Vector3(0.0f, 0.0f, 1.0f); 
                    } break;
                case Side.Right:
                    {
                        offsetDelta = new Vector3(0.0f, 0.0f, 1.0f);
                        counterClockWise = true;
                    } break;
                case Side.Front:
                    {
                        offsetDelta = new Vector3(1.0f, 0.0f, 0.0f);
                    } break;
                case Side.Back:
                    {
                        offsetDelta = new Vector3(1.0f, 0.0f, 0.0f);
                        counterClockWise = true;
                    } break;

                case Side.Top:
                    {
                        if (quadSide == Side.Left || quadSide == Side.Right)
                        {
                            offsetDelta.z = 1.0f;
                            if (quadSide == Side.Left)
                            {
                                counterClockWise = true;
                            }
                        }
                        else if (quadSide == Side.Front || quadSide == Side.Back)
                        {
                            offsetDelta.x = 1.0f;
                            if (quadSide == Side.Front)
                            {
                                counterClockWise = true;
                            }
                        }
                    } break;
                case Side.Bottom:
                    {
                        if (quadSide == Side.Left || quadSide == Side.Right)
                        {
                            offsetDelta.z = 1.0f;
                            if (quadSide == Side.Right)
                            {
                                counterClockWise = true;
                            }
                        }
                        else if (quadSide == Side.Front || quadSide == Side.Back)
                        {
                            offsetDelta.x = 1.0f;
                            if (quadSide == Side.Back)
                            {
                                counterClockWise = true;
                            }
                        }
                    } break;
            }
            offsetDelta *= lineWidth;
            Vector3 quadNormal = BoardManager.SideToOffset(quadSide);
            v1 += quadNormal * 0.1f;
            v2 += quadNormal * 0.1f;

            verties[0] = v1 - offsetDelta;
            verties[1] = v2 - offsetDelta;
            verties[2] = v2 + offsetDelta;
            verties[3] = v1 + offsetDelta;

            normals[0] = quadNormal; normals[1] = quadNormal; normals[2] = quadNormal; normals[3] = quadNormal;

            if (counterClockWise)
            {
                triangles[0] = 3; triangles[1] = 2; triangles[2] = 1;
                triangles[3] = 3; triangles[4] = 1; triangles[5] = 0;
            }
            else
            {
                triangles[0] = 0; triangles[1] = 1; triangles[2] = 2;
                triangles[3] = 0; triangles[4] = 2; triangles[5] = 3;
            }

            for (int j = 0; j < 6; ++j)
            {
                triangles[j] += quadIndex * 4;
            }
        }

        public static Mesh BuildQuadsFromNodeInfoList(List<NodeSideInfo> list,Vector2[,] inputUV, float lineWidth)
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

            List<Vector3> vertexList = new List<Vector3>();
            List<Side> sideList = new List<Side>();

            for (int i = 0; i < list.Count - 1; ++i)
            {
                NodeSideInfo current = list[i];
                NodeSideInfo next = list[i + 1];

                Vector3 closestEdge =  NodeSideInfo.GetClosestEdge(current, next);

                vertexList.Add(current.GetWorldPosition());
                vertexList.Add(closestEdge);
                vertexList.Add(next.GetWorldPosition());

                sideList.Add(current._side);
                sideList.Add(next._side);
            }


            int quadIndexCount = 0;

            int sideIndex = 0;

            for (int i = 0; i < vertexList.Count; i += 3)
            {
                Vector3 min = vertexList[i + 0];
                Vector3 middle = vertexList[i + 1];
                Vector3 max = vertexList[i + 2];

                Side minSide = sideList[sideIndex];
                Side maxSide = sideList[sideIndex + 1];

                //if (minSide == maxSide)
                {
                    BuildLineQuadData(min, middle,
                        minSide,
                        lineWidth, quadIndexCount++,
                        ref localVertices, ref localNormals, ref localTriangles);
                    vertices.AddRange(localVertices);
                    normals.AddRange(localNormals);
                    triangles.AddRange(localTriangles);

                    BuildLineQuadData(middle, max,
                        maxSide,
                        lineWidth, quadIndexCount++,
                        ref localVertices, ref localNormals, ref localTriangles);
                    vertices.AddRange(localVertices);
                    normals.AddRange(localNormals);
                    triangles.AddRange(localTriangles);
                }
                //else
                //{
                //    BuildCornerLineQuadData(
                //        min, middle,
                //        minSide, maxSide,
                //        lineWidth, quadIndexCount++,
                //        ref localVertices, ref localNormals, ref localTriangles);

                //    vertices.AddRange(localVertices);
                //    normals.AddRange(localNormals);
                //    triangles.AddRange(localTriangles);

                //    BuildCornerLineQuadData(
                //        middle, max,
                //        minSide, maxSide,
                //        lineWidth, quadIndexCount++,
                //        ref localVertices, ref localNormals, ref localTriangles);

                //    vertices.AddRange(localVertices);
                //    normals.AddRange(localNormals);
                //    triangles.AddRange(localTriangles);
                //}

                sideIndex += 2;
            }

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
