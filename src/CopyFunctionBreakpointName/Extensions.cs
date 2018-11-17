using System.Threading.Tasks;

namespace CopyFunctionBreakpointName
{
    internal static class Extensions
    {
        public static bool TryGetResult<T>(this Task<T> task, out T result)
        {
            var awaiter = task.GetAwaiter();
            if (awaiter.IsCompleted)
            {
#pragma warning disable VSTHRD002 // This is guaranteed not to block.
                result = awaiter.GetResult();
#pragma warning restore VSTHRD002
                return true;
            }

            result = default;
            return false;
        }
    }
}
