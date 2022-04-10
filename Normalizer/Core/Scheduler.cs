using Microsoft.Extensions.Logging;
using Normalizer.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Normalizer.Core
{
    public class Scheduler : IScheduler, IDisposable
    {
        private string[] files;
        private readonly ILogger logger;
        private readonly IArgumentBuilder argBuilder;
        private readonly ISettingsService settingsService;
        private readonly NormalizationPass pass;
        private object lockObj;
        private int currIndex;
        private uint activeItemsCount;
        private CancellationTokenSource cancellationSource;

        public event EventHandler<ProcessingItemCreatedEventArgs>? ProcessingItemCreated;

        public event EventHandler? Finished;

        public Scheduler(ILogger logger, IArgumentBuilder argBuilder, ISettingsService settingsService, string[] files, NormalizationPass pass)
        {
            this.logger = logger;
            this.argBuilder = argBuilder;
            this.settingsService = settingsService;
            this.files = files;
            this.pass = pass;
            this.currIndex = -1;
            this.activeItemsCount = 0;
            cancellationSource = new CancellationTokenSource();
            lockObj = new object();
        }

        public bool Start()
        {
            return ThreadPool.QueueUserWorkItem(InternalStart, null);
        }

        public bool Stop()
        {
            try
            {
                cancellationSource.Cancel();
            }
            catch (AggregateException agEx)
            {
                var msg = "Something went wrong while cancelling the jobs.";
                Debug.Fail(msg);
                logger.LogError(agEx, msg);
                return false;
            }

            return true;
        }
        public void Dispose()
        {
            cancellationSource.Dispose();
        }

        private void InternalStart(object? state)
        {
            for (int i = 0; i < settingsService.ParallelProcessesCount; i++)
                StartNextFile();
        }

        private void StartNextFile()
        {
            if (currIndex == files.Length - 1)
            {
                if (activeItemsCount == 0)
                    Finished?.Invoke(this, new EventArgs());

                return;
            }

            lock (lockObj)
            {
                if (cancellationSource.Token.IsCancellationRequested ||
                    activeItemsCount >= settingsService.ParallelProcessesCount)
                    return;

                activeItemsCount++;
            }

            var index = Interlocked.Increment(ref currIndex);

            StartPass(files[index], NormalizationPass.One);
        }

        private void StartPass(string fileName, NormalizationPass pass, NormalizationResult? result = null)
        {
            var processingItem = CreateProcessingItem(fileName, pass, result);
            SubscribeTo(processingItem);

            ProcessingItemCreated?.Invoke(this, new ProcessingItemCreatedEventArgs(processingItem));

            processingItem.Start();
        }

        private ProcessingItem CreateProcessingItem(string inputFile, NormalizationPass pass, NormalizationResult? result)
        {
            var outputFile = GetOutputName(inputFile, pass);
            string? args = null;

            var p = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = settingsService.FfmpegPath,
                    Arguments = argBuilder.GetArguments(inputFile, outputFile, result)
                }
            };

            var pInfo = new ProcessingItemInfo(inputFile, outputFile, p);
            return new ProcessingItem(logger, pInfo, new ResultBuilder(), pass, cancellationSource.Token);
        }

        private string GetOutputName(string file, NormalizationPass pass)
        {

            string postfix = string.Empty;
            switch (pass)
            {
                case NormalizationPass.One:
                    postfix = "pass1";
                    break;
                case NormalizationPass.Two:
                    postfix = "pass2";
                    break;
                case NormalizationPass.None:
                    Debug.Fail("Expected either first or second pass for normalization pass.");
                    break;
            }

            return $"{Path.GetFileNameWithoutExtension(file)}-normalized-{postfix}";
        }

        private void SubscribeTo(ProcessingItem pItem)
        {
            pItem.StatusChanged += OnProcessItemStatusChanged;
        }

        private void UnsubscribeFrom(ProcessingItem pItem)
        {
            pItem.StatusChanged -= OnProcessItemStatusChanged;
        }

        private void OnProcessItemStatusChanged(object? sender, StatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case ProcessingItemStatus.PassOneFinished:
                    UnsubscribeFrom(e.Owner);
                    if (pass == NormalizationPass.One)
                    {
                        // this is all done and move on to another file
                        Interlocked.Decrement(ref activeItemsCount);
                        StartNextFile();
                    }
                    else if (pass == NormalizationPass.Two)
                    {
                        StartPass(e.Owner.InputFileName, NormalizationPass.Two, e.Owner.Result);
                    }
                    break;
                
                case ProcessingItemStatus.PassTwoFinished:
                    UnsubscribeFrom(e.Owner);
                    // this is all done and move on to another file
                    Interlocked.Decrement(ref activeItemsCount);
                    StartNextFile();
                    break;

                case ProcessingItemStatus.Faulted:
                    UnsubscribeFrom(e.Owner);
                    break;
            }
        }
    }
}
