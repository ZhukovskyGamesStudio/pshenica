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
        UpdateVolume(_volumeSlider.value);
    }

    public void UpdateVolume(float percent) {
        AudioListener.volume = percent;
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