using UnityEngine;
using Random = UnityEngine.Random;

public class HayFactory : MonoBehaviour {
    [SerializeField]
    private Transform _minHaySpawnPos, _maxHaySpawnPos;

    [SerializeField]
    private Hay _hayScenePrefab;

    [SerializeField]
    private RectTransform _canvas;

    [SerializeField]
    private Transform _flyingHayTarget;

    public int HayAmount => _hayScenePrefab.transform.parent.childCount;

    public Hay GetHay() {
        Vector3 pos = Vector3.Lerp(_minHaySpawnPos.position, _maxHaySpawnPos.position, Random.Range(0, 1f));
        Hay h = Instantiate(_hayScenePrefab, pos, Quaternion.identity, _hayScenePrefab.transform.parent);
        Vector3 localPos = h.transform.localPosition;
        localPos.x = Random.Range(-1, 1f) * _canvas.rect.width * 0.45f;
        h.transform.localPosition = localPos;

        h.SetFlyTarget(_flyingHayTarget);
        return h;
    }
}