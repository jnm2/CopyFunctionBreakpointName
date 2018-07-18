﻿using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Threading;
using Task = System.Threading.Tasks.Task;

namespace CopyFunctionBreakpointName
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuidString)]
    [ProvideAutoLoad(UIContextGuid, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideUIContextRule(UIContextGuid,
        name: "C# editor window",
        expression: "CSharpEditorWindow",
        termNames: new[] { "CSharpEditorWindow" },
        termValues: new[] { "ActiveEditorContentType:cs" })]
    public sealed class CopyFunctionBreakpointNamePackage : AsyncPackage
    {
        public const string PackageGuidString = "b78d32a2-8af1-4838-858e-6f865bd7b291";
        public const string UIContextGuid = "91909bfb-2bff-4d68-ae11-e0f8478b4c46";

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            var menuService = (IMenuCommandService)await GetServiceAsync(typeof(IMenuCommandService));

            var lazyServices = new AsyncLazy<(IVsTextManager, IVsEditorAdaptersFactoryService)>(async () =>
            {
                var (componentModel, textManager) = ((IComponentModel, IVsTextManager))
                    await (
                        GetServiceAsync(typeof(SComponentModel)),
                        GetServiceAsync(typeof(SVsTextManager)));

                return (
                    textManager,
                    componentModel.GetService<IVsEditorAdaptersFactoryService>());
            }, JoinableTaskFactory);

            await JoinableTaskFactory.SwitchToMainThreadAsync();

            menuService.AddCommand(CopyFunctionBreakpointNameCommand.Create(this, lazyServices));
        }
    }
}
