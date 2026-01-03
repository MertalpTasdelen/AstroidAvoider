using UnityEngine;

public class ControlLayoutApplier : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform joystickRoot;

    [Header("Anchor presets")]
    [Tooltip("Optional: an empty RectTransform positioned where the joystick should be for LEFT-handed layout.")]
    [SerializeField] private RectTransform leftAnchor;

    [Tooltip("Optional: an empty RectTransform positioned where the joystick should be for RIGHT-handed layout.")]
    [SerializeField] private RectTransform rightAnchor;

    public void Apply(GameSettings.ControlSide side)
    {
        if (joystickRoot == null)
        {
            return;
        }

        RectTransform anchor = side == GameSettings.ControlSide.Right ? rightAnchor : leftAnchor;
        if (anchor == null)
        {
            return;
        }

        joystickRoot.position = anchor.position;
        joystickRoot.rotation = anchor.rotation;
        joystickRoot.localScale = anchor.localScale;
    }
}
