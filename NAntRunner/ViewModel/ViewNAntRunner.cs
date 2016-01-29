using NAntRunner.Controller;
using NAntRunner.ViewModel.Base;

namespace NAntRunner.ViewModel
{
    public class ViewNAntRunner : ViewModelBase
    {
        private bool _isStartEnabled;
        private bool _isStopEnabled;
        private bool _isEditEnabled;
        private bool _isSettingEnabled;
        private bool _isRefreshEnabled;

        
        public bool IsStartEnabled
        {
            get { return _isStartEnabled; }
            set { _isStartEnabled = value; OnPropertyChanged(); }
        }

        public bool IsStopEnabled
        {
            get { return _isStopEnabled; }
            set { _isStopEnabled = value; OnPropertyChanged(); }
        }

        public bool IsEditEnabled
        {
            get { return _isEditEnabled; }
            set { _isEditEnabled = value; OnPropertyChanged(); }
        }

        public bool IsSettingEnabled
        {
            get { return _isSettingEnabled; }
            set { _isSettingEnabled = value; OnPropertyChanged(); }
        }

        public bool IsRefreshEnabled
        {
            get { return _isRefreshEnabled; }
            set { _isRefreshEnabled = value; OnPropertyChanged(); }
        }

        public void Update(bool isNodeStartable)
        {
            var viewController = ViewController.Instance;
            var isNAntRunning = viewController.IsWorking;


            IsStartEnabled = isNodeStartable && !isNAntRunning;
            IsStopEnabled = isNodeStartable && isNAntRunning;
            IsEditEnabled = isNodeStartable && !isNAntRunning;
            IsSettingEnabled = !isNAntRunning;
            IsRefreshEnabled = !isNAntRunning && viewController.Filename != null;
        }
    }
}
