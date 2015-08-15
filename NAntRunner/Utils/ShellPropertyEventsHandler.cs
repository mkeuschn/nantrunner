using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace NAntRunner.Utils
{
    public class ShellPropertyEventsHandler : IVsShellPropertyEvents
    {
        private readonly IVsShell _shellService;
        private uint _cookie;
        private readonly Action _callback;

        public ShellPropertyEventsHandler(IVsShell shellService, Action callback)
        {
            _shellService = shellService;
            _callback = callback;

            // Set an event handler to detect when the IDE is fully initialized
            var hr = _shellService.AdviseShellPropertyChanges(this, out _cookie);

            ErrorHandler.ThrowOnFailure(hr);
        }

        public int OnShellPropertyChange(int propid, object var)
        {
            if (propid == (int)__VSSPROPID.VSSPROPID_Zombie)
            {
                var isZombie = (bool)var;

                if (!isZombie)
                {
                    // Release the event handler to detect when the IDE is fully initialized
                    var hr = _shellService.UnadviseShellPropertyChanges(_cookie);

                    ErrorHandler.ThrowOnFailure(hr);

                    _cookie = 0;

                    _callback();
                }
            }
            return VSConstants.S_OK;
        }
    }
}
