using System;
using System.Threading.Tasks;

namespace Poc
{
    internal sealed class OnetimeAsyncDelegateInvocation
    {
        private readonly Lazy<Task> task;

        public OnetimeAsyncDelegateInvocation(Func<Task> asyncDelegate)
        {
            this.task = new Lazy<Task>(asyncDelegate);
            AsyncDelegate = asyncDelegate;
        }

        public bool IsInvoked => this.task.IsValueCreated;

        public Func<Task> AsyncDelegate { get; }

        public async Task InvokeAsync()
        {
            if (!this.task.IsValueCreated)
            {
                await this.task.Value;
            }
        }
    }
}