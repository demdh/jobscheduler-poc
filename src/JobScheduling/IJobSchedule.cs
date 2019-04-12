using System;

public interface IJobSchedule
{
    DateTime? NextStart { get; }
}
