using System.Threading;
using Moq;

namespace Poc.JobScheduling
{
    public sealed partial class JobSchedulerTest
    {
        private sealed class Fixture
        {
            public Fixture()
            {
                this.JobScheduleRepositoryMock = new Mock<IJobScheduleRepository>();
                this.CancellationTokenSource = new CancellationTokenSource();
            }

            public Mock<IJobScheduleRepository> JobScheduleRepositoryMock { get; }

            public CancellationTokenSource CancellationTokenSource { get; }

            public JobScheduler CreateTestObject()
            {
                return new JobScheduler(this.JobScheduleRepositoryMock.Object, this.CancellationTokenSource.Token);
            }
        }
    }
}
