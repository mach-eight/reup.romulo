using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public static class ListUtils
    {
        public static T PopLast<T>(this List<T> list)
        {
            if (list.Count == 0)
            {
                throw new InvalidOperationException("The list is empty.");
            }
            T lastItem = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return lastItem;
        }
    }
}
