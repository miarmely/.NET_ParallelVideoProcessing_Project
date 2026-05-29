using System.Collections.Concurrent;
using System.Diagnostics;
using OpenCvSharp;
using Parallel_Video_Processing.Models;

namespace Parallel_Video_Processing;

public partial class ProducerConsumer  // private
{
    private readonly int bufferSize;
    private readonly BufferingStrategies strategy;
    private readonly BlockingCollection<Mat> frameBuffer;
    private volatile bool running = false;
    /// <summary>
    /// Processed frame lock. 
    /// </summary>
    private object pFrameLock = new();
    private Mat? pFrameLatest;
    /// <summary>
    /// Processed frame counter. 
    /// </summary>
    private int pFrameCounter = 0;
    private int currentFPS = 0;

    public ProducerConsumer(
        int? bufferSize = 5,
        BufferingStrategies? strategy = BufferingStrategies.DROP_LATEST_FRAME)
    {
        this.bufferSize = bufferSize ?? 5;
        this.frameBuffer = new BlockingCollection<Mat>(bufferSize ?? 5);
        this.strategy = strategy ?? BufferingStrategies.DROP_LATEST_FRAME;
    }

    private void Producer(VideoCapture capture)
    {
        var frame = new Mat();

        while (running)
        {
            capture.Read(frame);

            if (!frame.Empty())
            {
                var copyFrame = frame.Clone();  // clone() => For prevent "Shallow Copy" issue. 

                if (strategy == BufferingStrategies.DROP_LATEST_FRAME)
                    while (!frameBuffer.TryAdd(copyFrame, 100))
                    {
                        if (frameBuffer.TryTake(out var oldFrame))
                            oldFrame.Dispose();
                    }

                else
                    if (!frameBuffer.TryAdd(copyFrame, 100))
                        copyFrame.Dispose();
            }
        }

        frame.Dispose();
        frameBuffer.CompleteAdding();  // send "adding has finished" signal.
    }

    private void Consumer()
    {
        foreach (var frame in frameBuffer.GetConsumingEnumerable())
        {
            var pFrame = ProcessFrame(frame);  // i didn't put this func to in "LOCK statement" because of "less" duration in lock statment.  (Do not dispose this variable.)

            lock (pFrameLock)
            {
                pFrameLatest?.Dispose();
                pFrameLatest = pFrame;
            }

            Interlocked.Increment(ref pFrameCounter);
            frame.Dispose();
        }
    }
}

public partial class ProducerConsumer  // video processings
{
    private Mat ProcessFrame(Mat frame)
    {
        var gray = new Mat();
        Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);

        var edges = new Mat();
        Cv2.Canny(gray, edges, 100, 200);

        gray.Dispose();
        return edges;
    }
}

public partial class ProducerConsumer  // main
{
    public async Task StartAsync()
    {
        // start video capture
        var capture = new VideoCapture(0);
        if (!capture.IsOpened()) throw new Exception("Self camera of PC cannot open.");
        Console.WriteLine("Gerçek Zamanlı Video İşleme Başladı!");
        Console.WriteLine("Çıkış yapmak için ESC tuşuna basınız.");
        Console.WriteLine($"Buffer Size: {bufferSize} | Buffering Strategy: {strategy}");
        running = true;

        // start producers & consumers threads
        var producerTask = Task.Run(() => Producer(capture));
        var consumerTask1 = Task.Run(() => Consumer());

        // main thread
        var stopwatch = Stopwatch.StartNew();
        while (true)
        {
            // display frame
            Mat? frameToShow = null;
            lock (pFrameLock)
            {
                frameToShow = pFrameLatest;
                pFrameLatest = null;
            }
            if (frameToShow != null)
            {
                // get processed total frame count per 1s
                if (stopwatch.ElapsedMilliseconds >= 1000)
                {
                    currentFPS = Interlocked.Exchange(ref pFrameCounter, 0);
                    stopwatch.Restart();
                }

                Cv2.PutText(frameToShow,
                    $"FPS: {currentFPS}",
                    new Point(5, 20),
                    HersheyFonts.HersheyComplex,
                    0.5,
                    Scalar.White);
                Cv2.ImShow("Processed Video", frameToShow);

                frameToShow.Dispose();
            }

            // check "ESC" key whether pressed
            var keyCode = Cv2.WaitKey(1);
            if (keyCode == 27)
            {
                running = false;
                Cv2.DestroyAllWindows();
                break;
            }
        }

        await producerTask;
        await consumerTask1;
        capture.Release();
        capture.Dispose();
        pFrameLatest?.Dispose();
    }
}