using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 MousePosition { get; private set; }
    public bool ActiveSkillEnabled { get; private set; }

    private PlayerInput input;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        input = new PlayerInput();
    }

    private void OnEnable()
    {
        input.Enable();
        input.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += ctx => MoveInput = Vector2.zero;
        input.Player.Mouse.performed += ctx => MousePosition = ctx.ReadValue<Vector2>();

        input.Player.ActiveSkill.performed += ctx => ActiveSkillEnabled = true;
    }

    private void LateUpdate()
    {
        ActiveSkillEnabled = false;
    }

    private void OnDisable()
    {
        input.Disable();
    }
}
