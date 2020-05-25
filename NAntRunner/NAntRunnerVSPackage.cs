//------------------------------------------------------------------------------
// <copyright file="NAntRunnerVSPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NAntRunner.Utils;

namespace NAntRunner
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    /// TODO: Update to inherit from AsynPackage and move GetService calls into an async pattern, per https://docs.microsoft.com/en-us/visualstudio/extensibility/how-to-use-asyncpackage-to-load-vspackages-in-the-background?view=vs-2019
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(NAntRunnerToolWindow))]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasSingleProject_string)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasMultipleProjects_string)]
    public sealed class NAntRunnerVsPackage : Package
    {
        /// <summary>
        /// NAntRunnerVSPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "3c9d12d6-6cde-404c-b2bc-b22bde7b9cfe";

        /// <summary>
        /// Initializes a new instance of the <see cref="NAntRunnerVsPackage"/> class.
        /// </summary>
        public NAntRunnerVsPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Members

        private ShellPropertyEventsHandler _shellPropertyEventsHandler;

        private SolutionEventsHandler _solutionEventsHandler;
        private uint _solutionEventsCookie = 0;

        #endregion

        #region Properties

        public DTE Dte { get; set; }

        public DTE2 Dte2 { get; set; }

        public IVsSolution VsSolution { get; set; }

        public IVsShell VsShell { get; set; }

        #endregion

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            NAntRunnerToolWindowCommand.Initialize(this);
            
            InitializeVsObjects();
        }

        protected override void Dispose(bool disposing)
        {
            if (_solutionEventsCookie != 0)
            {
                VsSolution.UnadviseSolutionEvents(_solutionEventsCookie);
                _solutionEventsCookie = 0;
            }
            _solutionEventsHandler = null;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Initialize all Visual Studio Objects
        /// </summary>
        public void InitializeVsObjects()
        {
            Dte = GetService(typeof(SDTE)) as DTE;
            Dte2 = GetService(typeof(SDTE)) as DTE2;
            VsSolution = GetService(typeof(SVsSolution)) as IVsSolution;

            if (Dte2 == null || Dte == null || VsSolution == null)
            {
                VsShell = GetService(typeof(SVsShell)) as IVsShell;
                _shellPropertyEventsHandler = new ShellPropertyEventsHandler(VsShell, InitializeVsObjects);
            }
            else
            {
                _shellPropertyEventsHandler = null;

                ToolWindowPane window = FindToolWindow(typeof(NAntRunnerToolWindow), 0, true);
                _solutionEventsHandler = new SolutionEventsHandler(window.Content as NAntRunnerToolWindowControl);
                VsSolution.AdviseSolutionEvents(_solutionEventsHandler, out _solutionEventsCookie);
            }

        }
        
        #endregion

        #region Solution Event Handling 

        internal void HandleSolutionEvent(string eventName)
        {
            Debug.WriteLine(eventName);
        }

        #endregion
    }
}
