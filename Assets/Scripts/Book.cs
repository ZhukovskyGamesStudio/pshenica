using UnityEngine;

public class Book : MonoBehaviour {
    public Game game;

    private void Update() {
        if (GetComponent<RectTransform>().position.y < -15f / 19f) {
            game.GrewNewHay();
            Destroy(gameObject);
        }
    }
}