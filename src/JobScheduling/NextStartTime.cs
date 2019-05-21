using System;

namespace Poc.JobScheduling
{
    public class NextStartTime
    {
        internal NextStartTime(DateTime value)
        {
            this.Value = value;
        }

        public DateTime Value { get; }

        public static NextStartTime Create(DateTime value) => new NextStartTime(value);
    }
}