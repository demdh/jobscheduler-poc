using System;

namespace Poc.JobScheduling
{
  public class JobExecutionFailureEventArgs : EventArgs
  {
    public JobExecutionFailureEventArgs(IJob job, Exception exception)
    {
      this.Job = job ?? throw new ArgumentNullException(nameof(job));
      this.Exception = exception ?? throw new ArgumentNullException(nameof(exception));
    }

    public IJob Job { get; }

    public Exception Exception { get; }
  }
}