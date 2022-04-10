using Normalizer.Services;
using System;
using System.Diagnostics;

namespace Normalizer.Core
{
    public class ArgumentBuilder : IArgumentBuilder
    {
        public ArgumentBuilder(LoudNormSettings lnSettings)
        {
            I = lnSettings.I;
            Lra = lnSettings.Lra;
            Tp = lnSettings.Tp;
            Offset = lnSettings.Offset;
            DualMono = lnSettings.DualMono;
            MeasuredPriority = lnSettings.MeasuredPrioiry;
        }

        public float I { get; }

        public float Lra { get; }

        public float Tp { get; }

        public float Offset { get; }

        public bool DualMono { get; }

        public MeasuredResultPriority MeasuredPriority { get; }

        public string GetArguments(string inputFile, string outputFile, NormalizationResult? result)
        {
            if (result == null)
                return GenerateArgsWithOptions(inputFile, outputFile);
            else
                return GenerateArgsWithMeasured(inputFile, outputFile, result);
        }

        private string GenerateArgsWithMeasured(string inputFile, string outputFile, NormalizationResult result)
        {
            string args;

            switch (MeasuredPriority)
            {
                case MeasuredResultPriority.Input:
                    args = $"-i {inputFile} -af loudnorm=measured_i={result.InputI:N1}:measured_lra={result.InputLra:N1}:measured_tp={result.InputTp:N1}" +
                        $":measured_thresh={result.OutputThresh:N1}:linear=true:offset={Offset:N1}:dual_mono={DualMono} {outputFile}";
                    break;

                case MeasuredResultPriority.Output:
                    args = $"-i {inputFile} -af loudnorm=measured_i={result.OutputI:N1}:measured_lra={result.OutputLra:N1}:measured_tp={result.OutputTp:N1}" +
                        $":measured_thresh={result.OutputThresh:N1}:linear=true:offset={Offset:N1}:dual_mono={DualMono} {outputFile}";
                    break;

                default:
                    Debug.Fail($"Not expected '{MeasuredPriority}' as a value for MeasuredPriority property.");
                    throw new InvalidOperationException($"'{MeasuredPriority}' is not an acceptable value for MeasuredPriority property.");
            }

            return args;
        }

        private string GenerateArgsWithOptions(string inputFile, string outputFile)
        {
            return $"-i {inputFile} -af loudnorm=i={I:N1}:lra={Lra:N1}:tp={Tp:N1}:offset={Offset:N1}:dual_mono={DualMono} {outputFile}";
        }
    }
}
