﻿using NDream.AirConsole;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CharIdAllocator
{
    #region Fields
    private static Dictionary<CharId, int> _dictionary = new Dictionary<CharId, int>();
    #endregion

    #region Properties
    public static int AllocatedCharIdCount
    {
        get
        {
            return _dictionary.Values.Count;
        }
    }

    public static Dictionary<CharId, int> DeviceIdToCharId { get => _dictionary; }
    #endregion

    #region Methods
    public static int GetDeviceId(CharId charId)
    {
        if (_dictionary.ContainsKey(charId))
        {
            return _dictionary[charId];
        }
        else
        {
            return -1;
        }
    }

    public static string GetNickname(CharId charId)
    {
        int device_id = GetDeviceId(charId);
        return AirConsole.instance.GetNickname(device_id);
    }
    
    public static bool DoDeviceIdExist(int deviceId)
    {
        return (GetCharId(deviceId) != null);
    }

    public static bool IsCharIdConnected(CharId charId)
    {
        return _dictionary.ContainsKey(charId);
    }

    public static CharId? GetCharId(int deviceId)
    {
        if (_dictionary.ContainsValue(deviceId) == false)
            return null;

        var item = _dictionary.First(x => x.Value == deviceId);
        return item.Key;
    }

    #region Allocation Methods
    public static CharId? AllocateDeviceId(int allocatorDeviceId)
    {
        if (_dictionary.ContainsValue(allocatorDeviceId))
        {
            Debug.LogWarning(allocatorDeviceId + " is trying to allocate another charId! Aborting");
            return null;
        }

        foreach (CharId item in Enum.GetValues(typeof(CharId)))
        {
            // is charId isn't allocate ?
            if (_dictionary.ContainsKey(item) == false)
            {
                return AllocateDeviceId(allocatorDeviceId, item); ;
            }
        }

        Debug.LogWarning("No charId available!");
        return null;
    }

    public static CharId? AllocateDeviceId(int allocatorDeviceId, CharId charId)
    {
        if (_dictionary.ContainsKey(charId) == true)
        {
            Debug.LogWarning(charId + " is already used.");
            return null;
        }

        Debug.Log(charId + " has been allocated by device " + allocatorDeviceId);
        _dictionary[charId] = allocatorDeviceId;
        return charId;
    }

    public static void DeallocateDeviceId(int deviceId)
    {
        var item = _dictionary.First(x => x.Value == deviceId);
        _dictionary.Remove(item.Key);
    }
    #endregion
    #endregion
}
