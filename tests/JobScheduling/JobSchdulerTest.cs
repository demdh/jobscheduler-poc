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
            fixture = new Fixture();
        }

        [TestCleanup]
        public void Dispose()
        {
            fixture = null;
        }

        [TestMethod]
        public async Task RunAsync_ShouldInvokeRepository()
        {
            fixture.SetupSingleJobExecution();

            var testObject = fixture.CreateTestObject();

            try
            {
                await testObject.RunAsync();
            }
            catch (TaskCanceledException)
            { }

            fixture.VerifySingleJobExecution();
        }
    }
}
