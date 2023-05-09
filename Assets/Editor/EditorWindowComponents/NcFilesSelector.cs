using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor.EditorWindowComponents
{
    /// <summary>
    /// Class responsible for creating the GUI for selecting the netCDF files path.
    /// </summary>
    public class NcFilesSelector
    {
        private GUIStyle _folderButtonStyle;
        private GUIStyle _removeButtonStyle;
        private GUIStyle _folderIconStyle;
        
        /// <summary>
        /// A list of all paths that actually contain a netCDF file.
        /// </summary>
        public List<string> NcFiles => _ncFiles.Where(IsValidNetCdfFilePath).ToList();
        
        private readonly List<string> _ncFiles = new(){""};
        
        
        /// <summary>
        /// Draws the file selector GUI.
        /// </summary>
        /// <remarks>
        /// Must be used inside an EditorWindow.
        /// </remarks>
        public void Draw()
        {
            ApplyStyling();

            GUILayout.Label("Select NetCDF file", EditorStyles.boldLabel);

            for (int i = 0; i < _ncFiles.Count; i++)
            {
                GUILayout.BeginHorizontal();
                {
                    _ncFiles[i] = GUILayout.TextField(_ncFiles[i], GUILayout.Width(400));

                    GUILayout.BeginHorizontal(GUILayout.MaxWidth(45));
                    {
                        Rect folderButtonRect =
                            GUILayoutUtility.GetRect(_folderButtonStyle.fixedWidth, _folderButtonStyle.fixedHeight);
                        if (GUI.Button(folderButtonRect, "", _folderButtonStyle))
                        {
                            _ncFiles[i] = EditorUtility.OpenFilePanel("Select netCDF file", "", "nc");
                        }

                        // Render the folder icon inside the button
                        Rect folderIconRect = new(folderButtonRect.x, folderButtonRect.y, _folderButtonStyle.fixedWidth,
                            _folderButtonStyle.fixedHeight);
                        GUI.Label(folderIconRect, "", _folderIconStyle);

                        Rect removeButtonRect = GUILayoutUtility.GetRect(_removeButtonStyle.fixedWidth,
                            _removeButtonStyle.fixedHeight, GUILayout.ExpandWidth(false));
                        if (GUI.Button(removeButtonRect, "X", _removeButtonStyle))
                        {
                            _ncFiles.RemoveAt(i);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add file", GUILayout.Width(400)))
            {
                _ncFiles.Add("");
            }
        }
        
        
        private static bool IsValidNetCdfFilePath(string file)
        {
            return File.Exists(file) && Path.GetExtension(file).ToLower() == ".nc";
        }
        
        
        /// <summary>
        /// Must be declared at the start of each draw cycle.
        /// </summary>
        private void ApplyStyling()
        {
            _folderIconStyle ??= new GUIStyle
            {
                normal =
                {
                    background = (Texture2D) EditorGUIUtility.IconContent("Folder Icon").image
                }
            };

            _folderButtonStyle ??= new GUIStyle(GUI.skin.button)
            {
                fixedHeight = 20,
                fixedWidth = 20,
                alignment = TextAnchor.MiddleCenter
            };

            _removeButtonStyle ??= new GUIStyle(GUI.skin.button)
            {
                fixedHeight = 20,
                fixedWidth = 20,
                normal =
                {
                    textColor = Color.red
                },
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
        }
    }
}
