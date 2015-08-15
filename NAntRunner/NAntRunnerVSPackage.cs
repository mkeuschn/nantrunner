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
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(NAntRunnerToolWindow))]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasSingleProject_string)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasMultipleProjects_string)]
    public sealed class NAntRunnerVsPackage : Package, IVsShellPropertyEvents
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

        private IVsSolution _solution;
        private SolutionEventHandler _solutionEventHandler;
        private uint _solutionEventsCookie = 0;

        private uint _cookie;

        #endregion

        #region Properties

        public DTE Dte { get; set; }

        public DTE2 Dte2 { get; set; }

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

            IVsShell shellService = GetService(typeof(SVsShell)) as IVsShell;
            if (shellService != null)
                ErrorHandler.ThrowOnFailure(
                  shellService.AdviseShellPropertyChanges(this, out _cookie));

            _solution = base.GetService(typeof(SVsSolution)) as IVsSolution;

            // Solution Event Handling
            if (_solution != null)
            {
                _solutionEventHandler = new SolutionEventHandler(this);
                _solution.AdviseSolutionEvents(_solutionEventHandler, out _solutionEventsCookie);
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (_solutionEventsCookie != 0)
            {
                _solution.UnadviseSolutionEvents(_solutionEventsCookie);
                _solutionEventsCookie = 0;
            }
            _solutionEventHandler = null;
            base.Dispose(disposing);
        }

        public int OnShellPropertyChange(int propid, object var)
        {
            // when zombie state changes to false, finish package initialization
            if ((int)__VSSPROPID.VSSPROPID_Zombie == propid)
            {
                if ((bool)var == false)
                {
                    Dte = GetService(typeof(SDTE)) as DTE;
                    Dte2 = GetService(typeof(SDTE)) as DTE2;
                    IVsShell shellService = GetService(typeof(SVsShell)) as IVsShell;

                    if (shellService != null)
                        ErrorHandler.ThrowOnFailure(
                          shellService.UnadviseShellPropertyChanges(this._cookie));
                    this._cookie = 0;
                }
            }
            return VSConstants.S_OK;
        }

        #endregion

        #region Solution Event Handling 

        internal void HandleSolutionEvent(string eventName)
        {
            Debug.WriteLine(eventName);
        }

        // TODO delegate event to view class
        public void OnAfterOpenSolution()
        {
        }

        // TODO delegate event ot view class
        public void OnAfterCloseSolution()
        {
        }

        #endregion
    }
}
