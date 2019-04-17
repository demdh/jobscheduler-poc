using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Poc.JobScheduling
{
    /// <summary>
    /// Repository for jobs.
    /// </summary>
    public interface IJobRepository
    {
        /// <summary>
        /// Signals that a certain job was updated.
        /// </summary>
        event EventHandler JobUpdated;

        /// <summary>
        /// Checks if the repository contains jobs that have been scheduled for any time in the future.
        /// </summary>
        /// <param name="nextJobStartTime">Gets the time up to which the next job must be executed.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns><c>true</c> if there repository contains job which have to be executed at the given time, else <c>false</c>.</returns>
        Task<bool> HasActiveJobs(out DateTime nextJobStartTime, CancellationToken cancellationToken);

        /// <summary>
        /// Queries all jobs that have been scheduled for the given time or earlier.
        /// </summary>
        /// <param name="nextJobStartTime">Time of the scheduled execution.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A list of jobs to execute.</returns>
        Task<IReadOnlyCollection<IJob>> QueryJobsToStart(DateTime nextJobStartTime, CancellationToken cancellationToken);

        /// <summary>
        /// Update a job.
        /// </summary>
        /// <param name="job">The updated job.</param>
        Task UpdateAsync(IJob job);
    }
}