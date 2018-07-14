using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;

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

        public static ResumeOnMainThreadAwaitable<T> ResumeOnMainThread<T>(this Task<T> task, JoinableTaskFactory resumeOn)
        {
            return new ResumeOnMainThreadAwaitable<T>(task, resumeOn);
        }

        public readonly struct ResumeOnMainThreadAwaitable<T>
        {
            private readonly Task<T> task;
            private readonly JoinableTaskFactory.MainThreadAwaitable resumeAwaitable;

            public ResumeOnMainThreadAwaitable(Task<T> task, JoinableTaskFactory resumeOn)
            {
                this.task = task ?? throw new ArgumentNullException(nameof(task));
                if (resumeOn == null) throw new ArgumentNullException(nameof(resumeOn));
                resumeAwaitable = resumeOn.SwitchToMainThreadAsync();
            }

            public ResumeOnMainThreadAwaiter GetAwaiter()
            {
                return new ResumeOnMainThreadAwaiter(task.GetAwaiter(), resumeAwaitable.GetAwaiter());
            }

            public readonly struct ResumeOnMainThreadAwaiter : INotifyCompletion
            {
                private readonly TaskAwaiter<T> taskAwaiter;
                private readonly JoinableTaskFactory.MainThreadAwaiter resumeAwaiter;

                public ResumeOnMainThreadAwaiter(TaskAwaiter<T> taskAwaiter, JoinableTaskFactory.MainThreadAwaiter resumeAwaiter)
                {
                    this.taskAwaiter = taskAwaiter;
                    this.resumeAwaiter = resumeAwaiter;
                }

                public bool IsCompleted => taskAwaiter.IsCompleted && resumeAwaiter.IsCompleted;

                public void OnCompleted(Action continuation)
                {
                    var resumeAwaiter = this.resumeAwaiter;

                    taskAwaiter.OnCompleted(() => resumeAwaiter.OnCompleted(continuation));
                }

                public T GetResult() => taskAwaiter.GetResult();
            }
        }
    }
}
