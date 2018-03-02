using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class MeshBuilder 
    {
        public static Mesh BuildQuad(Side side, float radius, Vector3 offset)
        {
            Mesh mesh = new Mesh();
            mesh.name = "Scripted Mesh";

            Vector3[] vertices = new Vector3[4];
            Vector3[] normals = new Vector3[4];

            int[] triangles = new int[6];

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
                        vertices = new Vector3[] { p1, p5, p6, p2 };
                        normals = new Vector3[] { Vector3.left, Vector3.left, Vector3.left, Vector3.left };
                        triangles = new int[] { 0, 1, 2, 0, 2, 3 };
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
                        vertices = new Vector3[] { p5, p8, p7, p6 };
                        normals = new Vector3[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
                        triangles = new int[] { 0, 1, 2, 0, 2, 3 };
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

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.triangles = triangles;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        public static Mesh BuildQuadsFromNodeInfoList(List<NodeSideInfo> list)
        {
            Mesh mesh = new Mesh();

            return mesh;
        }

    }
}
