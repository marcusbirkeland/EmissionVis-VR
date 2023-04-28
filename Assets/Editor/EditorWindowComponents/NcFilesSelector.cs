using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor.EditorWindowComponents
{
    /// <summary>
    /// Class responsible for creating the GUI for selecting file locations
    /// of NetCDF files within a custom Unity Editor window.
    /// </summary>
    public class NcFilesSelector
    {
        public List<string> NcFiles { get;}
        
        private GUIStyle _folderButtonStyle;
        private GUIStyle _removeButtonStyle;
        private GUIStyle _folderIconStyle;

        
        /// <summary>
        /// Initializes a new instance of the NcFilesSelector class with an empty list of NetCDF files.
        /// </summary>
        public NcFilesSelector()
        {
            NcFiles = new List<string> {""};
        }
        
        
        /// <summary>
        /// Draws the file selector GUI. Must be called from inside an OnGUI event.
        /// </summary>
        public void Draw()
        {
            ApplyStyling();

            GUILayout.Label("Select NetCDF file", EditorStyles.boldLabel);

            for (int i = 0; i < NcFiles.Count; i++)
            {
                GUILayout.BeginHorizontal();
                
                    NcFiles[i] = GUILayout.TextField(NcFiles[i], GUILayout.Width(400));

                    GUILayout.BeginHorizontal(GUILayout.MaxWidth(45));

                        Rect folderButtonRect =
                            GUILayoutUtility.GetRect(_folderButtonStyle.fixedWidth, _folderButtonStyle.fixedHeight);
                        if (GUI.Button(folderButtonRect, "", _folderButtonStyle))
                        {
                            NcFiles[i] = EditorUtility.OpenFilePanel("Select netCDF file", "", "nc");
                        }

                        // Render the folder icon inside the button
                        Rect folderIconRect = new(folderButtonRect.x, folderButtonRect.y, _folderButtonStyle.fixedWidth,
                            _folderButtonStyle.fixedHeight);
                        GUI.Label(folderIconRect, "", _folderIconStyle);

                        Rect removeButtonRect = GUILayoutUtility.GetRect(_removeButtonStyle.fixedWidth,
                            _removeButtonStyle.fixedHeight, GUILayout.ExpandWidth(false));
                        if (GUI.Button(removeButtonRect, "X", _removeButtonStyle))
                        {
                            NcFiles.RemoveAt(i);
                        }

                    GUILayout.EndHorizontal();

                GUILayout.EndHorizontal();
            }

            // Add a new text field for additional files
            if (GUILayout.Button("Add file", GUILayout.Width(400)))
            {
                NcFiles.Add("");
            }
        }
        
        
        /**
         * <summary>
         * Applies styling to the file selector GUI elements.
         * </summary>
         */
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
