using System.Drawing;
using System.Windows.Controls;
using System.Windows.Forms;
using NAntRunner.Common;
using NAntRunner.XML;
using TreeView = System.Windows.Forms.TreeView;

namespace NAntRunner.Utils
{
    /// <summary>
    /// Utilities methods for the NAnt Addin tree view.
    /// </summary>
    public static class TreeViewUtils
    {
        /// <summary>
        /// Open a context menu at the right position on a tree view.
        /// </summary>
        /// <param name="treeView">The tree view.</param>
        /// <param name="contextMenu">The context menu to display.</param>
        /// <param name="evt">The mouse event.</param>
        /// <param name="mousePoint">The position of the mouse.</param>
        public static void ShowContext(TreeView treeView, ContextMenuStrip contextMenu, MouseEventArgs evt, Point mousePoint)
        {
            // Retrieve the node under the mouse
            treeView.SelectedNode = treeView.GetNodeAt(evt.X, evt.Y);

            if (treeView.SelectedNode != null)
            {
                // Show context menu at correct position
                contextMenu.Show(mousePoint);
            }
        }

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
    }
}
