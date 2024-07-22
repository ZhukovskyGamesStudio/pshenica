using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnimatedBackground : MonoBehaviour {
    
    [SerializeField]
    private Transform _cloudSpawnPoint;
    [SerializeField]
    private List<MovingCloud> _cloudPrefabs;


    [SerializeField]
    private float _timeBetweenClouds = 10;
    [SerializeField]
    private float _minCloudSpeed = 0.2f, _maxCloudSpeed = 0.5f;

    [SerializeField]
    private Transform _cloudsHolder;

    [SerializeField]
    private DayNightCycle _dayNightCycle;

    private void Start() {
        InitClouds();
        _dayNightCycle.StartMoving();
    }

    private void InitClouds() {
        StartCoroutine(MoveClounds());
    }

    private IEnumerator MoveClounds() {
        float untilNextCloud = 0;
        while (true) {
            if (untilNextCloud <= 0) {
                untilNextCloud = _timeBetweenClouds;
                MovingCloud cloud = Instantiate(_cloudPrefabs[Random.Range(0, _cloudPrefabs.Count)],
                    _cloudSpawnPoint.position + Vector3.up * Random.Range(-0.25f, 0.5f), Quaternion.identity, _cloudsHolder);

                cloud.StartMoving(Random.Range(_minCloudSpeed, _maxCloudSpeed));
                if (Random.Range(0, 5) != 0) {
                    cloud.transform.SetAsFirstSibling();
                }
            }

            untilNextCloud -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}