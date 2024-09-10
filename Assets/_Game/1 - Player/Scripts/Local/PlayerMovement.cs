using DesignPatterns;
using LocalPlayer;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    [Header("SOs")]
    [SerializeField] private InputReaderSO inputReader;
    [SerializeField] private PlayerStatsSO playerStats;
    [Header("Other")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private CharacterController characterController;

    public InputReaderSO InputReader => inputReader;
    public PlayerStatsSO PlayerStats => playerStats;
    public CharacterController CharacterController => characterController;
    public Vector3 Velocity { get; set; }
    public bool IsGrounded { get; private set; }
    public float FallingThreshold { get; } = -0.05f;
    
    //----- State Machine things -----
    public MovementIdleState MovementIdleState { get; private set; }
    public MovementWalkState MovementWalkState { get; private set; }
    public MovementJumpState MovementJumpState { get; private set; }
    public MovementFallingState MovementFallingState { get; private set; }

    private StateMachine<PlayerMovement> _stateMachine = new();
    //---------
    
    private void OnEnable()
    {
        inputReader.EnableInputActions();
    }

    private void OnDisable()
    {
        inputReader.DisableInputActions();
    }
    
    void Start()
    {
        MovementIdleState = new MovementIdleState(this, _stateMachine);
        MovementWalkState = new MovementWalkState(this, _stateMachine);
        MovementJumpState = new MovementJumpState(this, _stateMachine);
        MovementFallingState = new MovementFallingState(this, _stateMachine);

        _stateMachine.Initialize(MovementIdleState);
    }

    void Update()
    {
        _stateMachine.Update();
    }

    
    public void HandleMovement()
    {
        Vector2 direction = inputReader.Direction;
        Vector3 moveDirection = playerCamera.transform.right * direction.x + this.transform.forward * direction.y;
        
        Move(moveDirection);
    }
    
    public void Move(Vector3 direction)
    {
        float deltaTime = Time.deltaTime;
        float tickRate = 1f / Time.deltaTime;

        Vector3 previousPos = transform.position;
        Vector3 moveVelocity = Velocity;

        direction = direction.normalized;

        /*if (IsGrounded && moveVelocity.y < 0)
        {
            moveVelocity.y = 0f;
        }*/

        moveVelocity.y += playerStats.Gravity * deltaTime;

        Vector3 horizontalVel = new Vector3(moveVelocity.x, 0, moveVelocity.z);

        if (direction == Vector3.zero)
        {
            horizontalVel = Vector3.Lerp(horizontalVel, Vector3.zero, playerStats.Braking * deltaTime);
        }
        else
        {
            horizontalVel = Vector3.ClampMagnitude(horizontalVel + direction * playerStats.Acceleration * deltaTime,
                playerStats.MaxSpeed);
        }

        moveVelocity.x = horizontalVel.x;
        moveVelocity.z = horizontalVel.z;

        characterController.Move(moveVelocity * deltaTime);

        Velocity = (transform.position - previousPos) * tickRate;
        IsGrounded = characterController.isGrounded;
    }

}
