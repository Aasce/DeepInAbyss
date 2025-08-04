namespace Asce.Game.Stats
{
    /// <summary>
    ///     Represents the type of value a stat can have.
    /// </summary>
    public enum StatValueType
    {
        /// <summary>
        ///     The base value of the stat.
        /// </summary>
        Base,

        /// <summary>
        ///     A flat value added to the stat.
        /// </summary>
        Flat,

        /// <summary>
        ///     A ratio or percentage modifier for the stat.
        /// </summary>
        Ratio,

        /// <summary>
        ///     A scaling factor applied to the stat.
        /// </summary>
        Scale,
    }
}
