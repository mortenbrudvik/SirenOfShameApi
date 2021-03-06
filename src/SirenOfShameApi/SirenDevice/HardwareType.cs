namespace SirenOfShameApi.SirenDevice
{
    /// <summary>
    /// The type of Siren of Shame device.
    /// </summary>
    internal enum HardwareType : byte
    {
        /// <summary>
        /// No additional memory for audio and LED patterns.
        /// </summary>
        Standard = 1,

        /// <summary>
        /// Additional memory for audio and LED patterns.
        /// </summary>
        Pro = 2
    }
}