namespace Fanex.NAP.SPU.Core
{
    using System.Collections.Generic;
    using System.Linq;

    public static class Extensions
    {
        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> items, int partitionSize)
        {
            return items.Select((item, inx) => new { item, inx })
                        .GroupBy(x => x.inx / partitionSize)
                        .Select(g => g.Select(x => x.item));
        }

        public static T PopAt<T>(this List<T> list, int index)
        {
            T r = list[index];
            list.RemoveAt(index);

            return r;
        }
    }
}