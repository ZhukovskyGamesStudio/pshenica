using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MultikeyButton : MonoBehaviour {
    [SerializeField]
    private List<KeyCode> _keyCodes = new List<KeyCode>();

    private Button _button;

    protected void Awake() {
        _button = GetComponent<Button>();
    }

    private void Update() {
        if (!_button.interactable) {
            return;
        }

        if (_keyCodes.Any(Input.GetKeyDown)) {
            _button.onClick?.Invoke();
        }
    }
}