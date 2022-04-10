using System;
using System.Threading.Tasks;

namespace Normalizer.Core
{
    public interface IScheduler
    {
        event EventHandler<ProcessingItemCreatedEventArgs>? ProcessingItemCreated;

        event EventHandler? Finished;

        bool Start();

        bool Stop();
    }
}
