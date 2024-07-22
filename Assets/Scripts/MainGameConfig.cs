using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObject/MainGameConfig", fileName = "MainGameConfig", order = 1)]
public class MainGameConfig : ScriptableObject {
    public int MaxHayOnScreen = 1000;
    public int[] XpNeeded;
    public float HookSpeed = 5;
}
