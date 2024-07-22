using System.Collections;
using UnityEngine;

public class MovingCloud : MonoBehaviour {
    private Coroutine _movingCoroutine;
    private Transform _transform;

    private void Awake() {
        _transform = transform;
    }

    public void StartMoving(float speed) {
        if (_movingCoroutine != null) {
            StopCoroutine(_movingCoroutine);
        }
        _movingCoroutine = StartCoroutine(MoveCoroutine(speed));
    }

    private IEnumerator MoveCoroutine(float speed) {
        while (_transform.position.x < Screen.width) {
            _transform.position += Vector3.right * (speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
}