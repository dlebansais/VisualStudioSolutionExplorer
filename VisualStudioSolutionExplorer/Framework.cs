namespace SlnExplorer
{
    /// <summary>
    /// Defines a framework supported by the project.
    /// </summary>
    public class Framework
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Framework"/> class.
        /// </summary>
        /// <param name="type">The framework type.</param>
        /// <param name="major">The framework major version.</param>
        /// <param name="minor">The framework minor version.</param>
        /// <param name="moniker">The target framework moniker (TFM).</param>
        public Framework(FrameworkType type, int major, int minor, FrameworkMoniker moniker)
        {
            Type = type;
            Major = major;
            Minor = minor;
            Moniker = moniker;
        }

        /// <summary>
        /// Gets the framework type.
        /// </summary>
        public FrameworkType Type { get; init; }

        /// <summary>
        /// Gets the framework major version.
        /// </summary>
        public int Major { get; init; }

        /// <summary>
        /// Gets the framework minor version.
        /// </summary>
        public int Minor { get; init; }

        /// <summary>
        /// Gets the target framework moniker (TFM).
        /// </summary>
        public FrameworkMoniker Moniker { get; init; }
    }
}
