using System;

namespace Poc.JobScheduling
{
  public sealed class JobExecutionFailureEventArgs : EventArgs
  {
    public JobExecutionFailureEventArgs(IJob job, Exception exception)
    {
      this.Job = job;
      this.Exception = exception;
    }

    public IJob Job { get; }

    public Exception Exception { get; }
  }
}