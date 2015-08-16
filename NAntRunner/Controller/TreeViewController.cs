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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using NAntRunner.Common;
using NAntRunner.XML;

namespace NAntRunner.Controller
{
    /// <summary>
    /// NAnt Addin tree view factory.
    /// </summary>
    public static class TreeViewController
    {
        /// <summary>
        /// Return the NAntNode attached to a TreeNode.
        /// </summary>
        /// <param name="tvi">The TreeViewItem.</param>
        /// <returns>The NAntNode or null if any NAntNode is attached.</returns>
        public static XmlNode GetNAntNode(TreeViewItem tvi)
        {
            return tvi?.Tag as XmlNode;
        }

        /// <summary>
        /// Determines whether a TreeNode has a NAntNode attached to it.
        /// </summary>
        /// <param name="tvi">The TreeViewItem to check.</param>
        /// <returns>True if a NAntNode is attached and false if not.</returns>
        public static bool IsNAntNode(TreeViewItem tvi)
        {
            return GetNAntNode(tvi) != null;
        }

        /// <summary>
        /// Determines whether a TreeNode has a NAntNode attached to it and if
        /// this node is NAnt target.
        /// </summary>
        /// <param name="tvi">The TreeViewItem to check.</param>
        /// <returns>
        /// True if a NAntNode is attached and the NAntNode is a NAnt target and
        /// false if not.
        /// </returns>
        public static bool IsNAntTarget(TreeViewItem tvi)
        {
            if (tvi == null)
                return false;

            XmlNode nantNode = GetNAntNode(tvi);

            if (nantNode != null)
                return nantNode.Name == AppConstants.NANT_XML_TARGET;

            return false;
        }

        /// <summary>
        /// True if a NAntNode is attached and the NAntNode is a NAnt property and
        /// false if not.
        /// </summary>
        /// <param name="tvi"></param>
        /// <returns></returns>
        public static bool IsNAntProperty(TreeViewItem tvi)
        {
            if (tvi == null)
                return false;

            XmlNode nantNode = GetNAntNode(tvi);

            if (nantNode != null)
                return nantNode.Name == AppConstants.NANT_XML_PROPERTY;

            return false;
        }

        /// <summary>
        /// Build the NAnt Addin tree view from an NAntTree.
        /// </summary>
        /// <param name="treeView">The NAnt Addin tree.</param>
        /// <param name="nantTree">The NAntTree.</param>
        /// <param name="title">The title to display.</param>
        public static void CreateTree(TreeView treeView, XmlTree nantTree, string title)
        {
            if (nantTree == null)
                return;

            // Initialize the treeview and its root node
            treeView.Items.Clear();

            // Create root node
            TreeViewItem rootNode = CreateImageTreeViewItem(title, AppConstants.IconNAnt);
            rootNode.Tag = nantTree.Root; 
            
            treeView.Items.Add(rootNode);

            // Build properties
            if (nantTree.Properties != null)
            {
                TreeViewItem tvi = CreateTreeNode(nantTree.Properties, "Properties", AppConstants.IconProperties);
                rootNode.Items.Add(tvi);
            }

            // Build targets in two groups
            if (Settings.Default.NANT_SPLIT_TARGETS)
            {
                // Build public targets
                if (nantTree.PublicTargets != null)
                {
                    TreeViewItem publicNodes = CreateTreeNode(nantTree.PublicTargets, "Public targets", AppConstants.IconGear);
                    rootNode.Items.Add(publicNodes);
                    publicNodes.IsExpanded = true;
                }

                // Build private targets
                if (nantTree.PrivateTargets != null)
                {
                    TreeViewItem privateNodes = CreateTreeNode(nantTree.PrivateTargets, "Private targets", AppConstants.IconGear);
                    rootNode.Items.Add(privateNodes);
                }
            }
            // Build target all together
            else
            {
                // Build all targets
                if (nantTree.AllTargets != null)
                {
                    TreeViewItem allNodes = CreateTreeNode(nantTree.AllTargets, "Targets", AppConstants.IconGear);
                    rootNode.Items.Add(allNodes);
                    allNodes.IsExpanded = true;
                }
            }

            // Build icon
            //TreeViewController.CreateIcons(treeView);

            // Finally expand the root node
            rootNode.IsExpanded = true;
        }


        /// <summary>
        /// Build a branch of NAnt Addin tree view from a list of NAntNode.
        /// </summary>
        /// <param name="nodes">The list of NAntNode to add.</param>
        /// <param name="title">The title of the branch.</param>
        /// <param name="iconUri"></param>
        /// <returns>The root node of the branch.</returns>
        public static TreeViewItem CreateTreeNode(IList<XmlNode> nodes, string title, string iconUri)
        {
            // Parent node of the branch
            TreeViewItem rootNode = CreateImageTreeViewItem(title, iconUri);
            
            // Children
            List<TreeViewItem> nodeChildren = nodes.Select(CreateTreeNode).ToList();
            
            // Add all children
            foreach (var treeViewItem in nodeChildren)
            {
                rootNode.Items.Add(treeViewItem);
            }

            return rootNode;
        }

        
        /// <summary>
        /// Build recursivly a node of a branch from an NAntNode.
        /// </summary>
        /// <param name="nantNode">The NAntNode to build.</param>
        /// <returns>The root of the branch.</returns>
        public static TreeViewItem CreateTreeNode(XmlNode nantNode)
        {
            // Set the title of node with nantNode name
            string title = nantNode.Name;

            StackPanel header = new StackPanel {Orientation = Orientation.Horizontal};
            TextBlock label = new TextBlock();
            Image image = new Image();
            header.Children.Add(image);
            header.Children.Add(label);

            // For target/property we take the name of the target/property
            if (nantNode.Name == AppConstants.NANT_XML_TARGET)
            {
                title = nantNode["name"];
                image.Source = new BitmapImage(new Uri(AppConstants.IconGear));
            }

            if (nantNode.Name == AppConstants.NANT_XML_PROPERTY)
            {
                title = nantNode["name"];
                image.Source = new BitmapImage(new Uri(AppConstants.IconProperties));
            }

            label.Text = title;

            // Initialize the root node
            TreeViewItem root = new TreeViewItem
            {
                Header = header,
                Tag = nantNode
            };

            // Retrieve the children of node
            IList<XmlNode> children = nantNode.Children;

            if (children != null)
            {
                // Build each child recurvively
                foreach (XmlNode child in children)
                {
                    if (nantNode.Name != AppConstants.NANT_XML_TARGET)
                    {
                        // Build a node and add to parent
                        TreeViewItem node = CreateTreeNode(child);
                        root.Items.Add(node);
                    }
                }
            }

            return root;
        }

        /// <summary>
        /// Create TreeViewItem with image.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public static TreeViewItem CreateImageTreeViewItem(string title, string imagePath)
        {
            var tvi = new TreeViewItem();
            var header = new StackPanel {Orientation = Orientation.Horizontal};
            var text = new TextBlock {Text = title};
            var image = new Image {Source = new BitmapImage(new Uri(imagePath))};
            header.Children.Add(image);
            header.Children.Add(text);
            tvi.Header = header;

            return tvi;
        }
    }
}
