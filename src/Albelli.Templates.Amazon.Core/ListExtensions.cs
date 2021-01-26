using System;
using System.Collections.Generic;

namespace Albelli.Templates.Amazon.Core
{
    public static class ListExtensions
    {
        public static void Foreach<T>(this List<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }

        public static void ForeachReverse<T>(this List<T> list, Action<T> action)
        {
            for (var i = list.Count - 1; i >= 0; --i)
            {
                action(list[i]);
            }
        }
    }
}