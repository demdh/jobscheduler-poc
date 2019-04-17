using System;

namespace Poc.JobScheduling
{
  /// <summary>
  /// Contains Information regarding errors while execution a job.
  /// </summary>
  public class JobExecutionFailureEventArgs : EventArgs
  {
    internal JobExecutionFailureEventArgs(IJob job, Exception exception)
    {
      this.Job = job ?? throw new ArgumentNullException(nameof(job));
      this.Exception = exception ?? throw new ArgumentNullException(nameof(exception));
    }

    /// <summary>
    /// Gets the job.
    /// </summary>
    public IJob Job { get; }

    /// <summary>
    /// Gets the exception.
    /// </summary>
    public Exception Exception { get; }
  }
}