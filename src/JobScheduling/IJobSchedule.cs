using System;

namespace Poc.JobScheduling 
{
    /// <summary>
    /// A time schedule for a job.
    /// </summary>
    public interface IJobSchedule
    {
        /// <summary>
        /// Gets the next start time for this schedule.
        /// Returns <c>null</c> if there is no further start scheduled.
        /// </summary>
        NextStartTime NextStartTime { get; }

        /// <summary>
        /// Indicating whether the schedule has a next start time.
        /// </summary>
        bool HasNextStartTime { get; }
    }
}