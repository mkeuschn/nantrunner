//------------------------------------------------------------------------------
// <copyright file="NAntRunnerToolWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
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
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using NAntRunner.Common;
using NAntRunner.Controller;
using NAntRunner.Views;
using NAntRunner.XML;

namespace NAntRunner
{
    /// <summary>
    /// Interaction logic for NAntRunnerToolWindowControl.
    /// </summary>
    public partial class NAntRunnerToolWindowControl : UserControl
    {
        #region Members

        private readonly ViewController _viewController;

        private ContextMenu _contextMenu = new ContextMenu { Name = "contextMenu" };

        private MenuItem _start = new MenuItem
        {
            Name = "miStart",
            Header = "Start",
            Icon = new Image
            {
                Source = new BitmapImage(new Uri(AppConstants.IconStart))
            }
        };
        private MenuItem _stop = new MenuItem
        {
            Name = "miStop",
            Header = "Stop",
            Icon = new Image
            {
                Source = new BitmapImage(new Uri(AppConstants.IconStop))
            }
        };
        private MenuItem _edit = new MenuItem
        {
            Name = "miEdit",
            Header = "Edit"
        };
        private MenuItem _expandAll = new MenuItem
        {
            Name = "miExpandAll",
            Header = "ExpandAll"
        };
        private MenuItem _collapseAll = new MenuItem
        {
            Name = "miCollapseAll",
            Header = "Collapse All"
        };
        private MenuItem _settings = new MenuItem
        {
            Name = "miSettings",
            Header = "Settings",
            Icon = new Image
            {
                Source = new BitmapImage(new Uri(AppConstants.IconGear))
            }
        };

        #endregion

        #region Properties

        public IVsSolution VsSolution
        {
            get
            {
                var package = NAntRunnerToolWindowCommand.Instance.ServiceProvider as NAntRunnerVsPackage;
                return package?.VsSolution;
            }
        }

        #endregion

        #region Default Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NAntRunnerToolWindowControl"/> class.
        /// </summary>
        public NAntRunnerToolWindowControl()
        {
            this.InitializeComponent();
            _viewController = ViewController.Instance;
            _viewController.NAntProcess.TargetCompleted += OnTargetCompleted;
            InitContextMenu();
        }

        #endregion

        #region Toolbar Button Handler

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                DefaultExt = ".build",
                Filter = _viewController.FileFilter
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
            if (!string.IsNullOrEmpty(_viewController.Filename))
            {
                OnReload(sender, e);
            }
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

        #region Public Methods

        private void OnStartTarget(object sender, EventArgs e)
        {
            _viewController.StartTarget();
            RefreshView();
        }
        
        #endregion

        #region TreeView Handler

        private void TreeViewItemOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // TODO: Get Node From Sender ????
            // Get the selected node
            XmlNode selectedNode = TreeViewController.GetNAntNode(NAntTreeView.SelectedItem as TreeViewItem);
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
            // TODO: Get Node From Sender ????
            XmlNode selectedNode = TreeViewController.GetNAntNode(NAntTreeView.SelectedItem as TreeViewItem);
            _viewController.CurrentNode = selectedNode;

            // TODO Open Context Menu
            NAntTreeView.ContextMenu = _contextMenu;

        }

        private void TreeViewItemOnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // TODO: Get Node From Sender ????
            XmlNode selectedNode = TreeViewController.GetNAntNode(NAntTreeView.SelectedItem as TreeViewItem);
            _viewController.CurrentNode = selectedNode;
            
            if (TreeViewController.IsNAntTarget(NAntTreeView.SelectedItem as TreeViewItem))
            { 
                Start_Click(sender, e);
            }
            else
            {
                // TODO Open Nant File at property position
            }
        }

        #endregion
        
        #region Private Methods

        private void InitContextMenu()
        {
            _contextMenu.Items.Clear();
            _start.Click += Start_Click;
            _contextMenu.Items.Add(_start);
            _stop.Click += Stop_Click;
            _contextMenu.Items.Add(_stop);
            _edit.Click += OnEditTarget;
            _contextMenu.Items.Add(_edit);
            _expandAll.Click += OnExpandAll;
            _contextMenu.Items.Add(_expandAll);
            _collapseAll.Click += OnCollapseAll;
            _contextMenu.Items.Add(_collapseAll);
            _settings.Click += Settings_Click;
            _contextMenu.Items.Add(_settings);
        }

        /// <summary>
        /// 
        /// </summary>
        public void RefreshView()
        {
            bool isNAntRunning = _viewController.IsWorking;
            bool isNodeStartable = TreeViewController.IsNAntTarget(NAntTreeView.SelectedItem as TreeViewItem);

            // Refresh buttons
            btnStart.IsEnabled = isNodeStartable & !isNAntRunning;
            btnRefresh.IsEnabled = !isNAntRunning && _viewController.Filename != null;
            btnHelp.IsEnabled = true;
            btnSettings.IsEnabled = !isNAntRunning;
            btnStop.IsEnabled = isNAntRunning;

            // Refresh menus
            _start.IsEnabled = isNodeStartable && !isNAntRunning;
            _stop.IsEnabled = isNodeStartable && isNAntRunning;
            _edit.IsEnabled = isNodeStartable && !isNAntRunning;
            _settings.IsEnabled = !isNAntRunning;
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
                    TreeViewController.CreateTree(NAntTreeView, _viewController.NAntTree, _viewController.Filename);
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
            if (tvi != null) tvi.IsExpanded = false;
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
    }
}