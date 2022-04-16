using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TrackGenerator))]
public class TrackGeneratorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TrackGenerator generator = (TrackGenerator)target;

        if (GUILayout.Button("Random"))
        {
            GenerateRandom(generator);
        }
        if (GUILayout.Button("Looping"))
        {
            generator.Expand(Directions.Looping);
        }

        if (GUILayout.Button("Forward"))
        {
            generator.Expand(Directions.Forward);
        }
        if (GUILayout.Button("Fast Forward"))
        {
            for (int i = 0; i < 20; i++)
            {
                generator.Expand(Directions.Forward);
            }
        }
        if (GUILayout.Button("Left"))
        {
            generator.Expand(Directions.Left);
        }
        if (GUILayout.Button("Right"))
        {
            generator.Expand(Directions.Right);
        }
        if (GUILayout.Button("Reset"))
        {
            generator.Reset();
        }
    }

    private static void GenerateRandom(TrackGenerator generator)
    {
        bool curve = true;
        bool curveLeft = false;
        bool curveRight = false;
        for (int i = 0; i < 100; i++)
        {
            float random = Random.Range(0, 2f);
            if (random <= 1f || curve)
            {
                for (int k = 0; k < 5; k++)
                {
                    curve = false;
                    generator.Expand(Directions.Forward);
                }
            }
            else if (random > 1f && random < 1.5f && !curveLeft)
            {
                curve = true;
                curveLeft = true;
                curveRight = false;
                generator.Expand(Directions.Left);
            }
            else if (!curveRight)
            {
                curve = true;
                curveLeft = false;
                curveRight = true;
                generator.Expand(Directions.Right);
            }
        }
    }
}