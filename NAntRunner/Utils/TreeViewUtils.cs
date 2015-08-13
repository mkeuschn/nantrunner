using System.Windows.Controls;
using NAntRunner.Common;
using NAntRunner.XML;

namespace NAntRunner.Utils
{
    /// <summary>
    /// Utilities methods for the NAnt Addin tree view.
    /// </summary>
    public static class TreeViewUtils
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
    }
}
