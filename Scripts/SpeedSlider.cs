using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;    

/// <summary>
/// Simple "speed picker" game:
/// - A UI Slider moves up and down between minSpeed and maxSpeed.
/// - When the player presses Space, the slider stops and that value becomes
///   the movement speed for the player sprite.
/// - After that, the player moves continuously using the chosen speed.
/// </summary>
public class SpeedSlider : MonoBehaviour
{
    [Header("References")]
    public Slider speedSlider;      // UI slider that shows the speed range
    public Transform player;        // The sprite / player object to move
    
    // Optional: script that actually moves the circle using a Rigidbody2D
    // If not assigned, we'll try to find it on the player Transform.
    public CircleScript circleScript;

    [Header("Speed Settings")]
    public float minSpeed = 1f;     // Lowest possible speed
    public float maxSpeed = 10f;    // Highest possible speed
    public float sliderBounceSpeed = 1f; // How fast the slider value moves up/down

    private bool speedLocked = false;
    private float chosenSpeed = 0f;
    private int sliderDirection = 1; // 1 = going up, -1 = going down

    // Input System action (expects an action named "Jump" in your Input Actions asset)
    private InputAction jumpAction;

    [SerializeField] private TextMeshProUGUI sliderText = null;
    [SerializeField] private float maxSliderAmount = 100.0f;

    // Update the on-screen text whenever the slider's value changes.
    // "value" is the current speed represented by the slider.
    private void SliderChange(float value)
    {
        if (sliderText == null)
            return;

        // Normalize the slider's speed value into the 0..1 range.
        float normalizedSliderPosition = 0f;
        if (Mathf.Abs(maxSpeed - minSpeed) > Mathf.Epsilon)
        {
            // InverseLerp(min, max, value) answers: "How far between min and max is value?"
            // 0 means at minSpeed, 1 means at maxSpeed.
            normalizedSliderPosition = Mathf.InverseLerp(minSpeed, maxSpeed, value);
        }

        // Map that normalized 0..1 position into the 0..maxSliderAmount range
        // so you can show it as e.g. 0–100.0 on screen.
        float displayedSpeed = Mathf.Lerp(0f, maxSliderAmount, normalizedSliderPosition);
        sliderText.text = displayedSpeed.ToString("0.0");
    }
    private void Awake()
    {
        // Use the global InputSystem actions asset (same pattern as your other scripts)
        if (InputSystem.actions != null)
        {
            jumpAction = InputSystem.actions.FindAction("Jump");
            jumpAction?.Enable();
        }
    }

    private void Start()
    {
        if (speedSlider != null)
        {
            // Configure slider to match our speed range
            speedSlider.minValue = minSpeed;
            speedSlider.maxValue = maxSpeed;
            speedSlider.value = minSpeed;

            // Hook up the TMP text updater in code so you don't need to
            // use the Inspector's dynamic parameter dropdown.
            speedSlider.onValueChanged.AddListener(SliderChange);

            // Initialize the text with the starting value
            SliderChange(speedSlider.value);
        }

        // Auto-wire CircleScript if not set explicitly
        if (circleScript == null && player != null)
        {
            circleScript = player.GetComponent<CircleScript>();
        }
    }

    private void Update()
    {
        // Phase 1: Move the slider value up and down until Jump is pressed
        if (!speedLocked && speedSlider != null)
        {
            float value = speedSlider.value;

            // Move the slider value
            value += sliderDirection * sliderBounceSpeed * Time.deltaTime * (maxSpeed - minSpeed);

            // If we hit the ends, clamp and reverse direction (bounce effect)
            if (value >= maxSpeed)
            {
                value = maxSpeed;
                sliderDirection = -1;
            }
            else if (value <= minSpeed)
            {
                value = minSpeed;
                sliderDirection = 1;
            }

            speedSlider.value = value; // This will automatically invoke SliderChange via onValueChanged

            // Player performs Jump action (typically bound to Space) to lock in the current speed
            if (jumpAction != null && jumpAction.WasPressedThisFrame())
            {
                speedLocked = true;
                chosenSpeed = speedSlider.value;

                // Lock the slider so it can no longer be moved by the player
                speedSlider.interactable = false;

                // Inform the circle's movement script of the chosen speed
                if (circleScript != null)
                {
                    circleScript.StartMovement(chosenSpeed);
                }
            }
        }
    }
}