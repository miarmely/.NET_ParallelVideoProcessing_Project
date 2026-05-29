using Parallel_Video_Processing.Models;

namespace Parallel_Video_Processing;

public class Program
{
    static async Task Main(string[] args)
    {

        int? bufferSize = args.IndexOf("buffersize") != -1 ? int.Parse(args[args.IndexOf("buffersize") + 1]) : null;
        var strategy = args.IndexOf("strategy") != -1 ? args[args.IndexOf("strategy") + 1] : null;
        var producerConsumer = new ProducerConsumer(
            bufferSize: bufferSize,
            strategy: strategy != null ? Enum.Parse<BufferingStrategies>(strategy) : null);

        await producerConsumer.StartAsync();
    }
}
