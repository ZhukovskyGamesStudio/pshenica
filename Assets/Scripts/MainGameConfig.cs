using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObject/MainGameConfig", fileName = "MainGameConfig", order = 1)]
public class MainGameConfig : ScriptableObject {
    public int MaxHayOnScreen = 1000;
    public float HayGrowRadius = 0.375f;
    public int[] XpNeeded;
    public float HookSpeed = 4;
}
