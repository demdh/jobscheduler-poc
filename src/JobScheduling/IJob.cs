using System;
using System.Threading;
using System.Threading.Tasks;

namespace Poc.JobScheduling
{
    /// <summary>
    /// A job.
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// Gets the time schedule for this job.
        /// </summary>
        IJobSchedule Schedule { get; }

        /// <summary>
        /// Executes the job.
        /// </summary>
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}