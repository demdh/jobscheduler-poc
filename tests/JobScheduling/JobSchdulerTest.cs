using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Poc.JobScheduling
{
    [TestClass]
    public sealed partial class JobSchedulerTest : IDisposable
    {
        private Fixture fixture;

        [TestInitialize]
        public void Setup()
        {
            this.fixture = new Fixture();
        }

        [TestCleanup]
        public void Dispose()
        {
            this.fixture = null;
        }

        [TestMethod]
        public async Task RunAsync_ShouldInvokeRepository()
        {
            this.fixture.JobScheduleRepositoryMock
                .Setup(m => m.GetNextJobStartTime(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(DateTime.Now.AddSeconds(10)));

            Mock<IJobSchedule> jobMock = new Mock<IJobSchedule>();
            jobMock
                .Setup(m => m.ExecuteAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            this.fixture.JobScheduleRepositoryMock
                .Setup(m => m.QueryJobsToStart(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((IReadOnlyCollection<IJobSchedule>)new[] { jobMock.Object }));


            this.fixture.JobScheduleRepositoryMock
                .Setup(m => m.UpdateAsync(jobMock.Object))
                .Callback(this.fixture.CancellationTokenSource.Cancel)
                .Returns(Task.CompletedTask);

            var testObject = this.fixture.CreateTestObject();

            try
            {
                await testObject.RunAsync();
            }
            catch (TaskCanceledException)
            {
            }

            jobMock.Verify(m => m.ExecuteAsync(It.IsAny<CancellationToken>()), Times.Once);
            this.fixture.JobScheduleRepositoryMock.Verify(m => m.GetNextJobStartTime(It.IsAny<CancellationToken>()), Times.Once);
            this.fixture.JobScheduleRepositoryMock.Verify(m => m.QueryJobsToStart(It.IsAny<CancellationToken>()), Times.Once);
            this.fixture.JobScheduleRepositoryMock.Verify(m => m.UpdateAsync(jobMock.Object), Times.Once);
        }
    }
}
