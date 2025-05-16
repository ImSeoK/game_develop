using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("슬라이더")]
    public Slider VolumeSlider;
    public Slider sensitivitySlider;

    [Header("오디오")]
    public AudioSource bgmAudioSource;

    [Header("카메라")]
    public ThirdPersonCamera thirdPersonCamera;

    void Start()
    {
        // 초기값 설정 (예: 저장값 불러오기)
        VolumeSlider.value = bgmAudioSource.volume;
        sensitivitySlider.value = thirdPersonCamera.mouseSensitivity;

        // 리스너 등록
        VolumeSlider.onValueChanged.AddListener(SetSoundVolume);
        sensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);
    }

    public void SetSoundVolume(float value)
    {
        bgmAudioSource.volume = value;
    }

    public void SetMouseSensitivity(float value)
    {
        thirdPersonCamera.mouseSensitivity = value;
    }
}