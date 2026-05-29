using System.Diagnostics;
using OpenCvSharp;

namespace Parallel_Video_Processing;

public partial class MiarVideo  // private
{
    private Mat DetectEdgesOnFrame(Mat frame, bool useParallel = true)
    {
        var copyFrame = frame.Clone();
        var rowCount = copyFrame.Rows;
        var colCount = copyFrame.Cols;

        // convert frame to "grayscale" manually (PARALLEL OR NOT)
        if (useParallel)
            Parallel.For(0, rowCount, rowInd =>
            {
                for (int colInd = 0; colInd < colCount; colInd += 1)
                {
                    var oldRGB = copyFrame.At<Vec3b>(rowInd, colInd);
                    var gray = (byte)((oldRGB.Item0 + oldRGB.Item1 + oldRGB.Item2) / 3);
                    var newRGB = new Vec3b(gray, gray, gray);
                    copyFrame.Set(rowInd, colInd, newRGB);
                }
            });
        else
            for (int rowInd = 0; rowInd < rowCount; rowInd += 1)
                for (int colInd = 0; colInd < colCount; colInd += 1)
                {
                    var oldRGB = copyFrame.At<Vec3b>(rowInd, colInd);
                    var gray = (byte)((oldRGB.Item0 + oldRGB.Item1 + oldRGB.Item2) / 3);
                    var newRGB = new Vec3b(gray, gray, gray);
                    copyFrame.Set(rowInd, colInd, newRGB);
                }

        // get "edges" of frame
        var edges = new Mat();
        Cv2.Canny(copyFrame, edges, 100, 200);

        return edges;
    }
}

public partial class MiarVideo  // public
{
    public void DetectEdgesOnWebCam(bool useParallel)
    {
        // start video capture
        var capture = new VideoCapture(0);
        if (!capture.IsOpened()) throw new Exception("Self camera of PC cannot open.");
        Console.WriteLine("Gerçek Zamanlı Video İşleme Başladı!");
        Console.WriteLine("Çıkış yapmak için ESC tuşuna basınız");

        // read frames
        var frame = new Mat();
        var stopwatch = new Stopwatch();
        while (true)
        {
            // capture
            stopwatch.Restart();
            capture.Read(frame);
            if (frame.Empty()) break;

            // process
            var edges = DetectEdgesOnFrame(frame, useParallel);
            var fps = 1000.0 / stopwatch.ElapsedMilliseconds;
            Cv2.PutText(
                edges,
                $"FPS: {fps:F2}",
                new Point(0, 20),
                HersheyFonts.HersheyComplex,
                0.5,
                Scalar.White);

            // display
            Cv2.ImShow("Kameradaki Kenarlar", edges);
            stopwatch.Stop();

            // check "ESC" key whether pressed
            var keyCode = Cv2.WaitKey(1);
            if (keyCode == 27) break;
        }

        capture.Release();
        Cv2.DestroyAllWindows();
    }
}
