using System;

public interface IJobSchedule
{
    DateTime? NextStartTime { get; }
}
