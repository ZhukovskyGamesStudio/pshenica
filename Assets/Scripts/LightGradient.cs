using UnityEngine;
using UnityEngine.UI;

public class LightGradient : MonoBehaviour {
    [SerializeField]
    private Image _lightImage;

    [SerializeField]
    private Gradient _dayGradient;

    public void UpdateColor(float percent) {
        _lightImage.color = _dayGradient.Evaluate(percent);
    }
}