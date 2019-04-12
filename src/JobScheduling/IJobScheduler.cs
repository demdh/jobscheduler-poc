using System;
using System.Threading.Tasks;

namespace Poc.JobScheduling
{
    public interface IJobScheduler
    {
        event EventHandler<JobExecutionFailureEventArgs> JobExecutionFailed;
        bool IsRunning { get; }
        bool IsCanceled { get; }
        Task RunAsync();
    }
}