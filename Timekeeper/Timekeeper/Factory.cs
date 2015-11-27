using System;

namespace Mash.Timekeeper
{
    /// <summary>
    /// Constructs objects supporting the time keeper interfaces
    /// </summary>
    public static class Factory
    {
        /// <summary>
        /// Creates an object which supports ITimekeeper
        /// </summary>
        /// <param name="name">The name of the time keeper</param>
        public static ITimekeeper GetTimekeeperInstance(string name = "")
        {
            return new StopwatchWrapper(name);
        }
    }
}
