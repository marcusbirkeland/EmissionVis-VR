using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(BuildingSpawnerScript))]
public class BuildingSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BuildingSpawnerScript bs = (BuildingSpawnerScript) target;

        EditorGUILayout.LabelField("Henlo");

        if(GUILayout.Button("Spawn Buildings"))
        {
            bs.SpawnBuildings();
        }
    }
}
