using System;
using System.Threading;
using System.Threading.Tasks;

namespace Poc.JobScheduling
{
    public class JobScheduler : IJobScheduler, IDisposable
    {
        private readonly IJobRepository jobRepository;
        private readonly CancellationToken externalToken;
        private readonly OnetimeAsyncDelegateInvocation processJobsOnetimeDelegate;
        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken internalToken;

        public JobScheduler(IJobRepository jobRepository, CancellationToken cancellationToken)
        {
            this.jobRepository = jobRepository ?? throw new ArgumentNullException(nameof(jobRepository));
            this.externalToken = cancellationToken;
            
            processJobsOnetimeDelegate = new OnetimeAsyncDelegateInvocation(ProcessJobsAsync);
            internalToken = new CancellationToken(true);

            jobRepository.JobUpdated += CancelDelay;
        }

        public event EventHandler<JobExecutionFailureEventArgs> JobExecutionFailed;

        public bool IsRunning => processJobsOnetimeDelegate.IsInvoked;

        public bool IsCanceled => externalToken.IsCancellationRequested;

        public void Dispose()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = null;
                jobRepository.JobUpdated -= CancelDelay;
            }
        }

        public async Task RunAsync()
        {
            if (IsCanceled)
            {
                throw new InvalidOperationException("The scheduler has already been canceled.");
            }

            await processJobsOnetimeDelegate.InvokeAsync();
        }

        private async Task ProcessJobsAsync()
        {
            while (!this.externalToken.IsCancellationRequested)
            {
                EnsureUncanceledToken();
                await WaitForNextStartTimeAsync();
                await ExecuteJobsAsync();
            }
        }

        private async Task WaitForNextStartTimeAsync()
        {
            try
            {
                TimeSpan waitTimesSpan = await GetWaitTimeSpan();
                await Task.Delay(waitTimesSpan, internalToken);
            }
            catch (OperationCanceledException)
            { }
        }

        private async Task<TimeSpan> GetWaitTimeSpan()
        {
            TimeSpan waitTimesSpan = TimeSpan.FromMilliseconds(-1);
            DateTime nextJobStartTime;

            var hasActiveJobs = await jobRepository.HasActiveJobs(out nextJobStartTime, internalToken);
            if (hasActiveJobs)
            {
                waitTimesSpan = nextJobStartTime - DateTime.Now;

                if (waitTimesSpan < TimeSpan.FromMilliseconds(0)) 
                {
                    waitTimesSpan = TimeSpan.FromMilliseconds(0);
                }
            }

            return waitTimesSpan;
        }

        private async Task ExecuteJobsAsync()
        {
            var jobs = await jobRepository.QueryJobsToStart(externalToken);
            Parallel.ForEach(jobs, async job => await ExecuteJobAsync(job));
        }

        private async Task ExecuteJobAsync(IJob job)
        {
            try
            {
                await job.ExecuteAsync(externalToken);
                await jobRepository.UpdateAsync(job);
            }
            catch (Exception exception)
            {
                OnJobExecutionFailed(job, exception);
            }
        }

        private void EnsureUncanceledToken()
        {
            if (internalToken.IsCancellationRequested)
            {
                CreateCancellationToken();
            }
        }

        private void CreateCancellationToken()
        {
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
            internalToken = cancellationTokenSource.Token;
        }

        private void CancelDelay(object sender, EventArgs eventArgs)
        {
            cancellationTokenSource?.Cancel();
        }

        private void OnJobExecutionFailed(IJob job, Exception exception)
        {
            JobExecutionFailed?.Invoke(this, new JobExecutionFailureEventArgs(job, exception));
        }
    }
}