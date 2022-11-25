using System;
using System.Collections.Generic;
using System.Linq;

public static class ListExtension
{
    /// <summary>
    /// Inserts a ClassElement at the specified index if the index is smaller than the total length of the element list, or appends the element to the end otherwise
    /// Throws an argument exception when the index is negative
    /// </summary>
    /// <param name="elementIndex"></param>
    /// <param name="element"></param>
    public static void InsertOrAppend<T>(this List<T> list, int elementIndex, T element)
    {
        if (list.Count - 1 >= elementIndex)
        {
            list.Insert(elementIndex, element);
        }
        else
        {
            list.Add(element);
        }
    }
}
