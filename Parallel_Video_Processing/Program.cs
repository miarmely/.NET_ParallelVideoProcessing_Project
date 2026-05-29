namespace Parallel_Video_Processing;

public class Program1
{
    static void Main(string[] args)
    {
        var miarVideo = new MiarVideo();
        miarVideo.DetectEdgesOnWebCam(useParallel: true);
    }
}
