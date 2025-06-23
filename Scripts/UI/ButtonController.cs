using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public OpeningSceneController sceneController;

    private SpriteRenderer sr;
    private Color originalColor;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        originalColor.a = 1f;
    }

    private void OnMouseEnter()
    {
        Color darkened = new Color(
            originalColor.r * 0.5f,
            originalColor.g * 0.5f,
            originalColor.b * 0.5f,
            originalColor.a
        );

        sr.color = darkened;
    }

    private void OnMouseExit()
    {
        sr.color = originalColor;
    }

    private void OnMouseDown()
    {
        sceneController.OnStartButtonClicked();
    }
}
