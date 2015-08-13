using System.IO;
using EnvDTE80;
using NAntRunner.Utils;
using NAntRunner.XML;

namespace NAntRunner.Controller
{
    /// <summary>
    /// Controller of the NAnt Addin.
    /// </summary>
    public class ViewController
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ViewController()
        {
            NAntProcess = new NAntProcess(this);
        }

        /// <summary>
        /// Get or set current DTE2 attribute (extensibility instance).
        /// </summary>
        public DTE2 ApplicationObject
        {
            get
            {
                var package = NAntRunnerToolWindowCommand.Instance.ServiceProvider as NAntRunnerVSPackage;
                return package?.Dte2;
            }
        }
        
        /// <summary>
        /// Get or set NAnt the current script file.
        /// </summary>
        public string Filename { get; set; }
        
        /// <summary>
        /// Get the NAntTree of the NAnt script file.
        /// </summary>
        public XmlTree NAntTree { get; private set; }

        /// <summary>
        /// Get the NAntTree of the NAnt script file.
        /// </summary>
        public NAntProcess NAntProcess { get; }
        
        /// <summary>
        /// Get the current active NAntNode.
        /// </summary>
        public XmlNode CurrentNode { get; set; }
        
        /// <summary>
        /// Get the state of the controller.
        /// </summary>
        public bool IsWorking => NAntProcess.IsWorking;
        
        /// <summary>
        /// Get the filter required to select all nant files
        /// </summary>
        public string FileFilter => "Build files (*.build)|*.build|"
                                    + "NAnt files (*.nant)|*.nant|"
                                    + "Xml files (*.xml)|*.xml|"
                                    + "All files  (*.*)|*.*";


        /// <summary>
        /// Get the solution folder, or null if no solution loaded
        /// </summary>
        public string SolutionFolder
        {
            get
            {
                Solution2 solution = VisualStudioUtils.GetSolution(ApplicationObject);

                return solution?.FullName;
            }
        }
        
        /// <summary>
        /// Return the default build script
        /// </summary>
        public string DefaultBuildFile
        {
            get
            {
                string filename = null;

                // First check if a solultion is loaded
                Solution2 solution = VisualStudioUtils.GetSolution(ApplicationObject);

                if (solution == null)
                    return null;

                // Get all build files in solution
                string[] buildFiles = Directory.GetFiles(Path.GetDirectoryName(solution.FullName), "*.build", SearchOption.AllDirectories);

                foreach (string file in buildFiles)
                {
                    if (file.ToLower().EndsWith("default.build"))
                    {
                        filename = file;
                        break;
                    }
                }

                if (Filename == null && buildFiles.Length > 0)
                {
                    filename = buildFiles[0];
                }

                return filename;
            }
        }
        
        /// <summary>
        /// Load NAnt script file in the controller.
        /// </summary>
        public void LoadFile(string filename)
        {
            NAntTree = Filename != null ? XmlTreeFactory.CreateXmlTree(filename, false) : null;
        }

        /// <summary>
        /// Select the line of NAntNode in the VisualStudio editor.
        /// </summary>
        public void SelectNodeLine()
        {
            // First load the file into VisualStudio
            VisualStudioUtils.ShowFile(ApplicationObject, Filename);

            // Refocus AddIn (lost while opening the file)
            VisualStudioUtils.ShowWindow(ApplicationObject, Resources.Common.NAntRunner);

            // Display the node's line within the file
            VisualStudioUtils.ShowLine(ApplicationObject, Filename, CurrentNode.LineNumber, false);

            // Finally display the document
            VisualStudioUtils.ShowDocument(ApplicationObject, Filename);
        }

        /// <summary>
        /// Initialize the thread for NAnt process.
        /// </summary>
        public void StartTarget()
        {
            if (!NAntProcess.IsWorking)
            {
                NAntProcess.Filename   = Filename;
                NAntProcess.TargetNode = CurrentNode;

                // Sets focus to console
                VisualStudioUtils.ShowWindow(ApplicationObject, "Output");

                NAntProcess.Start();
            }
        }

        
        /// <summary>
        /// Stop the current NAnt process.
        /// </summary>
        public void StopTarget()
        {
            if (NAntProcess.IsWorking)
                NAntProcess.Stop();
        }
    }
}
