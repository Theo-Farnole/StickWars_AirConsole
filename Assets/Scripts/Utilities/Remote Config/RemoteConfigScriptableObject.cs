using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace TF.Utilities.RemoteConfig
{
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

    public class RemoteConfigScriptableObject : ScriptableObject
    {
        public void UpdateRemoteSettings()
        {
            Type myType = this.GetType();

            FieldInfo[] fields = myType.GetFields(
                         BindingFlags.NonPublic | BindingFlags.Public |
                         BindingFlags.Instance);

            foreach (var field in fields)
            {
                Entry entry = GetEntryFromField(this, field);


                if (RemoteSettings.HasKey(entry.key))
                {
                    bool valueChanged = false;

                    //Type doesn't work with a switch, so let's do it this way
                    if (entry.type == "bool")
                    {
                        bool value = RemoteSettings.GetBool(entry.key);

                        if (value != (bool)field.GetValue(this)) valueChanged = true;

                        field.SetValue(this, value);
                    }
                    else if (entry.type == "float")
                    {
                        float value = RemoteSettings.GetFloat(entry.key);

                        if (value != (float)field.GetValue(this)) valueChanged = true;

                        field.SetValue(this, value);
                    }
                    else if (entry.type == "int")
                    {
                        int value = RemoteSettings.GetInt(entry.key);

                        if (value != (int)field.GetValue(this)) valueChanged = true;

                        field.SetValue(this, value);
                    }
                    else if (entry.type == "string")
                    {
                        string value = RemoteSettings.GetString(entry.key);

                        if (value != (string)field.GetValue(this)) valueChanged = true;

                        field.SetValue(this, value);
                    }

                    if (valueChanged)
                    {
                        Debug.LogFormat("<color=cyan>Remote Settings</color> # Update field {0} of {1}.", entry.key, this.name);
                    }
                }

                else
                {
                    Debug.LogFormat("<color=cyan>Remote Settings</color> # <color=red>No key</color> for field {0} of {1}.", entry.key, this.name);
                }
            }
        }

        public static Entry GetEntryFromField(UnityEngine.Object instance, FieldInfo field)
        {
            string prefix = instance.name;

            // gather informations for entry
            string key = prefix + field.Name;
            string type = GetRemoteConfigType(field.GetValue(instance));
            string value = field.GetValue(instance).ToString().Replace(",", ".");
            string segment = "\"All Current Users\"";
            string priority = "";

            // create entry
            Entry entry = new Entry(key, type, value, segment, priority);
            return entry;
        }

        public static string GetRemoteConfigType(object toConvertObject)
        {
            var notConvertedType = toConvertObject.GetType();

            if (notConvertedType == typeof(float)) return "float"; // otherwise, return System.Single
            if (notConvertedType == typeof(int)) return "int"; // otherwise, return Single.Int32
            if (notConvertedType == typeof(long)) return "long"; // etc...
            if (notConvertedType == typeof(bool)) return "bool";
            if (notConvertedType == typeof(string)) return "string";

            Debug.LogErrorFormat("<color=cyan>Remote Settings</color> # Remote config doesn't support type of {0}.", notConvertedType);

            return "unsupported";
        }
    }
}

