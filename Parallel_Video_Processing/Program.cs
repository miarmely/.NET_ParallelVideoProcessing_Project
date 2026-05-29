namespace Parallel_Video_Processing;

public class Program
{
    static void Main(string[] args)
    {
        var miarVideo = new MiarVideo();
        miarVideo.DetectEdgesOnWebCam(useParallel: true);
    }
}
