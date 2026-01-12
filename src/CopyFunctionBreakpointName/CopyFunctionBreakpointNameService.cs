using System;
using System.ComponentModel.Design;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Threading;
using TextSpan = Microsoft.CodeAnalysis.Text.TextSpan;

namespace CopyFunctionBreakpointName
{
    public sealed class CopyFunctionBreakpointNameService
    {
        private static readonly CommandID MenuCommand = new CommandID(new Guid("840b69a0-a468-4950-8c25-16bb7a846a58"), 0x0100);

        private readonly IVsTextManager textManager;
        private readonly IVsEditorAdaptersFactoryService editorAdaptersFactoryService;
        private readonly IVsStatusbar statusBar;
        private readonly IVsCommandWindow commandWindow;
        private readonly JoinableTaskFactory joinableTaskFactory;

        public CopyFunctionBreakpointNameService(IVsTextManager textManager,
            IVsEditorAdaptersFactoryService editorAdaptersFactoryService,
            IMenuCommandService menuCommandService,
            IVsStatusbar statusBar,
            IVsCommandWindow commandWindow,
            JoinableTaskFactory joinableTaskFactory)
        {
            if (menuCommandService == null) throw new ArgumentNullException(nameof(menuCommandService));

            this.textManager = textManager ?? throw new ArgumentNullException(nameof(textManager));
            this.editorAdaptersFactoryService = editorAdaptersFactoryService ?? throw new ArgumentNullException(nameof(editorAdaptersFactoryService));
            this.statusBar = statusBar ?? throw new ArgumentNullException(nameof(statusBar));
            this.commandWindow = commandWindow;
            this.joinableTaskFactory = joinableTaskFactory ?? throw new ArgumentNullException(nameof(joinableTaskFactory));

            menuCommandService.AddCommand(
                new OleMenuCommand(OnMenuCommandInvoked, changeHandler: null, UpdateMenuCommandStatus, MenuCommand));
        }

        private void OnMenuCommandInvoked(object sender, EventArgs e)
        {
            joinableTaskFactory.Run(
                "Copy function breakpoint name",
                "Opening function breakpoint dialog...",
                async (progress, cancellationToken) =>
                {
                    var factory = await GetFunctionBreakpointNameFactoryAsync(cancellationToken);

                    await joinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

                    if (factory == null)
                    {
                        statusBar.SetText("Could not determine a function breakpoint name for the selected syntax");
                    }
                    else
                    {
                        // Don't wait for this to finish because it will deadlock (it waits till ExecuteCommand opens a
                        // new window, which happens after the containing JoinableTaskFactory.Run call.)
                        _ = ShowFunctionBreakpointDialogAsync(functionName: factory.ToString());
                    }
                });

            async Task ShowFunctionBreakpointDialogAsync(string functionName)
            {
                var window = await WindowUtils.WaitForNewlyOpenedWindowAsync(
                    triggerWindowOpening: () =>
                    {
                        ThreadHelper.ThrowIfNotOnUIThread();
                        commandWindow.ExecuteCommand("Debug.FunctionBreakpoint");
                    },
                    predicate: window => window.GetType().Name.Contains("Breakpoint", StringComparison.Ordinal));

                var textBox = LogicalTreeHelper.FindLogicalNode(window, "FunctionNameTextBox") as TextBox
                    // Fallback in case the name changes
                    ?? FocusManager.GetFocusedElement(window) as TextBox;

                if (textBox is not null)
                {
                    textBox.Text = functionName;
                    textBox.SelectAll();
                    textBox.Focus();
                }
                else
                {
                    Clipboard.SetText(functionName);

#pragma warning disable VSTHRD010 // This local function is always invoked on the main thread.
                    statusBar.SetText($"Could not prefill the breakpoint function name. Copied “{functionName}” to the clipboard instead.");
#pragma warning restore VSTHRD010
                }
            }
        }

        private void UpdateMenuCommandStatus(object sender, EventArgs e)
        {
            var source = new CancellationTokenSource();
            try
            {
                var task = GetFunctionBreakpointNameFactoryAsync(source.Token);

                ((MenuCommand)sender).Visible =
                    !task.TryGetResult(out var factory)
                    || factory != null;
            }
            finally
            {
                source.Cancel();
            }
        }

        private async Task<FunctionBreakpointNameFactory?> GetFunctionBreakpointNameFactoryAsync(CancellationToken cancellationToken)
        {
            ErrorHandler.ThrowOnFailure(textManager.GetActiveView(fMustHaveFocus: 1, pBuffer: null, out var view));
            var activeViewSelection = editorAdaptersFactoryService.GetWpfTextView(view).Selection;
            var document = activeViewSelection.Start.Position.Snapshot.GetOpenDocumentInCurrentContextWithChanges();

            return await FunctionBreakpointUtils.GetFunctionBreakpointNameFactoryAsync(
                await document.GetSyntaxRootAsync(cancellationToken),
                TextSpan.FromBounds(
                    activeViewSelection.Start.Position.Position,
                    activeViewSelection.End.Position.Position),
                document.GetSemanticModelAsync,
                cancellationToken);
        }
    }
}
