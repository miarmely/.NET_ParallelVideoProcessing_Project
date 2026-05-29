namespace Parallel_Video_Processing.Models;

public enum BufferingStrategies
{
    /// <summary>
    /// Some frames can drop. If frame buffer full then drop oldest frame. Provides low-latency.
    /// </summary>
    DROP_LATEST_FRAME = 1,
    /// <summary>
    /// Do not drop any frame. All frames will be handled. But high latency.
    /// </summary>
    PROCESS_ALL_FRAMES = 2
}
