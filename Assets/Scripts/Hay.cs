using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hay : MonoBehaviour {
    public Transform target;
    public Transform HayHolder;
    public Game Game;

    public float forcePower = 10;
    public float distanceBorder = 10;

    [SerializeField]
    private Animation _animation;

    [SerializeField]
    private AnimationClip _growClip;

    private bool _canBeCollected;
    public bool CanBeCollected => _canBeCollected;

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
        Hay h = Instantiate(gameObject, HayHolder).GetComponent<Hay>();
        h.StartCoroutine(h.Collected());
        gameObject.SetActive(false);
    }

    public IEnumerator Collected() {
        GetComponent<EventTrigger>().enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.simulated = true;

        rb.AddTorque(Random.Range(30, 50));
        //rb.AddForce(Vector2.up * 15 + Vector2.right * 1);
        rb.AddForce(Vector2.up * 30);
        yield return new WaitForSeconds(0.3f);

        rb.velocity = Vector2.zero;
        Vector2 startDist = target.position - transform.position;
        Vector2 dir = startDist;
        while (dir.magnitude > distanceBorder) {
            dir = target.position - transform.position;
            rb.AddForce(dir * forcePower * Time.fixedDeltaTime, ForceMode2D.Force);
            transform.localScale = Vector2.one * dir.magnitude / startDist.magnitude;
            yield return new WaitForFixedUpdate();
        }

        Game.CollectXP();

        Destroy(gameObject);
    }
}