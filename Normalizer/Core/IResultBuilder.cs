namespace Normalizer.Core
{
    public interface IResultBuilder
    {
        void Add(string line);

        NormalizationResult Build();
    }
}