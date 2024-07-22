using System;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static SoundManager Instance;

    [SerializeField]
    private AudioSource _buttonSource, _upgradeSource, _growthSource, _collectSource, _writeProgressSource;

    private void Awake() {
        Instance = this;
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
    WriteProgress
}