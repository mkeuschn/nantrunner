using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using NAntRunner.Common;
using NAntRunner.XML;

namespace NAntRunner.Controller
{
    /// <summary>
    /// NAnt Addin tree view factory.
    /// </summary>
    public static class TreeViewFactory
    {
        
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
            TreeViewItem rootNode = new TreeViewItem
            {
                Header = title,
                Tag = nantTree.Root
            };

            treeView.Items.Add(rootNode);

            // Build properties
            if (nantTree.Properties != null)
            {
                TreeViewItem tvi = CreateTreeNode(nantTree.Properties, "Properties");
                rootNode.Items.Add(tvi);
            }

            // Build targets in two groups
            if (Settings.Default.NANT_SPLIT_TARGETS)
            {
                // Build public targets
                if (nantTree.PublicTargets != null)
                {
                    TreeViewItem publicNodes = CreateTreeNode(nantTree.PublicTargets, "Public targets");
                    rootNode.Items.Add(publicNodes);
                    publicNodes.IsExpanded = true;
                }

                // Build private targets
                if (nantTree.PrivateTargets != null)
                {
                    TreeViewItem privateNodes = CreateTreeNode(nantTree.PrivateTargets, "Private targets");
                    rootNode.Items.Add(privateNodes);
                }
            }
            // Build target all together
            else
            {
                // Build all targets
                if (nantTree.AllTargets != null)
                {
                    TreeViewItem allNodes = CreateTreeNode(nantTree.AllTargets, "Targets");
                    rootNode.Items.Add(allNodes);
                    allNodes.IsExpanded = true;
                }
            }

            // Build icon
            //TreeViewFactory.CreateIcons(treeView);

            // Finally expand the root node
            rootNode.IsExpanded = true;
        }

        
        /// <summary>
        /// Build a branch of NAnt Addin tree view from a list of NAntNode.
        /// </summary>
        /// <param name="nodes">The list of NAntNode to add.</param>
        /// <param name="title">The title of the branch.</param>
        /// <returns>The root node of the branch.</returns>
        public static TreeViewItem CreateTreeNode(IList<XmlNode> nodes, string title)
        {
            // Parent node of the branch
            TreeViewItem rootNode = new TreeViewItem {Header = title};

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
            
            // For target/property we take the name of the target/property
            if (nantNode.Name == AppConstants.NANT_XML_TARGET 
                || nantNode.Name == AppConstants.NANT_XML_PROPERTY)
                title = nantNode["name"];

            // Initialize the root node
            TreeViewItem root = new TreeViewItem
            {
                Header = title,
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
    }
}
