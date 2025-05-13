using UnityEngine;

public class Button : MonoBehaviour
{
    public float newGravity = 10f;
    public float effectDuration = 10f;
    public Animator buttonAnimator;
    public string pressTrigger = "Button";
    public string resetTrigger = "ButtonReset";

    private float originalGravity;
    private bool isPressed = false;
    private float timer;
    private CharacterController astronaut;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            astronaut = player.GetComponent<CharacterController>();
        }

        if (astronaut != null)
        {
            originalGravity = astronaut.GetComponent<PlayerMovement>().gravity;
        }
    }

    void Update()
    {
        if (isPressed)
        {
            timer += Time.deltaTime;
            if (timer >= effectDuration)
            {
                ResetButton();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPressed && astronaut != null)
        {
            PressButton();
        }
    }

    void PressButton()
    {
        isPressed = true;
        timer = 0f;

        astronaut.GetComponent<PlayerMovement>().gravity = newGravity;

        if (buttonAnimator != null)
        {
            buttonAnimator.SetTrigger(pressTrigger);
        }
    }

    void ResetButton()
    {
        isPressed = false;

        if (astronaut != null)
        {
            astronaut.GetComponent<PlayerMovement>().gravity = originalGravity;
        }

        if (buttonAnimator != null)
        {
            buttonAnimator.SetTrigger(resetTrigger);
        }
    }
}