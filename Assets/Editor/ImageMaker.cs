using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using UnityEditor.Scripting.Python;
using Debug = UnityEngine.Debug;


public class ImageMaker : EditorWindow
{
    private static String _speedPath;
    private static String _topographyPath;

    [MenuItem("NetCDF/Open Window")]
    private static void Init()
    {
        EditorWindow wnd = GetWindow<ImageMaker>();
        wnd.titleContent = new GUIContent("Load netCDF data into unity");
    }

    void OnGUI()
    {
        GUILayout.Label("Select a netCDF file");
        _speedPath = EditorGUILayout.TextField("Select windspeed datafile: ", _speedPath);
        Vector2 two = new Vector2(10, 10);
        GUILayout.Button(Texture2D.blackTexture);
    }

    [MenuItem("NetCDF/Get SpeedPath")]
    static void GetSpeedPath()
    {
        _speedPath = EditorUtility.OpenFilePanel("Select netCDF file", "", "nc");
        Debug.Log(_speedPath);
    }

    [MenuItem("NetCDF/Get TopographyPath")]
    static void GetTopographyPath()
    {
        _topographyPath = EditorUtility.OpenFilePanel("Select netCDF file", "", "nc");
    }
    
    [MenuItem("NetCDF/Get Data")]
    static void GetData()
    {
        Debug.Log("button clicked");
        PythonRunner.RunFile($"{Application.dataPath}/Scripts/NetCdfReader/create_csv.py", "C:/Users/Lars/Downloads/bergen_24_summer_L_av_xy.003.nc$wspeed_10m*_xy$C:/Users/Lars/Downloads/windspeed");
        Debug.Log("finished");
    }
}