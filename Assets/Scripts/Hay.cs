using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hay : MonoBehaviour {
    private Transform _flyTarget;

    public float forcePower = 10;
    public float distanceBorder = 10;

    [SerializeField]
    private Animation _animation;

    [SerializeField]
    private AnimationClip _growClip;

    [SerializeField]
    private Image _image;

    [SerializeField]
    private Sprite[] _haySprites;

    private bool _canBeCollected;
    public bool CanBeCollected => _canBeCollected;

    public void SetFlyTarget(Transform target) {
        _flyTarget = target;
    }
    
    public void Init(int upgradeState) {
        _image.sprite = _haySprites[upgradeState];
    }

    public void Grow() {
        gameObject.SetActive(true);
        _animation.Play(_growClip.name);
        StartCoroutine(Growing());
    }

    private IEnumerator Growing() {
        _canBeCollected = false;
        yield return new WaitWhile(() => _animation.isPlaying);
        _canBeCollected = true;
    }

    public void StartCollecting() {
        StartCoroutine(Collected());
    }

    private IEnumerator Collected() {
        _canBeCollected = false;
        GetComponent<EventTrigger>().enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.simulated = true;

        rb.AddTorque(Random.Range(30, 50));
        //rb.AddForce(Vector2.up * 15 + Vector2.right * 1);
        rb.AddForce(Vector2.up * 30);
        yield return new WaitForSeconds(0.3f);

        rb.velocity = Vector2.zero;
        Vector2 startDist = _flyTarget.position - transform.position;
        Vector2 dir = startDist;
        while (dir.magnitude > distanceBorder) {
            dir = _flyTarget.position - transform.position;
            rb.AddForce(dir * forcePower * Time.fixedDeltaTime, ForceMode2D.Force);
            transform.localScale = Vector2.one * dir.magnitude / startDist.magnitude;
            yield return new WaitForFixedUpdate();
        }

        Game.Instance.CollectXp();

        Destroy(gameObject);
    }
}