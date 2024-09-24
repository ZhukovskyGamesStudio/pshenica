using System;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour {
    public static SoundManager Instance;

    [SerializeField]
    private AudioSource _buttonSource, _upgradeSource, _growthSource, _collectSource, _writeProgressSource, _hook;

    [SerializeField]
    private Slider _volumeSlider;

    private void Awake() {
        Instance = this;
#if UNITY_ANDROID
       _volumeSlider.gameObject.SetActive(false);
        UpdateVolume(1)
#endif
    }

    private void Start() {
        _volumeSlider.SetValueWithoutNotify(PshenicaSaveLoadManager.Profile.Volume);
        UpdateVolume(_volumeSlider.value);
    }

    public void UpdateVolume(float percent) {
        AudioListener.volume = percent;
        PshenicaSaveLoadManager.Profile.Volume = percent;
        PshenicaSaveLoadManager.Save();
    }

    public void PlaySound(Sounds type) {
        switch (type) {
            case Sounds.Button:
                _buttonSource.Play();
                break;
            case Sounds.Upgrade:
                _upgradeSource.Play();
                break;
            case Sounds.Growth:
                _growthSource.Play();
                break;
            case Sounds.Collect:
                _collectSource.Play();
                break;
            case Sounds.WriteProgress:
                _writeProgressSource.Play();
                break;
            case Sounds.Hook:
                _hook.Play();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}

[Serializable]
public enum Sounds {
    Button,
    Upgrade,
    Growth,
    Collect,
    WriteProgress,
    Hook
}