using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TF.Utilities.RemoteConfig
{
    /// <summary>
    /// We use it only to link scriptable object, trigger OnEnable callback.
    /// </summary>
    public class RemoteSettingsUpdater : MonoBehaviour
    {
        [SerializeField] private RemoteConfigScriptableObject[] scriptableObjects;

        void OnEnable()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                return;
#endif

            RemoteSettingsUpdated();
            RemoteSettings.Updated += RemoteSettingsUpdated;
        }

        void OnDisable()
        {
            RemoteSettings.Updated -= RemoteSettingsUpdated;
        }

        void RemoteSettingsUpdated()
        {
            foreach (var scriptableObject in scriptableObjects)
            {
                scriptableObject.UpdateRemoteSettings();
            }
        }

    }
}
