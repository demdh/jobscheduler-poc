using System;
using System.Threading.Tasks;

namespace Poc.JobScheduling
{
    /// <summary>
    /// Scheduler for jobs.
    /// </summary>
    public interface IJobScheduler
    {
        /// <summary>
        /// Signals errors during job execution.
        /// </summary>
        event EventHandler<JobExecutionFailureEventArgs> JobExecutionFailed;

        /// <summary>
        /// Gets whether this schedluer instance is running.
        /// </summary>
        bool IsRunning { get; }
        
        /// <summary>
        /// Gets whether this schedluer instance is canceled.
        /// </summary>
        bool IsCanceled { get; }

        /// <summary>
        /// Start the job scheduling.
        /// </summary>
        Task RunAsync();
    }
}