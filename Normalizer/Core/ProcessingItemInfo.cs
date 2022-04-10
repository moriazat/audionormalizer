using Normalizer.Utils;
using System.Diagnostics;

namespace Normalizer.Core
{
    public class ProcessingItemInfo
    {
        public ProcessingItemInfo(string inputFile, string outputFile, Process process)
        {
            InputFile = inputFile.AssertThrowNonEmpty(nameof(inputFile));
            OutputFile = outputFile.AssertThrowNonEmpty(nameof(outputFile));
            Process = process;
        }

        public string InputFile { get; }
        public string OutputFile { get; }
        public Process Process { get; }
    }
}
