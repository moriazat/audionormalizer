using System;

namespace Normalizer.Core
{
    public class ProcessingItemCreatedEventArgs : EventArgs
    {
        public ProcessingItemCreatedEventArgs(ProcessingItem processingItem)
        {
            ProcessingItem = processingItem;
        }

        public ProcessingItem ProcessingItem { get; }
    }
}