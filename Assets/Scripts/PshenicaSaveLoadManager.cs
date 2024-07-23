using UnityEngine;

public class PshenicaSaveLoadManager : SaveLoadManager<PshenicaSaveProfile> {
    [SerializeField]
    private MainGameConfig _mainGameConfig;

    protected override PshenicaSaveProfile CreateNewProfile() {
        PshenicaSaveProfile profile = new PshenicaSaveProfile();
        profile.HayUpgrade = _mainGameConfig.StartHayLvl;
        profile.BookUpgrade = _mainGameConfig.StartBookLvl;
        profile.ButtonsUpgrade = _mainGameConfig.StartButtonsLvl;
        return profile;
    }
}