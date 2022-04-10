namespace Normalizer.Core
{
    public class StatusChangedEventArgs
    {
        public StatusChangedEventArgs(ProcessingItemStatus status, ProcessingItem owner)
        {
            Status = status;
            Owner = owner;
        }

        public ProcessingItemStatus Status { get; }
        public ProcessingItem Owner { get; }
    }
}