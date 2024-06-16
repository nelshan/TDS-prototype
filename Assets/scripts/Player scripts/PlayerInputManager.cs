using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public static Vector2 Movement;
    private InputAction moveAction;
    private PlayerInput playerInput;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
    }
    void Update()
    {
        Movement = moveAction.ReadValue<Vector2>();
    }
}