using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace TF.Utilities.RemoteConfig
{
    public class RemoteConfigCSVExporter : EditorWindow
    {
        #region Fields
        #region Configuration Fields
        private const int BROWSE_BUTTON_SIZE = 150;
        private const int OK_BUTTON_SIZE = 100;
        #endregion

        #region User read/write fields
        private string _databasesFolder;
        private string _outFolder;
        #endregion

        #region User readonly fields
        private string _logLabel = string.Empty;
        #endregion
        #endregion

        #region Fields
        [MenuItem("StickWars/Open CSV exporter")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            RemoteConfigCSVExporter window = (RemoteConfigCSVExporter)EditorWindow.GetWindow(typeof(RemoteConfigCSVExporter));
            window.Show();
        }

        void OnGUI()
        {
            GUILayout.Label("CSV Exporter", EditorStyles.boldLabel);
            DrawDatabasesBrowse();
            DrawOutputBrowse();
            DrawExportButton();
            DrawFeedbackText();
        }

        #region Draw methods
        void DrawDatabasesBrowse()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Databases Folder", GUILayout.ExpandWidth(false), GUILayout.Width(BROWSE_BUTTON_SIZE)))
            {
                _databasesFolder = EditorUtility.SaveFolderPanel("Path to databases folder", _databasesFolder, Application.dataPath);
            }

            EditorGUILayout.TextField(_databasesFolder);


            EditorGUILayout.EndHorizontal();
        }

        void DrawOutputBrowse()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Browser output", GUILayout.ExpandWidth(false), GUILayout.Width(BROWSE_BUTTON_SIZE)))
            {
                _outFolder = EditorUtility.SaveFolderPanel("Output path", _outFolder, Application.dataPath);
            }

            EditorGUILayout.TextField(_outFolder);

            EditorGUILayout.EndHorizontal();
        }

        void DrawExportButton()
        {
            // center button
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("OK", GUILayout.Width(OK_BUTTON_SIZE)))
            {
                Export();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        void DrawFeedbackText()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label(_logLabel);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        #endregion

        #region Export methods
        void Export()
        {
            List<Entry> entries = new List<Entry>();

            // Get all ScriptablesObject in databases path
            string[] scriptablesObjectsPath = Directory.GetFiles(_databasesFolder, "*.asset", SearchOption.AllDirectories);

            // Browse each scriptable objects founded
            foreach (var scriptableObjectPath in scriptablesObjectsPath)
                entries = BrowseAssetField(ref entries, scriptableObjectPath);

            // Create file
            CreateFileFromEntries(entries);

            // Feedbacks           
            System.Diagnostics.Process.Start(_outFolder); // open explorer on the output folder
            _logLabel = string.Format("Work done, {0}. Founded {1} entries.", GetFilename(), entries.Count);
        }

        /// <summary>
        /// Get asset from assetPath. Then, foreach private andn public fields add an entry.
        /// </summary>
        private List<Entry> BrowseAssetField(ref List<Entry> entries, string assetPath)
        {
            // Get relative path (eg. "Assets/Databases/myScriptableObject.asset")
            string fileRelativePath = assetPath;
            fileRelativePath = fileRelativePath.Replace(Application.dataPath + @"/", "");
            fileRelativePath = fileRelativePath.Insert(0, "Assets/");

            // Get GUID
            var guid = AssetDatabase.AssetPathToGUID(fileRelativePath);

            // Get instance from GUID
            UnityEngine.Object instance = AssetDatabase.LoadAssetAtPath<ScriptableObject>(fileRelativePath);
            Type scriptableObjectType = instance.GetType();

            FieldInfo[] fields = scriptableObjectType.GetFields(
                         BindingFlags.NonPublic | BindingFlags.Public |
                         BindingFlags.Instance);

            // Browse every private fields          
            string prefix = instance.name; // filename
            foreach (var field in fields)
            {
                AddEntryFromField(ref entries, instance, prefix, field);
            }

            // Debug
            Debug.LogFormat("Found file guid: {1} of path {0}", fileRelativePath, guid);

            return entries;
        }

        void AddEntryFromField(ref List<Entry> entries, UnityEngine.Object instance, string prefix, FieldInfo field)
        {
            // gather informations for entry
            string key = prefix + field.Name;
            string type = GetRemoteConfigType(field.GetValue(instance));
            string value = field.GetValue(instance).ToString().Replace(",", ".");
            string segment = "\"All Current Users\"";
            string priority = "";

            // create entry
            Entry entry = new Entry(key, type, value, segment, priority);
            entries.Add(entry);
        }

        void CreateFileFromEntries(List<Entry> entries)
        {
            string outputPath = _outFolder + "/" + GetFilename() + ".csv";

            using (StreamWriter file = new StreamWriter(outputPath))
            {
                file.WriteLine("key,type,value,segment,priority");

                foreach (var entry in entries)
                {
                    if (entry.type != "unsupported")
                    {
                        file.WriteLine(entry.ToString());
                    }
                }
            }
        }

        #region Getter
        public string GetRemoteConfigType(object toConvertObject)
        {
            var notConvertedType = toConvertObject.GetType();

            if (notConvertedType == typeof(float)) return "float"; // otherwise, return System.Single
            if (notConvertedType == typeof(int)) return "int"; // otherwise, return Single.Int32
            if (notConvertedType == typeof(long)) return "long"; // etc...
            if (notConvertedType == typeof(bool)) return "bool";
            if (notConvertedType == typeof(string)) return "string";

            Debug.LogErrorFormat("Remote config doesn't support type of {0}.", notConvertedType);

            return "unsupported";
        }

        public string GetFilename()
        {
            return DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        }
        #endregion
        #endregion
        #endregion
    }

    public struct Entry
    {
        public string key;
        public string type;
        public string value;
        public string segment;
        public string priority;

        public Entry(string key, string type, string value, string segment, string priority)
        {
            this.key = key;
            this.type = type;
            this.value = value;
            this.segment = segment;
            this.priority = priority;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},", key, type, value, segment, priority);
        }
    }
}
