
namespace PBT
{
    internal static class Extensions
    {
        internal static void Shuffle<T, DataType>(this T[] array, TaskContext<DataType> context)
        {
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = context.Random.Next(n + 1);
                T value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
        }
    }
}
