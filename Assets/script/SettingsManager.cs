using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("�����̴�")]
    public Slider VolumeSlider;
    public Slider sensitivitySlider;

    [Header("�����")]
    public AudioSource bgmAudioSource;

    [Header("ī�޶�")]
    public ThirdPersonCamera thirdPersonCamera;

    void Start()
    {
        // �ʱⰪ ���� (��: ���尪 �ҷ�����)
        VolumeSlider.value = bgmAudioSource.volume;
        sensitivitySlider.value = thirdPersonCamera.mouseSensitivity;

        // ������ ���
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