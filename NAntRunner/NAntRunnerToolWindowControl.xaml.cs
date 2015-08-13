//------------------------------------------------------------------------------
// <copyright file="NAntRunnerToolWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace NAntRunner
{
    using Microsoft.Win32;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for NAntRunnerToolWindowControl.
    /// </summary>
    public partial class NAntRunnerToolWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NAntRunnerToolWindowControl"/> class.
        /// </summary>
        public NAntRunnerToolWindowControl()
        {
            this.InitializeComponent();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Set filter for file extension and default file extension 
            openFileDialog.DefaultExt = ".build";
            openFileDialog.Filter = "Build Files (*.build)|*.build|Text Files (*.txt)|*.txt";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = openFileDialog.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = openFileDialog.FileName;

                // parse the NAnt file
                var properties = new TreeViewItem();
                properties.Header = "Properties";

                var target = new TreeViewItem();
                target.Header = "Target";

                NAntTreeView.Items.Add(properties);
                NAntTreeView.Items.Add(target);
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {

        }

        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
                "NAntBuilderToolWindow");
        }
    }
}