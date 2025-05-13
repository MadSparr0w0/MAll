using UnityEngine;
using System.Collections.Generic;

public class MultiTargetButton : MonoBehaviour
{
    public Animator buttonAnimator;
    public string pressTrigger = "Button";
    public string resetTrigger = "ButtonReset";
    public float effectDuration = 3f;

    public List<GameObject> targetObjects = new List<GameObject>();

    private bool useFadeEffect = true;
    public float fadeDuration = 1f;

    private bool isPressed = false;
    private float effectTimer;
    private List<FadeData> fadeDataList = new List<FadeData>();

    private class FadeData
    {
        public Renderer renderer;
        public Material material;
        public Color originalColor;
    }

    void Start()
    {
        InitializeTargets();
    }

    void InitializeTargets()
    {
        fadeDataList.Clear();

        foreach (GameObject target in targetObjects)
        {
            if (target != null)
            {
                Renderer renderer = target.GetComponent<Renderer>();
                if (renderer != null)
                {
                    FadeData data = new FadeData
                    {
                        renderer = renderer,
                        material = new Material(renderer.material),
                        originalColor = renderer.material.color
                    };

                    renderer.material = data.material;
                    fadeDataList.Add(data);
                }
            }
        }
    }

    void Update()
    {
        if (isPressed)
        {
            effectTimer += Time.deltaTime;

            if (useFadeEffect)
            {
                float progress = Mathf.Clamp01(effectTimer / fadeDuration);
                foreach (FadeData data in fadeDataList)
                {
                    if (data.material != null)
                    {
                        Color newColor = data.originalColor;
                        newColor.a = Mathf.Lerp(1f, 0f, progress);
                        data.material.color = newColor;
                    }
                }
            }

            if (effectTimer >= effectDuration)
            {
                CompleteEffect();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPressed)
        {
            PressButton();
        }
    }

    void PressButton()
    {
        isPressed = true;
        effectTimer = 0f;

        if (buttonAnimator != null)
        {
            buttonAnimator.SetTrigger(pressTrigger);
        }
    }

    void CompleteEffect()
    {
        foreach (GameObject target in targetObjects)
        {
            if (target != null)
            {
                target.SetActive(false);
            }
        }

        isPressed = false;
    }

    public void ResetButton()
    {
        if (buttonAnimator != null)
        {
            buttonAnimator.SetTrigger(resetTrigger);
        }

        foreach (GameObject target in targetObjects)
        {
            if (target != null)
            {
                target.SetActive(true);
            }
        }

        foreach (FadeData data in fadeDataList)
        {
            if (data.material != null)
            {
                data.material.color = data.originalColor;
            }
        }

        isPressed = false;
        effectTimer = 0f;
    }

    public void AddTarget(GameObject newTarget)
    {
        if (!targetObjects.Contains(newTarget))
        {
            targetObjects.Add(newTarget);
            InitializeTargets();
        }
    }
}