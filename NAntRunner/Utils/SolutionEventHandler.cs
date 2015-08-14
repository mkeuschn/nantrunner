using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace NAntRunner.Utils
{
    public class SolutionEventHandler : IVsSolutionEvents
    {
        #region Members

        private NAntRunnerVSPackage _package;

        #endregion

        #region Construktor

        public SolutionEventHandler(NAntRunnerVSPackage package)
        {
            _package = package;
        }

        #endregion

        #region IVsSolutionEvents

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            _package.HandleSolutionEvent("OnAfterOpenProject");
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            _package.HandleSolutionEvent("OnQueryCloseProject");
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            _package.HandleSolutionEvent("OnBeforeCloseProject");
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            _package.HandleSolutionEvent("OnAfterLoadProject");
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            _package.HandleSolutionEvent("OnQueryUnloadProject");
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            _package.HandleSolutionEvent("OnBeforeUnloadProject");
            return VSConstants.S_OK;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            _package.HandleSolutionEvent("OnAfterOpenSolution");
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            _package.HandleSolutionEvent("OnQueryCloseSolution");
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            _package.HandleSolutionEvent("OnBeforeCloseSolution");
            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            _package.HandleSolutionEvent("OnAfterCloseSolution");
            return VSConstants.S_OK;
        }

        #endregion
    }
}
