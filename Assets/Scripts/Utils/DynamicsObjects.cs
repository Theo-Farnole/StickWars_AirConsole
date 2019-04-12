using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicsObjects : Singleton<DynamicsObjects>
{    
    // just add values to enum, in order to create new parents 
    public enum ParentType { Enemies, Bullets, VFX };

    /////////////////////////////////////////////////////////
    /// DO NOT TOUCH THE REST
    /// If you make changes, it is at your own risk.
    /// Made by Theo Farnole 06 feb 2019
    /////////////////////////////////////////////////////////

    #region Internal
    private List<Transform> _parents = new List<Transform>();    
    
    void Start()
    {
        var types = Enum.GetValues(typeof(ParentType));

        // create parents, modify their name, and set their parent
        for (int i=0; i < types.Length; i++)
        {
            Transform obj = new GameObject().transform;
            obj.name = "Parent " + Enum.GetNames(typeof(ParentType))[i]; 
            obj.SetParent(transform);

            _parents.Add(obj);
        }
    }

    // set the Transform obj to the Transform parent which is equals to type
    public void SetToParent(Transform obj, ParentType type)
    {
        obj.parent = _parents[(int)type];
    }
    #endregion
}
