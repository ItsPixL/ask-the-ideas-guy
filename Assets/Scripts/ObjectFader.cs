using UnityEngine;

public class ObjectFader : MonoBehaviour {
    public float fadeSpeed = 5.0f; // Speed of the fade effect
    public float fadeAmount = 0.5f; // Amount to fade the object (0.0 to 1.0)
    float originalOpacity;
    Material[] Mats;
    public bool doFade = false;
    void Start() {
        Mats = GetComponent<Renderer>().materials; // Get the material of the object
        if (Mats == null) {
            Debug.LogError("Material not found on the object. Please assign a material to the object or ensure it has a Renderer component.");
            return;
        }
        foreach(Material mat in Mats) {
            originalOpacity = mat.color.a; // Store the original opacity of the material
        }
    }

    void Update() {
        if (doFade) {
            FadeNow();
        }
        else {
            ResetFade();
        }
    }

    void FadeNow() { // this is fading the object out so that we can still see the player
        foreach(Material mat in Mats) {
        Color currentColour = mat.color;
        Color smoothColour = new Color(currentColour.r, currentColour.g, currentColour.b, Mathf.Lerp(currentColour.a, fadeAmount, fadeSpeed * Time.deltaTime));
        mat.color = smoothColour;
        }
    }
    void ResetFade() { // this is fading the object back to its original amount when the player has left
        foreach(Material mat in Mats) {
        Color currentColour = mat.color;
        Color smoothColour = new Color(currentColour.r, currentColour.g, currentColour.b, Mathf.Lerp(currentColour.a, originalOpacity, fadeSpeed * Time.deltaTime));
        mat.color = smoothColour;
        }
    }
}
