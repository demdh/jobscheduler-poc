using System;
using System.Threading;
using System.Threading.Tasks;

namespace Poc.JobScheduling
{
    public interface IJob
    {
        IJobSchedule Schedule { get; }

        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
