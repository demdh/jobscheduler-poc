using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;

namespace Poc.JobScheduling
{
    public sealed partial class JobSchedulerTest
    {
        private sealed class Fixture
        {
            private DateTime nextStartTimeForJob;

            private Mock<IJobRepository> jobScheduleRepositoryMock; 
            private CancellationTokenSource cancellationTokenSource;
            private Mock<IJob> jobMock;

            public Fixture()
            {
                jobMock = new Mock<IJob>();
                jobScheduleRepositoryMock = new Mock<IJobRepository>();
                cancellationTokenSource = new CancellationTokenSource();
            }

            public JobScheduler CreateTestObject()
            {
                return new JobScheduler(jobScheduleRepositoryMock.Object, cancellationTokenSource.Token);
            }

            public void SetupSingleJobExecution()
            {
                nextStartTimeForJob = DateTime.Now.AddSeconds(10);

                jobScheduleRepositoryMock
                .Setup(m => m.HasActiveJobs(out nextStartTimeForJob, It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

                jobMock
                    .Setup(m => m.ExecuteAsync(It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

                jobScheduleRepositoryMock
                    .Setup(m => m.QueryJobsToStart(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult((IReadOnlyCollection<IJob>)new[] { jobMock.Object }));

                jobScheduleRepositoryMock
                    .Setup(m => m.UpdateAsync(jobMock.Object))
                    .Callback(cancellationTokenSource.Cancel)
                    .Returns(Task.CompletedTask);
            }

            internal void VerifySingleJobExecution()
            {
                jobMock.Verify(m => m.ExecuteAsync(It.IsAny<CancellationToken>()), Times.Once);
                jobScheduleRepositoryMock.Verify(m => m.HasActiveJobs(out nextStartTimeForJob, It.IsAny<CancellationToken>()), Times.Once);
                jobScheduleRepositoryMock.Verify(m => m.QueryJobsToStart(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once);
                jobScheduleRepositoryMock.Verify(m => m.UpdateAsync(jobMock.Object), Times.Once);
            }
        }
    }
}
