namespace Normalizer.Core
{
    public interface IArgumentBuilder
    {
        string GetArguments(string inputFile, string outputFile, NormalizationResult? result);
    }
}