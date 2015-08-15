using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using NAntRunner.Controller;

namespace NAntRunner.Utils
{
    public class SolutionEventsHandler : IVsSolutionEvents
    {
        #region Members

        private readonly NAntRunnerToolWindowControl _control;
        private readonly ViewController _viewController;

        #endregion

        #region Constructor

        public SolutionEventsHandler(NAntRunnerToolWindowControl control)
        {
            _control = control;
            _viewController = ViewController.Instance;
        }

        #endregion

        #region IVsSolutionEvents

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            // Your Implementation here!
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            // Your Implementation here!
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            // Your Implementation here!
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            // Your Implementation here!
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            // Your Implementation here!
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            // Your Implementation here!
            return VSConstants.S_OK;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            // If autoload specified, just load default build file
            if (Settings.Default.NANT_AUTOLOAD)
            {
                _viewController.Filename = _viewController.DefaultBuildFile;
                _control.OnReload(this, null);
            }
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            // Your Implementation here!
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            // Your Implementation here!
            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            // If NAnt process is running, stop it
            if (_viewController.IsWorking)
            {
                _viewController.StopTarget();
                _control.RefreshView();
            }

            // If autoload specified, just clean current build file
            if (Settings.Default.NANT_AUTOLOAD)
            {
                _viewController.Filename = null;
                _control.NAntTreeView.Items.Clear();
            }
            return VSConstants.S_OK;
        }

        #endregion
    }
}
