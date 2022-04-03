using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Directions
{
    Up,
    Down,
    Left,
    Right,
    Forward
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TrackGenerator : MonoBehaviour
{
    List<Vector3> verticeList;
    List<Vector3> lastVerticies;
    List<int> triangles;
    MeshFilter meshFilter;
    Mesh mesh;

    int nextTrianglePosition;

    public bool showGizmos;

    public void Expand(Directions direction)
    {
        switch (direction)
        {
            case Directions.Forward:
                ExpandForward();
                break;
            case Directions.Left:
                TurnLeft();
                break;
            case Directions.Right:
                TurnRight();
                break;
            default:
                break;
        }

        mesh.vertices = verticeList.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
        RecalculateUvs();

        //GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    private void ExpandForward()
    {
        Vector3 direction;
        var bottomRight = lastVerticies[0];
        var prebottomRight = verticeList[verticeList.Count - 8];

        direction = bottomRight - prebottomRight;

        AddVerticies(direction);

        AddTriangles();

    }

    private void TurnLeft()
    {
        int TurnVerticiPoints = 10;

        Vector3 bottomRight = lastVerticies[0];
        Vector3 bottomLeft = lastVerticies[1];

        var radius = Vector3.Distance(bottomRight, bottomLeft) / 2f;
        var centerPos = bottomLeft - Vector3.Normalize(bottomRight - bottomLeft) / 2;
        
        var centerDirection = Quaternion.LookRotation((bottomRight - bottomLeft).normalized);

        for (var i = 0; i < TurnVerticiPoints; i++)
        {
            List<Vector3> newVerticies = new List<Vector3>();
            bool SwitchRadius = true;

            foreach (var item in lastVerticies)
            {

                radius = SwitchRadius ? radius + 1 : radius - 1;
                SwitchRadius = !SwitchRadius;

                var angle = Mathf.PI * (-i - 1) / (TurnVerticiPoints + TurnVerticiPoints / 2);
                var x = Mathf.Sin(angle) * radius;
                var z = Mathf.Cos(angle) * radius;
                var pos = new Vector3(x, 0, z);

                pos = centerDirection * pos;
                pos.y = item.y;
                newVerticies.Add(centerPos + pos);
            }


            lastVerticies = new List<Vector3>();
            lastVerticies.AddRange(newVerticies);
            verticeList.AddRange(newVerticies);
            AddTriangles();
        }
    }

    private void TurnRight()
    {
        int TurnVerticiPoints = 10;

        Vector3 bottomRight = lastVerticies[0];
        Vector3 bottomLeft = lastVerticies[1];

        var radius = Vector3.Distance(bottomRight, bottomLeft) / 2f;
        var centerPos = bottomRight + (bottomRight - bottomLeft) / 2;

        var centerDirection = Quaternion.LookRotation((bottomLeft - bottomRight).normalized);

        radius += 1;
        for (var i = 0; i < TurnVerticiPoints; i++)
        {
            List<Vector3> newVerticies = new List<Vector3>();
            bool SwitchRadius = false;

            foreach (var item in lastVerticies)
            {             
                radius = SwitchRadius ? radius + 1 : radius - 1;
                SwitchRadius = !SwitchRadius;

                var angle = Mathf.PI * (i + 1) / (TurnVerticiPoints + TurnVerticiPoints / 2);
                var x = Mathf.Sin(angle) * radius;
                var z = Mathf.Cos(angle) * radius;
                var pos = new Vector3(x, 0, z);

                pos = centerDirection * pos;
                pos.y = item.y;
                newVerticies.Add(centerPos + pos);
            }

            lastVerticies = new List<Vector3>();
            lastVerticies.AddRange(newVerticies);
            verticeList.AddRange(newVerticies);
            AddTriangles();
        }
    }

    private void AddVerticies(Vector3 dircetion)
    {
        List<Vector3> newVerticies = new List<Vector3>();
        foreach (var item in lastVerticies)
        {
            newVerticies.Add(item + dircetion);
        }
        lastVerticies = newVerticies;
        verticeList.AddRange(newVerticies);
    }

    private void AddTriangles()
    {

        List<int> trianglesBackFace = new List<int>();
        for (int i = 0; i < 6; i++)
        {
            trianglesBackFace.Add(triangles[triangles.Count - 1]);
            triangles.RemoveAt(triangles.Count - 1);
        }

        for (int i = 0; i < 24; i++)
        {
            triangles.Add(triangles[nextTrianglePosition] + 4);
            nextTrianglePosition++;
        }

        foreach (var itemBackface in trianglesBackFace)
        {
            triangles.Add(itemBackface + 4);
        }
    }

    internal void Reset()
    {
        nextTrianglePosition = 6;
        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
        }
        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = "Track";
            meshFilter.mesh = mesh;
        }

        ResetVerticies();
        ResetTriangles();

        mesh.vertices = verticeList.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        RecalculateUvs();
    }



    private void RecalculateUvs()
    {
        Vector2[] uvs = new Vector2[verticeList.Count];
        Vector2[] uvs2 = new Vector2[verticeList.Count];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(verticeList[i].x, verticeList[i].z);
        }
        mesh.uv = uvs;
    }

    private void ResetVerticies()
    {
        verticeList = new List<Vector3>
        {
            new Vector3(0, 0, 0), //Bottom 0 
            new Vector3(1, 0, 0), //Bottom Left 1
            new Vector3(0, .5f, 0), //Top  2
            new Vector3(1, .5f, 0) //Top Left 3
         };
        lastVerticies = new List<Vector3>
        {
            new Vector3(0, 0, -.5f), //4
            new Vector3(1, 0, -.5f), //5
            new Vector3(0, .5f, -.5f), //6
            new Vector3(1, .5f, -.5f) // 7
        };
        verticeList.AddRange(lastVerticies);
    }

    private void ResetTriangles()
    {
        triangles = new List<int>
        {
        //Front Face
        0, 1, 2,
        2, 1, 3,

        //Right Side
        0, 2, 6,
        6, 4, 0,

        //Top
        2, 3, 6,
        6, 3, 7,

        //Left Side
        3, 1, 7,
        1, 5, 7,

        //Bottom
        4, 1, 0,
        4, 5, 1,

        //Back Face
        4, 6, 7,
        7, 5, 4 };

    }

    void OnDrawGizmosSelected()
    {
        if (verticeList == null || !showGizmos) return;
        foreach (var item in verticeList)
        {
            Gizmos.color = item == Vector3.zero || verticeList.IndexOf(item) % 4 == 0 ? Color.blue : Color.red;
            Gizmos.DrawSphere(item, .02f);
        }
    }
}