//------------------------------------------------------------------------------
// <copyright file="NAntRunnerToolWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using NAntRunner.Controller;
using NAntRunner.Utils;
using NAntRunner.Views;
using NAntRunner.XML;

namespace NAntRunner
{
    /// <summary>
    /// Interaction logic for NAntRunnerToolWindowControl.
    /// </summary>
    public partial class NAntRunnerToolWindowControl : UserControl, IVsSolutionEvents3
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NAntRunnerToolWindowControl"/> class.
        /// </summary>
        public NAntRunnerToolWindowControl()
        {
            this.InitializeComponent();
            _viewController = new ViewController();
            _viewController.NAntProcess.TargetCompleted += OnTargetCompleted;
        }

        #region Toolbar Button Handler

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                DefaultExt = ".build",
                Filter = "Build Files (*.build)|*.build|Text Files (*.txt)|*.txt"
            };
            

            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = openFileDialog.ShowDialog();
            
            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Update controller and refresh view
                _viewController.Filename = openFileDialog.FileName;
                OnReload(sender, e);
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            // TODO Reload Content From the same file
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            _viewController.StartTarget();
            RefreshView();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            _viewController.StopTarget();
            RefreshView();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            // TODO Open Settings Dialog
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }

        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(CultureInfo.CurrentUICulture, "Invoked '{0}'", this),
                "NAnt Runner About Dialog");
        }

        #endregion

        #region TreeView Handler

        private void TreeViewItemOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Get the selected node
            XmlNode selectedNode = TreeViewUtils.GetNAntNode(sender as TreeViewItem);
            _viewController.CurrentNode = selectedNode;

            /*
            // Check if node can be display in property pane
            if (selectedNode != null)
                m_ItemProperties.SelectedObject = _viewController.CurrentNode.Descriptor;
            else
                m_ItemProperties.SelectedObject = null;
            */


            // Refresh button
            RefreshView();
        }

        private void TreeViewItemOnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            XmlNode selectedNode = TreeViewUtils.GetNAntNode(sender as TreeViewItem);
            _viewController.CurrentNode = selectedNode;

            // TODO Open Context Menu
        }

        private void TreeViewItemOnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            XmlNode selectedNode = TreeViewUtils.GetNAntNode(sender as TreeViewItem);
            _viewController.CurrentNode = selectedNode;

            // TODO Execute Nant Target or Open Nant File and Show Property
            if (TreeViewUtils.IsNAntTarget(NAntTreeView.SelectedItem as TreeViewItem))
            { 
                Start_Click(sender, e);
            }
            else
            {
                // TODO Open Nant File at property position
            }
        }

        #endregion

        #region Members

        private readonly ViewController _viewController;

        #endregion

        #region Properties

        #endregion

        #region Private Methods

        private void RefreshView()
        {
            bool isNAntRunning = _viewController.IsWorking;
            bool isNodeStartable = TreeViewUtils.IsNAntTarget(NAntTreeView.SelectedItem as TreeViewItem);

            // Refresh buttons
            Start.IsEnabled = isNodeStartable & !isNAntRunning;
            Refresh.IsEnabled = !isNAntRunning && _viewController.Filename != null;
            Help.IsEnabled = true;
            Settings.IsEnabled = !isNAntRunning;
            Stop.IsEnabled = isNAntRunning;

            // Refresh menus
            //m_MenuStart.Enabled = isNodeStartable && !isNAntRunning;
            //m_MenuStop.Enabled = isNodeStartable && isNAntRunning;
            //m_MenuEdit.Enabled = isNodeStartable && !isNAntRunning;
            //m_MenuOption.Enabled = !isNAntRunning;
        }

        #endregion

        #region Custom Events
       
        /// <summary>
        /// The refresh button has been clicked or a new file has been loaded.
        /// The current TreeView will be reloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        public void OnReload(object sender, EventArgs e)
        {

            // Load file if any
            if (_viewController.Filename == null)
            {
                NAntTreeView.Items.Clear();
                
                // TODO
                //m_ItemProperties.SelectedObject = null;
            }
            else
            {
                try
                {
                    // Load the file
                    _viewController.LoadFile(_viewController.Filename);

                    // Create a corresponding visual tree
                    TreeViewFactory.CreateTree(NAntTreeView, _viewController.NAntTree, _viewController.Filename);
                }
                catch (Exception e1)
                {
                    var error = "Error while loading file '"
                                + _viewController.Filename + "'."
                                + "\n"
                                + e1.Message;
                    MessageBox.Show(error, "NAnt Runner Error",MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            
            RefreshView();
        }
        
        /// <summary>
        /// Open the script file at the selected target location.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void OnEditTarget(object sender, EventArgs e)
        {
            _viewController.SelectNodeLine();
        }
        
        /// <summary>
        /// The Collapse All contextual menu item has been selected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnCollapseAll(object sender, EventArgs e)
        {
            var tvi = NAntTreeView.SelectedItem as TreeViewItem;
            if (tvi != null) tvi.IsExpanded = true;
        }
        
        /// <summary>
        /// The Expand All context menu item has been selected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnExpandAll(object sender, EventArgs e)
        {
            var tvi = NAntTreeView.SelectedItem as TreeViewItem;
            if (tvi != null) tvi.IsExpanded = true;
        }
        
        /// <summary>
        /// The current NAntProcess has been cancelled and stopped.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        public void OnTargetCompleted(object sender, EventArgs e)
        {
            RefreshView();
            // TODO Set Focus to ToolWindow
        }

        #endregion

        #region IVsSolutionEvents3 Events

        int IVsSolutionEvents.OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents3.OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents3.OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents3.OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents3.OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents3.OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents3.OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents3.OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents3.OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents3.OnAfterCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents3.OnAfterMergeSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeOpeningChildren(IVsHierarchy pHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterOpeningChildren(IVsHierarchy pHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeClosingChildren(IVsHierarchy pHierarchy)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterClosingChildren(IVsHierarchy pHierarchy)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents3.OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents2.OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents2.OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents2.OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents2.OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents2.OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents2.OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            // TODO
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents2.OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents2.OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents2.OnAfterCloseSolution(object pUnkReserved)
        {
            // TODO
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents2.OnAfterMergeSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents2.OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        #endregion
    }
}