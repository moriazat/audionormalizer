using Normalizer.Core;

namespace Normalizer.Services
{
    public interface ISettingsService
    {
        uint ParallelProcessesCount { get; }

        string FfmpegPath { get; }

        NormalizationPass NormalizationPass { get; }

        LoudNormSettings LoudNormSettings { get; }
    }
}
