using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.BlankTable
{
    /// <summary>
    /// Represents settings of the Blank Table plugin
    /// </summary>
    public class BlankTableSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a description text
        /// </summary>
        public string DescriptionText { get; set; }

        /// <summary>
        /// Gets or sets a value for showing or hiding the Plugin's dashboard widget.
        /// </summary>
        public bool EnableDashboardWidget { get; set; }

        /// <summary>
        /// Gets or Sets a value to install sample data.
        /// </summary>
        public bool InstallSampleData { get; set; }
    }
}
