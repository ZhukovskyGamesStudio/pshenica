using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour {
    public Game game;

    [SerializeField]
    private List<GameObject> _upgradeStates = new List<GameObject>();

    public void SetUpgradeState(int state) {
        for (int index = 0; index < _upgradeStates.Count; index++) {
            GameObject VARIABLE = _upgradeStates[index];
            VARIABLE.SetActive(index == state);
        }
    }
    
    private void Update() {
        if (GetComponent<RectTransform>().position.y < -15f / 19f) {
            game.GrewNewHay();
            Destroy(gameObject);
        }
    }
}