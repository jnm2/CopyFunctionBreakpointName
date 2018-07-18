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
    public sealed class CopyFunctionBreakpointNameService
    {
        private static readonly CommandID MenuCommand = new CommandID(new Guid("840b69a0-a468-4950-8c25-16bb7a846a58"), 0x0100);

        private readonly IVsTextManager textManager;
        private readonly IVsEditorAdaptersFactoryService editorAdaptersFactoryService;
        private readonly JoinableTaskFactory joinableTaskFactory;

        public CopyFunctionBreakpointNameService(
            IVsTextManager textManager,
            IVsEditorAdaptersFactoryService editorAdaptersFactoryService,
            IMenuCommandService menuCommandService,
            JoinableTaskFactory joinableTaskFactory)
        {
            if (menuCommandService == null) throw new ArgumentNullException(nameof(menuCommandService));

            this.textManager = textManager ?? throw new ArgumentNullException(nameof(textManager));
            this.editorAdaptersFactoryService = editorAdaptersFactoryService ?? throw new ArgumentNullException(nameof(editorAdaptersFactoryService));
            this.joinableTaskFactory = joinableTaskFactory ?? throw new ArgumentNullException(nameof(joinableTaskFactory));

            menuCommandService.AddCommand(
                new OleMenuCommand(OnMenuCommandInvoked, changeHandler: null, UpdateMenuCommandStatus, MenuCommand));
        }

        private void OnMenuCommandInvoked(object sender, EventArgs e)
        {
            joinableTaskFactory.Run(
                "Copy function breakpoint name",
                "Copying the function breakpoint name to the clipboard...",
                async (progress, cancellationToken) =>
                {
                    var factory = await GetFunctionBreakpointNameFactoryAsync(cancellationToken);

                    if (factory != null)
                        Clipboard.SetText(factory.Value.ToString());
                });
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

            return FunctionBreakpointUtils.GetFunctionBreakpointNameFactory(
                await document.GetSyntaxRootAsync(cancellationToken),
                TextSpan.FromBounds(
                    activeViewSelection.Start.Position.Position,
                    activeViewSelection.End.Position.Position));
        }
    }
}