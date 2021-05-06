namespace SlnExplorer
{
    /// <summary>
    /// Values for the nullable project element.
    /// </summary>
    public enum NullableAnnotation
    {
        /// <summary>
        /// No nullable value.
        /// </summary>
        None,

        /// <summary>
        /// Nullable enabled.
        /// </summary>
        Enable,

        /// <summary>
        /// All nullability warnings are enabled.
        /// </summary>
        Warnings,

        /// <summary>
        /// All nullability warnings are disabled.
        /// </summary>
        Annotations,

        /// <summary>
        /// Nullable disabled.
        /// </summary>
        Disable,
    }
}
