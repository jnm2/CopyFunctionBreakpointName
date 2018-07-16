using System;
using System.ComponentModel.Design;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Threading;
using TextSpan = Microsoft.CodeAnalysis.Text.TextSpan;

namespace CopyFunctionBreakpointName
{
    internal sealed class CopyFunctionBreakpointNameCommand
    {
        public const int CommandId = 0x0100;
        public static readonly Guid CommandSet = new Guid("840b69a0-a468-4950-8c25-16bb7a846a58");

        private readonly AsyncPackage package;
        private readonly AsyncLazy<(IVsTextManager, IVsEditorAdaptersFactoryService)> lazyServices;

        private CopyFunctionBreakpointNameCommand(AsyncPackage package, AsyncLazy<(IVsTextManager, IVsEditorAdaptersFactoryService)> lazyServices)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            this.lazyServices = lazyServices;
        }

        public static MenuCommand Create(
            AsyncPackage package,
            AsyncLazy<(IVsTextManager textManager, IVsEditorAdaptersFactoryService editorAdaptersFactoryService)> lazyServices)
        {
            var closure = new CopyFunctionBreakpointNameCommand(package, lazyServices);

            return new OleMenuCommand(
                closure.InvokeHandler,
                changeHandler: null,
                closure.BeforeQueryStatus,
                new CommandID(CommandSet, CommandId));
        }

        private void BeforeQueryStatus(object sender, EventArgs e)
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

        private void InvokeHandler(object sender, EventArgs e)
        {
            package.JoinableTaskFactory.Run(
                "Copy function breakpoint name",
                "Copying the function breakpoint name to the clipboard...",
                async (progress, cancellationToken) =>
                {
                    var factory = await GetFunctionBreakpointNameFactoryAsync(cancellationToken);

                    if (factory != null)
                        Clipboard.SetText(factory.Value.ToString());
                });
        }

        private async Task<FunctionBreakpointNameFactory?> GetFunctionBreakpointNameFactoryAsync(CancellationToken cancellationToken)
        {
            var (textManager, editorAdaptersFactoryService) = await lazyServices.GetValueAsync(cancellationToken);

            ErrorHandler.ThrowOnFailure(textManager.GetActiveView(fMustHaveFocus: 1, pBuffer: null, out var view));
            var activeViewSelection = editorAdaptersFactoryService.GetWpfTextView(view).Selection;
            var document = activeViewSelection.Start.Position.Snapshot.GetOpenDocumentInCurrentContextWithChanges();

            return FunctionBreakpointUtils.GetFunctionBreakpointNameFactory(
                await document.GetSyntaxRootAsync(cancellationToken),
                TextSpan.FromBounds(
                    activeViewSelection.Start.Position.Position,
                    activeViewSelection.End.Position.Position));
        }
    }
}
