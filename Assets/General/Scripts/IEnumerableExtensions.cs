using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GeneralHelpers
{
    public static class IEnumerableExtensions 
    {
        public static Vector3 Sum(this IEnumerable<Vector3> source) => source.Aggregate(Vector3.zero, (current, v) => current + v);
        public static Vector3 Average(this IEnumerable<Vector3> source)
        {
            var sum = Vector3.zero;
            var count = 0;
            foreach (var v in source)
            {
                sum += v;
                count++;
            }
            return sum / count;
        } 
    }
}