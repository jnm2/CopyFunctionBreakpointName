using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CopyFunctionBreakpointName;

internal static class WindowUtils
{
    public static Task<Window> WaitForNewlyOpenedWindowAsync(Action triggerWindowOpening, Func<Window, bool> predicate)
    {
        var seenWindows = Application.Current.Windows.Cast<Window>().ToHashSet();

        var searchTask = SearchLoopAsync();
        triggerWindowOpening();
        return searchTask;

        async Task<Window> SearchLoopAsync()
        {
            while (true)
            {
                await seenWindows.Single(w => w.IsActive).WhenDeactivatedAsync();

                foreach (Window window in Application.Current.Windows)
                {
                    if (seenWindows.Add(window))
                    {
                        if (predicate(window))
                            return window;
                    }
                }
            }
        }
    }

    public static Task WhenDeactivatedAsync(this Window window)
    {
        var taskCompletionSource = new TaskCompletionSource<object>();
        window.Deactivated += OnDeactivated;
        return taskCompletionSource.Task;

        void OnDeactivated(object sender, EventArgs e)
        {
            var activeWindow = (Window)sender;
            activeWindow.Deactivated -= OnDeactivated;
            taskCompletionSource.SetResult(null);
        }
    }
    /*
    public static T FindDescendant<T>(this FrameworkElement frameworkElement, Func<T, bool> predicate)
    {
        LogicalTreeHelper.FindLogicalNode
    }*/
}
