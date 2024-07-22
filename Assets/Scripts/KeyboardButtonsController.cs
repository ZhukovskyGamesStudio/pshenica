using System;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardButtonsController : MonoBehaviour {
    public Button[] _buttons0;
    public Button[] _buttons1;
    public Button[] _buttons2;
    public Button[] _buttons3;
    public Button[] _buttons4;
    Button[] _curButtons;

    public Action OnLastButtonPressed;
    public Action<float> OnNextButtonPressed;
    private int _curButton;

    public void SetUpgradeLevel(int newLevel) {
        switch (newLevel) {
            case 0:
                for (int i = 0; i < _buttons0.Length; i++) {
                    _buttons0[i].gameObject.SetActive(true);
                }

                _curButtons = _buttons0;
                break;

            case 1:
                for (int i = 0; i < _buttons0.Length; i++) {
                    _buttons0[i].gameObject.SetActive(false);
                }

                for (int i = 0; i < _buttons1.Length; i++) {
                    _buttons1[i].gameObject.SetActive(true);
                }

                _curButtons = _buttons1;
                break;
            case 2:
                for (int i = 0; i < _buttons1.Length; i++) {
                    _buttons1[i].gameObject.SetActive(false);
                }

                for (int i = 0; i < _buttons2.Length; i++) {
                    _buttons2[i].gameObject.SetActive(true);
                }

                _curButtons = _buttons2;
                break;
            case 3:
                for (int i = 0; i < _buttons2.Length; i++) {
                    _buttons2[i].gameObject.SetActive(false);
                }

                for (int i = 0; i < _buttons3.Length; i++) {
                    _buttons3[i].gameObject.SetActive(true);
                }

                _curButtons = _buttons3;
                break;
            case 4:
                for (int i = 0; i < _buttons3.Length; i++) {
                    _buttons3[i].gameObject.SetActive(false);
                }

                for (int i = 0; i < _buttons4.Length; i++) {
                    _buttons4[i].gameObject.SetActive(true);
                }

                _curButtons = _buttons4;
                break;
        }
    }

    void SwithToNextButton() {
        for (int i = 0; i < _curButtons.Length; i++) {
            _curButtons[i].interactable = false;
            _curButtons[i].gameObject.GetComponent<Image>().raycastTarget = false;
        }

        _curButtons[_curButton].interactable = true;
        _curButtons[_curButton].gameObject.GetComponent<Image>().raycastTarget = true;
    }

    public void NextButtonPressed() {
        OnNextButtonPressed?.Invoke((_curButton + 0f) / _curButtons.Length);
        SoundManager.Instance.PlaySound(Sounds.Button);

        _curButton++;
        if (_curButton == _curButtons.Length) {
            _curButton = 0;
            OnLastButtonPressed?.Invoke();
        }

        SwithToNextButton();
    }
}