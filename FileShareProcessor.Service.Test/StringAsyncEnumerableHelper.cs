using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileShareProcessor.Service.Test
{
    public static class StringAsyncEnumerableHelper
    {
        public static async IAsyncEnumerable<string> GetAsyncEnumerable(this string[] stringArray)
        {
            foreach (var item in stringArray)
            {
                yield return item;
            }

            await Task.CompletedTask;
        }
    }
}
