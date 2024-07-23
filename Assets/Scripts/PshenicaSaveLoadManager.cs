using UnityEngine;

public class PshenicaSaveLoadManager : SaveLoadManager<PshenicaSaveProfile> {
    [SerializeField]
    private MainGameConfig _mainGameConfig;

    protected override PshenicaSaveProfile CreateNewProfile() {
        PshenicaSaveProfile profile = new PshenicaSaveProfile();
        profile.HayUpgrade = _mainGameConfig.StartHayLvl - 1;
        profile.BookUpgrade = _mainGameConfig.StartBookLvl - 1;
        profile.ButtonsUpgrade = _mainGameConfig.StartButtonsLvl - 1;
        return profile;
    }
}