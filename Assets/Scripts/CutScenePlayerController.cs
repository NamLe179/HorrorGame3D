using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using TMPro;


public class CutScenePlayerController : MonoBehaviour
{
    [Header("Video Player")]
    public VideoPlayer videoPlayer;

    [Header("Ending Panel")]
    public GameObject endingPanel;
    public TMP_Text textEndingType;
    public TMP_Text textDescription;

    [Header("Videos (assign in Inspector)")]
    public VideoClip badEndStabVideo;
    public VideoClip badEndVideo;
    public VideoClip goodEndVideo;
    public VideoClip trueEndVideo;

    [Header("Ending Texts")]
    public string badEndStabDesc = "Hơi non.";
    public string badEndDesc = "Gục ngã trước ngưỡng cửa thiên đường.";
    public string goodEndDesc = "Công lý sẽ được thực thi.";
    public string trueEndDesc = "Giờ phút của sự thật.";

    bool canContinue = false;

    void Start()
    {
        endingPanel.SetActive(false);

        // Chọn video theo ending ID
        switch (CutSceneLoader.endingID)
        {
            case 0:
                videoPlayer.clip = badEndStabVideo;
                textEndingType.text = "BAD END";
                textEndingType.color = Color.red;     // màu xanh dương
                textDescription.text = badEndStabDesc;
                break;

            case 1:
                videoPlayer.clip = badEndVideo;
                textEndingType.text = "BAD END";
                textEndingType.color = Color.red;
                textDescription.text = badEndDesc;
                break;

            case 2:
                videoPlayer.clip = goodEndVideo;
                textEndingType.text = "GOOD END";
                textEndingType.color = Color.cyan;
                textDescription.text = goodEndDesc;
                break;

            case 3:
                videoPlayer.clip = trueEndVideo;
                textEndingType.text = "TRUE END";
                textEndingType.color = Color.green;
                textDescription.text = trueEndDesc;
                break;
        }

        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.Play();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        endingPanel.SetActive(true);
        canContinue = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (canContinue && Input.anyKeyDown)
        {
            SceneManager.LoadScene("DemoMainMenu");
        }
    }
}
