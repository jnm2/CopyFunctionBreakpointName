using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Threading;
using Task = System.Threading.Tasks.Task;

namespace CopyFunctionBreakpointName
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuidString)]
    [ProvideAutoLoad(UIContextGuids.SolutionExists), ProvideAutoLoad(UIContextGuids.NoSolution)]
    public sealed class CopyFunctionBreakpointNamePackage : AsyncPackage
    {
        public const string PackageGuidString = "b78d32a2-8af1-4838-858e-6f865bd7b291";

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            if (JoinableTaskFactory.Context.IsOnMainThread) await TaskScheduler.Default;
            
            var menuService = (IMenuCommandService)await GetServiceAsync(typeof(IMenuCommandService)).ResumeOnMainThread(JoinableTaskFactory);
           
            var lazyServices = new AsyncLazy<(IVsTextManager, IVsEditorAdaptersFactoryService)>(async () =>
            {
                var (componentModel, textManager) = ((IComponentModel, IVsTextManager))
                    await (
                        GetServiceAsync(typeof(SComponentModel)),
                        GetServiceAsync(typeof(SVsTextManager))
                    ).ConfigureAwait(false);

                return (
                    textManager,
                    componentModel.GetService<IVsEditorAdaptersFactoryService>());
            }, JoinableTaskFactory);

            menuService.AddCommand(CopyFunctionBreakpointNameCommand.Create(this, lazyServices));
        }
    }
}
