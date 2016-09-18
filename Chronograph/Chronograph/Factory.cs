namespace Mash.Chronograph
{
    /// <summary>
    /// Constructs objects supporting the chronograph interfaces
    /// </summary>
    public static class ChronographFactory
    {
        /// <summary>
        /// Creates an object which supports IChronograph
        /// </summary>
        /// <param name="name">The name of the chronograph</param>
        public static IChronograph GetChronograph(string name = "")
        {
            return new StopwatchChronograph(name);
        }
    }
}
