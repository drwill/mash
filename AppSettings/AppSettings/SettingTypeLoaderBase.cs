namespace Mash.AppSettings
{
    /// <summary>
    /// A base class for setting type loaders
    /// </summary>
    internal abstract class SettingTypeLoaderBase
    {
        /// <summary>
        /// The next setting type loader in the chain of responsibility
        /// </summary>
        public SettingTypeLoaderBase Next { get; set; }

        /// <summary>
        /// Indicates whether or not this setting type matches the specified member
        /// </summary>
        /// <param name="member">The property to evaluate</param>
        /// <returns>True if a match, otehrwise false</returns>
        internal virtual bool DoWork(SettingTypeModel model)
        {
            if (Next != null)
            {
                return Next.DoWork(model);
            }

            return false;
        }
    }
}
