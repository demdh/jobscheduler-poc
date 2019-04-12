using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Poc.JobScheduling
{
    public interface IJobRepository
    {
        event EventHandler JobUpdated;

        Task<DateTime?> GetNextJobStart(CancellationToken cancellationToken);

        Task<IReadOnlyCollection<IJob>> QueryJobsToStart(CancellationToken cancellationToken);

        Task UpdateAsync(IJob job);
    }
}