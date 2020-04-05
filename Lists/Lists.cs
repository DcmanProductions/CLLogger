namespace ChaseLabs.CLLogger
{
    /// <summary>
    /// <para>
    /// Author: Drew Chase
    /// </para>
    /// <para>
    /// Company: Chase Labs
    /// </para>
    /// </summary>
    public class Lists
    {

        /// <summary>
        /// <list type="table">
        /// <listheader>Log Importance Order</listheader>
        /// <item>
        /// All: Display All Logs
        /// </item>
        /// <item>
        /// Debug: Display All Logs
        /// </item>
        /// <item>
        /// Info: Display Logs From Info to Fatal
        /// </item>
        /// <item>
        /// Warn: Display Logs from Warn to Fatal
        /// </item>
        /// <item>
        /// Error: Display Logs from Error to Fatal
        /// </item>
        ///<item>
        /// Fatal: Display only Fatal Logs
        ///</item>
        /// </list>
        /// </summary>
        public enum LogTypes
        {
            All,
            Debug,
            Info,
            Warn,
            Error,
            Fatal
        }

        /// <summary>
        /// <list type="table">
        /// <item>
        /// None: The Log File will Never Roll Over (Not Recommended)
        /// </item>
        /// <item>
        /// Date: The Log File will Roll Over every day
        /// </item>
        /// <item>
        /// Size: The Log File will Roll Over once the File has reached a specified Size.
        /// </item>
        /// <item>
        /// OnLaunch: The Log File will Roll Over upon the application closing
        /// </item>
        /// </list>
        /// </summary>
        private enum RollOverType
        {
            None,
            Date,
            Size,
            OnLaunch
        }

        /// <summary>
        /// <list type="table">
        /// <listheader>The File Name Change when Rolling Over</listheader>
        /// <item>
        /// Date: The File will change to the Current Date and Time.
        /// </item>
        /// <item>
        /// Number: The File will change to a Incremented Number
        /// <code>latest.log -> 1.log -> 2.log -></code>
        /// </item>
        /// </list>
        /// </summary>
        private enum RollOverFormatType
        {
            Date,
            Number
        }
    }
}
