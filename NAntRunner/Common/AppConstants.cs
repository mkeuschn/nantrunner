namespace NAntRunner.Common
{
    public static class AppConstants
    {
        /// <summary>
        /// Xml tree constants.
        /// </summary>
        public const string NANT_XML_TARGET = "target";
        /// <summary>
        /// The XML tag include name
        /// </summary>
        public const string NANT_XML_INCLUDE = "include";
        /// <summary>
        /// The XML tag property name
        /// </summary>
        public const string NANT_XML_PROPERTY = "property";
        /// <summary>
        /// The XML tag decription name
        /// </summary>
        public const string NANT_XML_DESCRIPTION = "description";
        /// <summary>
        /// The XML tag build filename
        /// </summary>
        public const string NANT_XML_BUILDFILE = "buildfile";

        #region Icons

        public static string IconPath => "pack://application:,,,/NAntRunner;component/Resources/";

        public static string IconCancel => IconPath + "action_Cancel_16xLG.png";
        public static string IconBullets => IconPath + "Bullets_11690.png";
        public static string IconHelp => IconPath + "DynamicHelp_5659.png";
        public static string IconOpen => IconPath + "folder_Open_16xLG.png";
        public static string IconGear => IconPath + "gear_16xLG.png";
        public static string IconOpenArrow => IconPath + "Open_6529.png";
        public static string IconRestart => IconPath + "Restart_6322.png";
        public static string IconPlayLight => IconPath + "startwithoutdebugging_6556.png";
        public static string IconHelpLight => IconPath + "Symbols_Help_and_inclusive_16xLG.png";
        public static string IconStart => IconPath + "Symbols_Play_16xLG.png";
        public static string IconStop => IconPath + "Symbols_Stop_16xLG.png";
        public static string IconStroke => IconPath + "Stroke.png";
        public static string IconRefresh => IconPath + "Synchronize_16xLG.png";
        public static string IconTest => IconPath + "test_16xLG.png";

        #endregion
    }
}
