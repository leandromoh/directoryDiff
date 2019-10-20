using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoreLinq;

namespace helloworld
{
    static class Program
    {
        static void Main(string[] args)
        {
            var dirA = @"C:\Users\leandro\Dropbox";
            var dirB = @"F:\Dropbox";

            var filesA = GetFilesFrom(dirA);
            var filesB = GetFilesFrom(dirB);

            var getRelativeNameA = (Func<FileInfo, string>) (f => f.FullName.Substring(dirA.Length));
            var getRelativeNameB = (Func<FileInfo, string>) (f => f.FullName.Substring(dirB.Length));

            var relativeNamesA = filesA.Select(getRelativeNameA);
            var relativeNamesB = filesB.Select(getRelativeNameB);

            var onlyInA = relativeNamesA.Except(relativeNamesB);
            var onlyInB = relativeNamesB.Except(relativeNamesA);

            var existInBoth = relativeNamesA.Intersect(relativeNamesB);

            var dicA = filesA.ToDictionary(getRelativeNameA, f => f.Length);
            var dicB = filesB.ToDictionary(getRelativeNameB, f => f.Length);

            Console.WriteLine("------------------------");
            Console.WriteLine("exists only in A");
            foreach(var file in onlyInA)
            {
                Console.WriteLine(file);
            }

            Console.WriteLine("------------------------");
            Console.WriteLine("exists only in B");
            foreach(var file in onlyInB)
            {
                Console.WriteLine(file);
            }

            Console.WriteLine("------------------------");
            Console.WriteLine("exists in both but having different lengths");
            foreach(var file in existInBoth)
            {
                var lengthA = dicA[file];
                var lengthB = dicB[file];

                if (lengthA != lengthB)
                {
                    Console.WriteLine($"{file} | length in A = {lengthA} | length B = {lengthB}");
                }
            }
        }

        public static FileInfo[] GetFilesFrom(string dir) =>
            Directory
                .GetFiles(dir, "*.*", SearchOption.AllDirectories)
                .Select(x => new FileInfo(x))
                .ToArray();

        public static IEnumerable<TSource> IntersectBy<TSource, TKey>(
            this IEnumerable<TSource> first,
            IEnumerable<TSource> second,
            Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> keyComparer = null)
        {

            keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;

            var keys = new HashSet<TKey>(keyComparer);
            foreach (var item in first)
            {
                var k = keySelector(item);
                if (keys.Add(k))
                {
                    yield return item;
                    keys.Remove(k);
                }
            }
        }
    }
}
