using UnityEngine;
using UnityEngine.UI;

public class KeyButton : MonoBehaviour {
    [SerializeField]
    private KeyCode _keyCode;

    private Button _button;

    protected void Awake() {
        _button = GetComponent<Button>();
    }

    private void Update() {
        if (!_button.interactable) {
            return;
        }

        if (Input.GetKeyDown(_keyCode)) {
            _button.onClick?.Invoke();
        }
    }
}