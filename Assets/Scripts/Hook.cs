using System;
using System.Collections;
using UnityEngine;

public class Hook : MonoBehaviour {
    [SerializeField]
    private Rigidbody2D _rb2d;

    private Vector3 _startPos;

    public Action OnHookCollectedHay;

    private bool _isFlying;

    private void Awake() {
        _startPos = _rb2d.transform.position;
    }

    public void Activate() {
        _rb2d.gameObject.SetActive(true);
    }

    public void TryStartMove() {
        if (_isFlying || !_rb2d.gameObject.activeInHierarchy) {
            return;
        }

        StartCoroutine(HookHay());
    }

    IEnumerator HookHay() {
        _isFlying = true;
        _rb2d.simulated = true;

        _rb2d.velocity = Vector2.zero;
        _rb2d.AddForce(Vector2.right * Game.MainGameConfig.HookSpeed, ForceMode2D.Impulse);
        SoundManager.Instance.PlaySound(Sounds.Hook);
        yield return new WaitForSeconds(1.5f);
        OnHookCollectedHay?.Invoke();
        yield return new WaitForSeconds(3f);
        _rb2d.simulated = false;
        _rb2d.transform.position = _startPos;
        _isFlying = false;
    }
}