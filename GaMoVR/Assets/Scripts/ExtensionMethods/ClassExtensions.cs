using UnityEngine;

public static class ClassExtensions
{/*
    public static Transform FindChildRecursive(this Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Contains(name))
                return child;

            var result = child.FindChildRecursive(name);
            if (result != null)
                return result;
        }
        return null;
    }*/

    
    public static T FindObjectInParentRecursive<T>(this Transform child) where T : Object
    {

        if (!(child.parent is null))
        {
            T component = child.GetComponentInParent<T>();
            if (!(component is null))
            {
                return component;
            }

            return child.parent.FindObjectInParentRecursive<T>();
        }
        return null;
    }
}