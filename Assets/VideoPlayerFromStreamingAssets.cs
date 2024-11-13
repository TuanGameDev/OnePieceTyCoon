using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerFromStreamingAssets : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string videoFileName = "IntroGacha.mp4";

    void Start()
    {
        string url;

#if UNITY_WEBGL && !UNITY_EDITOR
        // Với WebGL, sử dụng URL tương đối
        url = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
#elif UNITY_EDITOR || UNITY_STANDALONE

        url = "file:///" + System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
#endif
        videoPlayer.url = url;
        videoPlayer.Prepare();
        videoPlayer.Play();
    }
}
