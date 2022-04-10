using Normalizer.Core;

namespace Normalizer.Services
{
    public class LoudNormSettings
    {
        public float I { get; set; }

        public float Lra { get; set; }

        public float Tp { get; set; }
        
        public float Offset { get; set; }

        public bool DualMono { get; set; }

        /// <summary>
        /// Indicates how many passes should be run on each file
        /// </summary>
        public NormalizationPass Pass { get; set; }

        /// <summary>
        /// Indicates that on the second pass on a file, which type of
        /// measured values from the results of the first pass should be
        /// taken as the inputs for the measured_X options
        /// </summary>
        public MeasuredResultPriority MeasuredPrioiry { get; set; }
    }
}