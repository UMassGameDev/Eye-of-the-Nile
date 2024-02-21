using UnityEngine;
using UnityEngine.UIElements;

public class InteractProgressBar : MonoBehaviour
{
    public GameObject progressbarUnderside;
    public GameObject progressbar;
    public RectTransform progressbarTransform;

    public float xLength = 200f; // this is where the progressbar's x should be "pinned" to
    public float minDisplayProgress = 0.1f; // from 0 to 1

    void OnEnable()
    {
        PlayerInteract.interactProgress += UpdateProgressBar;
    }

    void OnDisable()
    {
        PlayerInteract.interactProgress -= UpdateProgressBar;
    }

    void Awake()
    {
        progressbar.SetActive(false);
        progressbarUnderside.SetActive(false);
    }

    void UpdateProgressBar(float progress)
    {
        if (Input.GetKey(KeyCode.E))
            Debug.Log(progress);
            
        if (progress >= (1 - minDisplayProgress) || progress <= 0 || progress >= 1) {
            progressbar.SetActive(false);
            progressbarUnderside.SetActive(false);
        } else {
            progressbar.SetActive(true);
            progressbarUnderside.SetActive(true);

            progressbarTransform.sizeDelta = new Vector2(xLength - (progress * xLength), progressbarTransform.sizeDelta.y);

            // The math can definitely be simplier here, but I'm pressed for time. It works so it's fine!
            progressbarTransform.anchoredPosition = new Vector2(-xLength/2 + (xLength/2 - (xLength/2 * progress)), progressbarTransform.anchoredPosition.y);
        }
    }
}
