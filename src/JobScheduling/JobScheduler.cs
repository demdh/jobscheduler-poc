using System;
using System.Threading;
using System.Threading.Tasks;

namespace Poc.JobScheduling
{
    public sealed class JobScheduler : IJobScheduler, IDisposable
    {
        private readonly IJobRepository jobRepository;
        private readonly CancellationToken externalToken;
        private readonly OnetimeAsyncDelegateInvocation processJobsOnetimeDelegate;
        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken internalToken;

        public JobScheduler(IJobRepository jobRepository, CancellationToken cancellationToken)
        {
            this.jobRepository = jobRepository;
            this.externalToken = cancellationToken;
            this.processJobsOnetimeDelegate = new OnetimeAsyncDelegateInvocation(this.ProcessJobsAsync);
            this.internalToken = new CancellationToken(true);
            this.jobRepository.JobUpdated += this.CancelDelay;
        }

        public event EventHandler<JobExecutionFailureEventArgs> JobExecutionFailed;

        public bool IsRunning => this.processJobsOnetimeDelegate.IsInvoked;

        public bool IsCanceled => this.externalToken.IsCancellationRequested;

        public void Dispose()
        {
            if (this.cancellationTokenSource != null)
            {
                this.cancellationTokenSource?.Dispose();
                this.cancellationTokenSource = null;
                this.jobRepository.JobUpdated -= this.CancelDelay;
            }
        }

        public async Task RunAsync()
        {
            if (this.IsCanceled)
            {
                throw new InvalidOperationException("The scheduler has already been canceled.");
            }

            await this.processJobsOnetimeDelegate.InvokeAsync();
        }

        private async Task ProcessJobsAsync()
        {
            while (!this.externalToken.IsCancellationRequested)
            {
                this.EnsureUncanceledToken();
                await this.WaitForNextStartTimeAsync();
                await this.ExecuteJobsAsync();
            }
        }

        private async Task WaitForNextStartTimeAsync()
        {
            try
            {
                var nextStart = await this.jobRepository.GetNextJobStart(this.internalToken);

                if (nextStart == null)
                {
                    nextStart = DateTime.MaxValue;
                }

                await Task.Delay(nextStart.Value - DateTime.Now, this.internalToken);
            }
            catch (OperationCanceledException)
            { }
        }

        private async Task ExecuteJobsAsync()
        {
            var jobs = await this.jobRepository.QueryJobsToStart(this.externalToken);
            Parallel.ForEach(jobs, async job => await this.ExecuteJobAsync(job));
        }

        private async Task ExecuteJobAsync(IJob job)
        {
            try
            {
                await job.ExecuteAsync(this.externalToken);
                await this.jobRepository.UpdateAsync(job);
            }
            catch (Exception exception)
            {
                this.OnJobExecutionFailed(job, exception);
            }
        }

        private void EnsureUncanceledToken()
        {
            if (this.internalToken.IsCancellationRequested)
            {
                this.CreateCancellationToken();
            }
        }

        private void CreateCancellationToken()
        {
            this.cancellationTokenSource?.Dispose();
            this.cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this.externalToken);
            this.internalToken = this.cancellationTokenSource.Token;
        }

        private void CancelDelay(object sender, EventArgs eventArgs)
        {
            this.cancellationTokenSource?.Cancel();
        }

        private void OnJobExecutionFailed(IJob job, Exception exception)
        {
            this.JobExecutionFailed?.Invoke(this, new JobExecutionFailureEventArgs(job, exception));
        }
    }
}