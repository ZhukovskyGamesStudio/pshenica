using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour {
    [SerializeField]
    private float _timeForDay;

    [SerializeField]
    private float _trajectoryHeightPercent;
    
    [SerializeField]
    private RectTransform _canvas;

    [SerializeField]
    private RectTransform _movingContainer;

    [SerializeField]
    private LightGradient _light;

    [SerializeField]
    private GameObject _sun, _moon;

    private Coroutine _movingCoroutine;

    private float MoveDistance => _canvas.rect.width * 1.15f;
    private float PassedPercent => Mathf.Clamp((_movingContainer.transform.localPosition.x + MoveDistance / 2) / (MoveDistance), 0, 1f);

    private Vector3 TargetPosition =>
        new Vector3(MoveDistance * PassedPercent - MoveDistance / 2, Screen.height *_trajectoryHeightPercent * Mathf.Sin(Mathf.PI * PassedPercent ));

    private bool _isDragging;
    private bool _isDay;

    public void StartMoving() {
        if (_movingCoroutine != null) {
            StopCoroutine(_movingCoroutine);
        }

        _movingCoroutine = StartCoroutine(MovingCoroutine());
    }

    private IEnumerator MovingCoroutine() {
        ChangeDayNight();
        _movingContainer.transform.localPosition = TargetPosition;
        while (true) {
            if (!_isDragging) {
                Vector3 position = _movingContainer.transform.localPosition;
                position += Vector3.right * (MoveDistance / _timeForDay) * Time.deltaTime;
                position = Vector3.Lerp(position, TargetPosition, 0.1f);
                _movingContainer.transform.localPosition = position;
            }

            if (PassedPercent >= 1) {
                ChangeDayNight();
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private void LateUpdate() {
        _light.UpdateColor((_isDay ? 0 : 0.5f) + PassedPercent / 2);
    }

    public void OnStartDrag() {
        _isDragging = true;
      
    }

    public void OnDrag() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        _movingContainer.position = mousePos;
    }

    public void OnEndDrag() {
        _isDragging = false;
    }

    private void ChangeDayNight() {
        _isDay = !_isDay;
        _sun.SetActive(_isDay);
        _moon.SetActive(!_isDay);
        _movingContainer.transform.localPosition = new Vector3(-MoveDistance / 2, Screen.height / 5f * Mathf.Sin(PassedPercent * 2));
    }
}