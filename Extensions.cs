using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GobbletBot
{
    public static class Extensions
    {
        public static T[] Populate<T>(this T[] array) where T : new()
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new T();
            }
            return array;
        }
        public static T[] Populate<T>(this T[] array, Func<T> provider)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = provider();
            }
            return array;
        }
        public static int FirstIndexMatch<TItem>(this IEnumerable<TItem> items, Func<TItem, bool> matchCondition)
        {
            var index = 0;
            foreach (var item in items)
            {
                if (matchCondition.Invoke(item))
                {
                    return index;
                }
                index++;
            }
            return -1;
        }
    }
}
