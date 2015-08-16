// The MIT License(MIT)
// 
// Copyright(c) <year> <copyright holders>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using EnvDTE80;
using NAntRunner.Common;
using NAntRunner.Utils;
using NAntRunner.XML;

namespace NAntRunner.Controller
{
    public class NAntProcess
    {
        #region Private attributes

        private ViewController _viewController;
        private BackgroundWorker _backgroundWorker;
        private Process _nAntProcess;

        #endregion

        #region Public events/delegate
        
        /// <summary>
        /// The Taget complete handler
        /// </summary>
        public event TargetCompletedHandler TargetCompleted;
        /// <summary>
        /// The target complete event handler
        /// </summary>
        /// <param name="sender">the object name</param>
        /// <param name="e">The arguments</param>
        public delegate void TargetCompletedHandler(object sender, EventArgs e);

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewController"></param>
        public NAntProcess(ViewController viewController)
        {
            _viewController = viewController;
        }
        
        public string Filename { get; set; }

        public DTE2 ApplicationObject
        {
            get
            {
                var package = NAntRunnerToolWindowCommand.Instance.ServiceProvider as NAntRunnerVsPackage;
                return package?.Dte2;
            }
        }

        public XmlNode TargetNode { get; set; }
        
        public bool IsWorking => _backgroundWorker != null && _backgroundWorker.IsBusy;
        
        public void Start()
        {
            // Initialize backround worker
            _backgroundWorker = new BackgroundWorker() 
            {
                WorkerReportsProgress = true, 
                WorkerSupportsCancellation = true 
            };
            _backgroundWorker.DoWork += OnStart;
            _backgroundWorker.ProgressChanged += OnProgress;
            _backgroundWorker.RunWorkerCompleted += OnComplete;

            // Run task
            _backgroundWorker.RunWorkerAsync();

            // Autoclear console, if required
            if (Settings.Default.NANT_CLEAR_OUTPUT)
            {
                VisualStudioUtils.GetConsole(ApplicationObject, AppConstants.NAntRunner).Clear();
            }

            // Trace start build
            WriteConsole(string.Format("{0}[{2}]: Target '{1}' started...{0}{0}", 
                         Environment.NewLine, 
                         TargetNode["name"],
                         AppConstants.NAntRunner));
        }
        
        public void Stop()
        {
            _backgroundWorker.CancelAsync();
        }
        
        public void WriteConsole(string message)
        {
            VisualStudioUtils.GetConsole(ApplicationObject, AppConstants.NAntRunner).OutputString(message);
        }
        
        private void OnStart(object sender, DoWorkEventArgs e)
        {
            string nantCommand = Settings.Default.NANT_COMMAND;
            string nantArguments = string.Format(Settings.Default.NANT_PARAMS, Filename, TargetNode["name"]);

            try
            {
                string nantOutput;

                // Create and initialize the process
                _nAntProcess = new Process
                {
                    StartInfo =
                    {
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        FileName = nantCommand,
                        WorkingDirectory = Path.GetDirectoryName(Filename),
                        Arguments = nantArguments
                    }
                };


                // Start process
                _nAntProcess.Start();

                // Read standard output and write string in console
                while ((nantOutput = _nAntProcess.StandardOutput.ReadLine()) != null)
                {
                    if (_backgroundWorker.CancellationPending)
                    {
                        if (!_nAntProcess.HasExited)
                        {
                            _nAntProcess.Kill();
                            _nAntProcess.WaitForExit();
                            e.Cancel = true;
                        }
                    }
                    else
                    {
                        nantOutput += Environment.NewLine;
                        _backgroundWorker.ReportProgress(0, nantOutput);
                    }
                }
            }
            catch (Exception e1)
            {
                // Trace exception on console
                WriteConsole("[" + AppConstants.NAntRunner + "]: Unexpected error occured while executing command: "
                    + Environment.NewLine
                    + "\t" + nantCommand + nantArguments
                    + Environment.NewLine
                    + Environment.NewLine
                    + "An exception has been raised with the following stacktrace:"
                    + Environment.NewLine
                    + "   " + e1.Message
                    + Environment.NewLine
                    + e1.StackTrace
                    + Environment.NewLine
                    + Environment.NewLine
                    + "Please check that NAnt command path is properly configured within NAntAddin options."
                    + Environment.NewLine
                );
            }
        }
        
        /// <summary>
        /// Reports background vorker progress.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnProgress(object sender, ProgressChangedEventArgs e)
        {
            string progressString = e.UserState as string;

            if (Settings.Default.NANT_VERBOSE)
                WriteConsole(progressString);
        }

        private void OnComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            _backgroundWorker = null;
            _nAntProcess      = null;

            if (e.Cancelled)
                WriteConsole(string.Format("{0}[{2}]: Target '{1}' aborted !{0}", 
                             Environment.NewLine, 
                             TargetNode["name"],
                             AppConstants.NAntRunner));
            else
                WriteConsole(string.Format("{0}[{2}]: Target '{1}' completed.{0}", 
                             Environment.NewLine, 
                             TargetNode["name"],
                             AppConstants.NAntRunner));

            // Notify listeners that the process has exited
            TargetCompleted?.Invoke(this, new EventArgs());
        }
    }
}