namespace spaar
{
    /// <summary>
    /// Basic datastore (singleton) for mod loader statistics.
    /// </summary>
    internal class ModLoaderStats
    {
        private static readonly ModLoaderStats instance = new ModLoaderStats();

        public static ModLoaderStats Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Whether or not the loader was already loaded this game session and in turn loaded all mods.
        /// Due to the way the internal mod loader works, the ModLoader component can be added more than once,
        /// however the mods and mod loader components should only be added once.
        /// </summary>
        public bool WasLoaded { get; set; }

        private ModLoaderStats() { }
    }
}
