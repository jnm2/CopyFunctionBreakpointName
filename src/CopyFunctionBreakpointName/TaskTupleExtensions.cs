using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CopyFunctionBreakpointName
{
    public static class TaskTupleExtensions
    {
        #region (Task<T1>)

        public static TaskAwaiter<T1> GetAwaiter<T1>(this ValueTuple<Task<T1>> tasks)
        {
            return tasks.Item1.GetAwaiter();
        }

        public static ConfiguredTaskAwaitable<T1> ConfigureAwait<T1>(this ValueTuple<Task<T1>> tasks, bool continueOnCapturedContext)
        {
            return tasks.Item1.ConfigureAwait(continueOnCapturedContext);
        }

        #endregion

        #region (Task<T1>..Task<T2>)

        public static TupleTaskAwaiter<T1, T2> GetAwaiter<T1, T2>(this (Task<T1>, Task<T2>) tasks)
        {
            return new TupleTaskAwaiter<T1, T2>(tasks);
        }

        public struct TupleTaskAwaiter<T1, T2> : ICriticalNotifyCompletion
        {
            private readonly (Task<T1>, Task<T2>) tasks;
            private readonly TaskAwaiter whenAllAwaiter;

            public TupleTaskAwaiter((Task<T1>, Task<T2>) tasks)
            {
                this.tasks = tasks;
                whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2).GetAwaiter();
            }

            public bool IsCompleted => whenAllAwaiter.IsCompleted;
            public void OnCompleted(Action continuation) => whenAllAwaiter.OnCompleted(continuation);
            public void UnsafeOnCompleted(Action continuation) => whenAllAwaiter.UnsafeOnCompleted(continuation);

            public (T1, T2) GetResult()
            {
                whenAllAwaiter.GetResult();
                return (tasks.Item1.Result, tasks.Item2.Result);
            }
        }

        public static TupleConfiguredTaskAwaitable<T1, T2> ConfigureAwait<T1, T2>(this (Task<T1>, Task<T2>) tasks, bool continueOnCapturedContext)
        {
            return new TupleConfiguredTaskAwaitable<T1, T2>(tasks, continueOnCapturedContext);
        }

        public struct TupleConfiguredTaskAwaitable<T1, T2>
        {
            private readonly (Task<T1>, Task<T2>) tasks;
            private readonly bool continueOnCapturedContext;

            public TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>) tasks, bool continueOnCapturedContext)
            {
                this.tasks = tasks;
                this.continueOnCapturedContext = continueOnCapturedContext;
            }

            public Awaiter GetAwaiter() => new Awaiter(tasks, continueOnCapturedContext);

            public struct Awaiter : ICriticalNotifyCompletion
            {
                private readonly (Task<T1>, Task<T2>) tasks;
                private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter whenAllAwaiter;

                public Awaiter((Task<T1>, Task<T2>) tasks, bool continueOnCapturedContext)
                {
                    this.tasks = tasks;
                    whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2).ConfigureAwait(continueOnCapturedContext).GetAwaiter();
                }

                public bool IsCompleted => whenAllAwaiter.IsCompleted;
                public void OnCompleted(Action continuation) => whenAllAwaiter.OnCompleted(continuation);
                public void UnsafeOnCompleted(Action continuation) => whenAllAwaiter.UnsafeOnCompleted(continuation);

                public (T1, T2) GetResult()
                {
                    whenAllAwaiter.GetResult();
                    return (tasks.Item1.Result, tasks.Item2.Result);
                }
            }
        }

        #endregion

        #region (Task<T1>..Task<T3>)

        public static TupleTaskAwaiter<T1, T2, T3> GetAwaiter<T1, T2, T3>(this (Task<T1>, Task<T2>, Task<T3>) tasks)
        {
            return new TupleTaskAwaiter<T1, T2, T3>(tasks);
        }

        public struct TupleTaskAwaiter<T1, T2, T3> : ICriticalNotifyCompletion
        {
            private readonly (Task<T1>, Task<T2>, Task<T3>) tasks;
            private readonly TaskAwaiter whenAllAwaiter;

            public TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>) tasks)
            {
                this.tasks = tasks;
                whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3).GetAwaiter();
            }

            public bool IsCompleted => whenAllAwaiter.IsCompleted;
            public void OnCompleted(Action continuation) => whenAllAwaiter.OnCompleted(continuation);
            public void UnsafeOnCompleted(Action continuation) => whenAllAwaiter.UnsafeOnCompleted(continuation);

            public (T1, T2, T3) GetResult()
            {
                whenAllAwaiter.GetResult();
                return (tasks.Item1.Result, tasks.Item2.Result, tasks.Item3.Result);
            }
        }

        public static TupleConfiguredTaskAwaitable<T1, T2, T3> ConfigureAwait<T1, T2, T3>(this (Task<T1>, Task<T2>, Task<T3>) tasks, bool continueOnCapturedContext)
        {
            return new TupleConfiguredTaskAwaitable<T1, T2, T3>(tasks, continueOnCapturedContext);
        }

        public struct TupleConfiguredTaskAwaitable<T1, T2, T3>
        {
            private readonly (Task<T1>, Task<T2>, Task<T3>) tasks;
            private readonly bool continueOnCapturedContext;

            public TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>, Task<T3>) tasks, bool continueOnCapturedContext)
            {
                this.tasks = tasks;
                this.continueOnCapturedContext = continueOnCapturedContext;
            }

            public Awaiter GetAwaiter() => new Awaiter(tasks, continueOnCapturedContext);

            public struct Awaiter : ICriticalNotifyCompletion
            {
                private readonly (Task<T1>, Task<T2>, Task<T3>) tasks;
                private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter whenAllAwaiter;

                public Awaiter((Task<T1>, Task<T2>, Task<T3>) tasks, bool continueOnCapturedContext)
                {
                    this.tasks = tasks;
                    whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2).ConfigureAwait(continueOnCapturedContext).GetAwaiter();
                }

                public bool IsCompleted => whenAllAwaiter.IsCompleted;
                public void OnCompleted(Action continuation) => whenAllAwaiter.OnCompleted(continuation);
                public void UnsafeOnCompleted(Action continuation) => whenAllAwaiter.UnsafeOnCompleted(continuation);

                public (T1, T2, T3) GetResult()
                {
                    whenAllAwaiter.GetResult();
                    return (tasks.Item1.Result, tasks.Item2.Result, tasks.Item3.Result);
                }
            }
        }

        #endregion

        #region (Task<T1>..Task<T4>)

        public static TupleTaskAwaiter<T1, T2, T3, T4> GetAwaiter<T1, T2, T3, T4>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>) tasks)
        {
            return new TupleTaskAwaiter<T1, T2, T3, T4>(tasks);
        }

        public struct TupleTaskAwaiter<T1, T2, T3, T4> : ICriticalNotifyCompletion
        {
            private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>) tasks;
            private readonly TaskAwaiter whenAllAwaiter;

            public TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>) tasks)
            {
                this.tasks = tasks;
                whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4).GetAwaiter();
            }

            public bool IsCompleted => whenAllAwaiter.IsCompleted;
            public void OnCompleted(Action continuation) => whenAllAwaiter.OnCompleted(continuation);
            public void UnsafeOnCompleted(Action continuation) => whenAllAwaiter.UnsafeOnCompleted(continuation);

            public (T1, T2, T3, T4) GetResult()
            {
                whenAllAwaiter.GetResult();
                return (tasks.Item1.Result, tasks.Item2.Result, tasks.Item3.Result, tasks.Item4.Result);
            }
        }

        public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4> ConfigureAwait<T1, T2, T3, T4>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>) tasks, bool continueOnCapturedContext)
        {
            return new TupleConfiguredTaskAwaitable<T1, T2, T3, T4>(tasks, continueOnCapturedContext);
        }

        public struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4>
        {
            private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>) tasks;
            private readonly bool continueOnCapturedContext;

            public TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>, Task<T3>, Task<T4>) tasks, bool continueOnCapturedContext)
            {
                this.tasks = tasks;
                this.continueOnCapturedContext = continueOnCapturedContext;
            }

            public Awaiter GetAwaiter() => new Awaiter(tasks, continueOnCapturedContext);

            public struct Awaiter : ICriticalNotifyCompletion
            {
                private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>) tasks;
                private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter whenAllAwaiter;

                public Awaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>) tasks, bool continueOnCapturedContext)
                {
                    this.tasks = tasks;
                    whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2).ConfigureAwait(continueOnCapturedContext).GetAwaiter();
                }

                public bool IsCompleted => whenAllAwaiter.IsCompleted;
                public void OnCompleted(Action continuation) => whenAllAwaiter.OnCompleted(continuation);
                public void UnsafeOnCompleted(Action continuation) => whenAllAwaiter.UnsafeOnCompleted(continuation);

                public (T1, T2, T3, T4) GetResult()
                {
                    whenAllAwaiter.GetResult();
                    return (tasks.Item1.Result, tasks.Item2.Result, tasks.Item3.Result, tasks.Item4.Result);
                }
            }
        }

        #endregion

        #region (Task<T1>..Task<T5>)

        public static TupleTaskAwaiter<T1, T2, T3, T4, T5> GetAwaiter<T1, T2, T3, T4, T5>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) tasks)
        {
            return new TupleTaskAwaiter<T1, T2, T3, T4, T5>(tasks);
        }

        public struct TupleTaskAwaiter<T1, T2, T3, T4, T5> : ICriticalNotifyCompletion
        {
            private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) tasks;
            private readonly TaskAwaiter whenAllAwaiter;

            public TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) tasks)
            {
                this.tasks = tasks;
                whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5).GetAwaiter();
            }

            public bool IsCompleted => whenAllAwaiter.IsCompleted;
            public void OnCompleted(Action continuation) => whenAllAwaiter.OnCompleted(continuation);
            public void UnsafeOnCompleted(Action continuation) => whenAllAwaiter.UnsafeOnCompleted(continuation);

            public (T1, T2, T3, T4, T5) GetResult()
            {
                whenAllAwaiter.GetResult();
                return (tasks.Item1.Result, tasks.Item2.Result, tasks.Item3.Result, tasks.Item4.Result, tasks.Item5.Result);
            }
        }

        public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5> ConfigureAwait<T1, T2, T3, T4, T5>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) tasks, bool continueOnCapturedContext)
        {
            return new TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5>(tasks, continueOnCapturedContext);
        }

        public struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5>
        {
            private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) tasks;
            private readonly bool continueOnCapturedContext;

            public TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) tasks, bool continueOnCapturedContext)
            {
                this.tasks = tasks;
                this.continueOnCapturedContext = continueOnCapturedContext;
            }

            public Awaiter GetAwaiter() => new Awaiter(tasks, continueOnCapturedContext);

            public struct Awaiter : ICriticalNotifyCompletion
            {
                private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) tasks;
                private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter whenAllAwaiter;

                public Awaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>) tasks, bool continueOnCapturedContext)
                {
                    this.tasks = tasks;
                    whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2).ConfigureAwait(continueOnCapturedContext).GetAwaiter();
                }

                public bool IsCompleted => whenAllAwaiter.IsCompleted;
                public void OnCompleted(Action continuation) => whenAllAwaiter.OnCompleted(continuation);
                public void UnsafeOnCompleted(Action continuation) => whenAllAwaiter.UnsafeOnCompleted(continuation);

                public (T1, T2, T3, T4, T5) GetResult()
                {
                    whenAllAwaiter.GetResult();
                    return (tasks.Item1.Result, tasks.Item2.Result, tasks.Item3.Result, tasks.Item4.Result, tasks.Item5.Result);
                }
            }
        }

        #endregion

        #region (Task<T1>..Task<T6>)

        public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6> GetAwaiter<T1, T2, T3, T4, T5, T6>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) tasks)
        {
            return new TupleTaskAwaiter<T1, T2, T3, T4, T5, T6>(tasks);
        }

        public struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6> : ICriticalNotifyCompletion
        {
            private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) tasks;
            private readonly TaskAwaiter whenAllAwaiter;

            public TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) tasks)
            {
                this.tasks = tasks;
                whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6).GetAwaiter();
            }

            public bool IsCompleted => whenAllAwaiter.IsCompleted;
            public void OnCompleted(Action continuation) => whenAllAwaiter.OnCompleted(continuation);
            public void UnsafeOnCompleted(Action continuation) => whenAllAwaiter.UnsafeOnCompleted(continuation);

            public (T1, T2, T3, T4, T5, T6) GetResult()
            {
                whenAllAwaiter.GetResult();
                return (tasks.Item1.Result, tasks.Item2.Result, tasks.Item3.Result, tasks.Item4.Result, tasks.Item5.Result, tasks.Item6.Result);
            }
        }

        public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6> ConfigureAwait<T1, T2, T3, T4, T5, T6>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) tasks, bool continueOnCapturedContext)
        {
            return new TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6>(tasks, continueOnCapturedContext);
        }

        public struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6>
        {
            private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) tasks;
            private readonly bool continueOnCapturedContext;

            public TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) tasks, bool continueOnCapturedContext)
            {
                this.tasks = tasks;
                this.continueOnCapturedContext = continueOnCapturedContext;
            }

            public Awaiter GetAwaiter() => new Awaiter(tasks, continueOnCapturedContext);

            public struct Awaiter : ICriticalNotifyCompletion
            {
                private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) tasks;
                private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter whenAllAwaiter;

                public Awaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>) tasks, bool continueOnCapturedContext)
                {
                    this.tasks = tasks;
                    whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2).ConfigureAwait(continueOnCapturedContext).GetAwaiter();
                }

                public bool IsCompleted => whenAllAwaiter.IsCompleted;
                public void OnCompleted(Action continuation) => whenAllAwaiter.OnCompleted(continuation);
                public void UnsafeOnCompleted(Action continuation) => whenAllAwaiter.UnsafeOnCompleted(continuation);

                public (T1, T2, T3, T4, T5, T6) GetResult()
                {
                    whenAllAwaiter.GetResult();
                    return (tasks.Item1.Result, tasks.Item2.Result, tasks.Item3.Result, tasks.Item4.Result, tasks.Item5.Result, tasks.Item6.Result);
                }
            }
        }

        #endregion

        #region (Task<T1>..Task<T7>)

        public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7> GetAwaiter<T1, T2, T3, T4, T5, T6, T7>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) tasks)
        {
            return new TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7>(tasks);
        }

        public struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7> : ICriticalNotifyCompletion
        {
            private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) tasks;
            private readonly TaskAwaiter whenAllAwaiter;

            public TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) tasks)
            {
                this.tasks = tasks;
                whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7).GetAwaiter();
            }

            public bool IsCompleted => whenAllAwaiter.IsCompleted;
            public void OnCompleted(Action continuation) => whenAllAwaiter.OnCompleted(continuation);
            public void UnsafeOnCompleted(Action continuation) => whenAllAwaiter.UnsafeOnCompleted(continuation);

            public (T1, T2, T3, T4, T5, T6, T7) GetResult()
            {
                whenAllAwaiter.GetResult();
                return (tasks.Item1.Result, tasks.Item2.Result, tasks.Item3.Result, tasks.Item4.Result, tasks.Item5.Result, tasks.Item6.Result, tasks.Item7.Result);
            }
        }

        public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) tasks, bool continueOnCapturedContext)
        {
            return new TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7>(tasks, continueOnCapturedContext);
        }

        public struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7>
        {
            private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) tasks;
            private readonly bool continueOnCapturedContext;

            public TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) tasks, bool continueOnCapturedContext)
            {
                this.tasks = tasks;
                this.continueOnCapturedContext = continueOnCapturedContext;
            }

            public Awaiter GetAwaiter() => new Awaiter(tasks, continueOnCapturedContext);

            public struct Awaiter : ICriticalNotifyCompletion
            {
                private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) tasks;
                private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter whenAllAwaiter;

                public Awaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>) tasks, bool continueOnCapturedContext)
                {
                    this.tasks = tasks;
                    whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2).ConfigureAwait(continueOnCapturedContext).GetAwaiter();
                }

                public bool IsCompleted => whenAllAwaiter.IsCompleted;
                public void OnCompleted(Action continuation) => whenAllAwaiter.OnCompleted(continuation);
                public void UnsafeOnCompleted(Action continuation) => whenAllAwaiter.UnsafeOnCompleted(continuation);

                public (T1, T2, T3, T4, T5, T6, T7) GetResult()
                {
                    whenAllAwaiter.GetResult();
                    return (tasks.Item1.Result, tasks.Item2.Result, tasks.Item3.Result, tasks.Item4.Result, tasks.Item5.Result, tasks.Item6.Result, tasks.Item7.Result);
                }
            }
        }

        #endregion

        #region (Task<T1>..Task<T8>)

        public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8> GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) tasks)
        {
            return new TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8>(tasks);
        }

        public struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8> : ICriticalNotifyCompletion
        {
            private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) tasks;
            private readonly TaskAwaiter whenAllAwaiter;

            public TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) tasks)
            {
                this.tasks = tasks;
                whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8).GetAwaiter();
            }

            public bool IsCompleted => whenAllAwaiter.IsCompleted;
            public void OnCompleted(Action continuation) => whenAllAwaiter.OnCompleted(continuation);
            public void UnsafeOnCompleted(Action continuation) => whenAllAwaiter.UnsafeOnCompleted(continuation);

            public (T1, T2, T3, T4, T5, T6, T7, T8) GetResult()
            {
                whenAllAwaiter.GetResult();
                return (tasks.Item1.Result, tasks.Item2.Result, tasks.Item3.Result, tasks.Item4.Result, tasks.Item5.Result, tasks.Item6.Result, tasks.Item7.Result, tasks.Item8.Result);
            }
        }

        public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) tasks, bool continueOnCapturedContext)
        {
            return new TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8>(tasks, continueOnCapturedContext);
        }

        public struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8>
        {
            private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) tasks;
            private readonly bool continueOnCapturedContext;

            public TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) tasks, bool continueOnCapturedContext)
            {
                this.tasks = tasks;
                this.continueOnCapturedContext = continueOnCapturedContext;
            }

            public Awaiter GetAwaiter() => new Awaiter(tasks, continueOnCapturedContext);

            public struct Awaiter : ICriticalNotifyCompletion
            {
                private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) tasks;
                private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter whenAllAwaiter;

                public Awaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>) tasks, bool continueOnCapturedContext)
                {
                    this.tasks = tasks;
                    whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2).ConfigureAwait(continueOnCapturedContext).GetAwaiter();
                }

                public bool IsCompleted => whenAllAwaiter.IsCompleted;
                public void OnCompleted(Action continuation) => whenAllAwaiter.OnCompleted(continuation);
                public void UnsafeOnCompleted(Action continuation) => whenAllAwaiter.UnsafeOnCompleted(continuation);

                public (T1, T2, T3, T4, T5, T6, T7, T8) GetResult()
                {
                    whenAllAwaiter.GetResult();
                    return (tasks.Item1.Result, tasks.Item2.Result, tasks.Item3.Result, tasks.Item4.Result, tasks.Item5.Result, tasks.Item6.Result, tasks.Item7.Result, tasks.Item8.Result);
                }
            }
        }

        #endregion

        #region (Task<T1>..Task<T9>)

        public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9> GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>) tasks)
        {
            return new TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9>(tasks);
        }

        public struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9> : ICriticalNotifyCompletion
        {
            private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>) tasks;
            private readonly TaskAwaiter whenAllAwaiter;

            public TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>) tasks)
            {
                this.tasks = tasks;
                whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9).GetAwaiter();
            }

            public bool IsCompleted => whenAllAwaiter.IsCompleted;
            public void OnCompleted(Action continuation) => whenAllAwaiter.OnCompleted(continuation);
            public void UnsafeOnCompleted(Action continuation) => whenAllAwaiter.UnsafeOnCompleted(continuation);

            public (T1, T2, T3, T4, T5, T6, T7, T8, T9) GetResult()
            {
                whenAllAwaiter.GetResult();
                return (tasks.Item1.Result, tasks.Item2.Result, tasks.Item3.Result, tasks.Item4.Result, tasks.Item5.Result, tasks.Item6.Result, tasks.Item7.Result, tasks.Item8.Result, tasks.Item9.Result);
            }
        }

        public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>) tasks, bool continueOnCapturedContext)
        {
            return new TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9>(tasks, continueOnCapturedContext);
        }

        public struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9>
        {
            private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>) tasks;
            private readonly bool continueOnCapturedContext;

            public TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>) tasks, bool continueOnCapturedContext)
            {
                this.tasks = tasks;
                this.continueOnCapturedContext = continueOnCapturedContext;
            }

            public Awaiter GetAwaiter() => new Awaiter(tasks, continueOnCapturedContext);

            public struct Awaiter : ICriticalNotifyCompletion
            {
                private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>) tasks;
                private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter whenAllAwaiter;

                public Awaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>) tasks, bool continueOnCapturedContext)
                {
                    this.tasks = tasks;
                    whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2).ConfigureAwait(continueOnCapturedContext).GetAwaiter();
                }

                public bool IsCompleted => whenAllAwaiter.IsCompleted;
                public void OnCompleted(Action continuation) => whenAllAwaiter.OnCompleted(continuation);
                public void UnsafeOnCompleted(Action continuation) => whenAllAwaiter.UnsafeOnCompleted(continuation);

                public (T1, T2, T3, T4, T5, T6, T7, T8, T9) GetResult()
                {
                    whenAllAwaiter.GetResult();
                    return (tasks.Item1.Result, tasks.Item2.Result, tasks.Item3.Result, tasks.Item4.Result, tasks.Item5.Result, tasks.Item6.Result, tasks.Item7.Result, tasks.Item8.Result, tasks.Item9.Result);
                }
            }
        }

        #endregion

        #region (Task<T1>..Task<T10>)

        public static TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> GetAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>) tasks)
        {
            return new TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(tasks);
        }

        public struct TupleTaskAwaiter<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : ICriticalNotifyCompletion
        {
            private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>) tasks;
            private readonly TaskAwaiter whenAllAwaiter;

            public TupleTaskAwaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>) tasks)
            {
                this.tasks = tasks;
                whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10).GetAwaiter();
            }

            public bool IsCompleted => whenAllAwaiter.IsCompleted;
            public void OnCompleted(Action continuation) => whenAllAwaiter.OnCompleted(continuation);
            public void UnsafeOnCompleted(Action continuation) => whenAllAwaiter.UnsafeOnCompleted(continuation);

            public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10) GetResult()
            {
                whenAllAwaiter.GetResult();
                return (tasks.Item1.Result, tasks.Item2.Result, tasks.Item3.Result, tasks.Item4.Result, tasks.Item5.Result, tasks.Item6.Result, tasks.Item7.Result, tasks.Item8.Result, tasks.Item9.Result, tasks.Item10.Result);
            }
        }

        public static TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> ConfigureAwait<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>) tasks, bool continueOnCapturedContext)
        {
            return new TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(tasks, continueOnCapturedContext);
        }

        public struct TupleConfiguredTaskAwaitable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
        {
            private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>) tasks;
            private readonly bool continueOnCapturedContext;

            public TupleConfiguredTaskAwaitable((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>) tasks, bool continueOnCapturedContext)
            {
                this.tasks = tasks;
                this.continueOnCapturedContext = continueOnCapturedContext;
            }

            public Awaiter GetAwaiter() => new Awaiter(tasks, continueOnCapturedContext);

            public struct Awaiter : ICriticalNotifyCompletion
            {
                private readonly (Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>) tasks;
                private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter whenAllAwaiter;

                public Awaiter((Task<T1>, Task<T2>, Task<T3>, Task<T4>, Task<T5>, Task<T6>, Task<T7>, Task<T8>, Task<T9>, Task<T10>) tasks, bool continueOnCapturedContext)
                {
                    this.tasks = tasks;
                    whenAllAwaiter = Task.WhenAll(tasks.Item1, tasks.Item2).ConfigureAwait(continueOnCapturedContext).GetAwaiter();
                }

                public bool IsCompleted => whenAllAwaiter.IsCompleted;
                public void OnCompleted(Action continuation) => whenAllAwaiter.OnCompleted(continuation);
                public void UnsafeOnCompleted(Action continuation) => whenAllAwaiter.UnsafeOnCompleted(continuation);

                public (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10) GetResult()
                {
                    whenAllAwaiter.GetResult();
                    return (tasks.Item1.Result, tasks.Item2.Result, tasks.Item3.Result, tasks.Item4.Result, tasks.Item5.Result, tasks.Item6.Result, tasks.Item7.Result, tasks.Item8.Result, tasks.Item9.Result, tasks.Item10.Result);
                }
            }
        }

        #endregion

        #region Task

        public static TaskAwaiter GetAwaiter(this ValueTuple<Task> tasks)
        {
            return tasks.Item1.GetAwaiter();
        }

        public static ConfiguredTaskAwaitable ConfigureAwait(this ValueTuple<Task> tasks, bool continueOnCapturedContext)
        {
            return tasks.Item1.ConfigureAwait(continueOnCapturedContext);
        }

        public static TaskAwaiter GetAwaiter(this (Task, Task) tasks)
        {
            return Task.WhenAll(tasks.Item1, tasks.Item2).GetAwaiter();
        }

        public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task) tasks, bool continueOnCapturedContext)
        {
            return Task.WhenAll(tasks.Item1, tasks.Item2).ConfigureAwait(continueOnCapturedContext);
        }

        public static TaskAwaiter GetAwaiter(this (Task, Task, Task) tasks)
        {
            return Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3).GetAwaiter();
        }

        public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task) tasks, bool continueOnCapturedContext)
        {
            return Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3).ConfigureAwait(continueOnCapturedContext);
        }

        public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task) tasks)
        {
            return Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4).GetAwaiter();
        }

        public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task) tasks, bool continueOnCapturedContext)
        {
            return Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4).ConfigureAwait(continueOnCapturedContext);
        }

        public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task) tasks)
        {
            return Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5).GetAwaiter();
        }

        public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext)
        {
            return Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5).ConfigureAwait(continueOnCapturedContext);
        }

        public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task) tasks)
        {
            return Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6).GetAwaiter();
        }

        public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext)
        {
            return Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6).ConfigureAwait(continueOnCapturedContext);
        }

        public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task) tasks)
        {
            return Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7).GetAwaiter();
        }

        public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext)
        {
            return Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7).ConfigureAwait(continueOnCapturedContext);
        }

        public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task, Task) tasks)
        {
            return Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8).GetAwaiter();
        }

        public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext)
        {
            return Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8).ConfigureAwait(continueOnCapturedContext);
        }

        public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks)
        {
            return Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9).GetAwaiter();
        }

        public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext)
        {
            return Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9).ConfigureAwait(continueOnCapturedContext);
        }

        public static TaskAwaiter GetAwaiter(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks)
        {
            return Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10).GetAwaiter();
        }

        public static ConfiguredTaskAwaitable ConfigureAwait(this (Task, Task, Task, Task, Task, Task, Task, Task, Task, Task) tasks, bool continueOnCapturedContext)
        {
            return Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3, tasks.Item4, tasks.Item5, tasks.Item6, tasks.Item7, tasks.Item8, tasks.Item9, tasks.Item10).ConfigureAwait(continueOnCapturedContext);
        }

        #endregion
    }
}
