using UnityEngine;
using System.Collections;


public class PlayerController : MonoBehaviour
{
    [Header("Movement Handling")]
    public float runSpeed = 8f;
    public float groundDamping = 20f; // how fast do we change direction? higher means faster

    [Header("Jumping Handling")]
	public float gravity = -25f;
    public float onWallGravity = -1f;
    private float m_gravity;
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;
    /* Improving on Jump */
    public float jumpPressedRememberTime = 0.1f;
    public float groundedRememberTime = 0.1f;
    public float cutJumpHeight = 0.5f;
    private float m_jumpPressedRemember;
    private float m_groundedRemember;
    
    // [Header("Wall Jumping Handling")]
    private Vector2 m_wallJumpForce;

    [Header("Sprite Child")]
    public Transform spriteChild;

	
    /* PRIVATE MEMBERS */
    [HideInInspector]
	private float m_normalizedHorizontalSpeed = 0;

	private Prime31.CharacterController2D m_controller;
    private Animator m_animator;
	private RaycastHit2D m_lastControllerColliderHit;
	private Vector3 m_velocity;

    /* SCALE JUICING */
    private Vector2 m_originalScale;
    private Vector2 m_goingUpScaleMultiplier = new Vector2(0.6f, 1.4f);
    private Vector2 m_groundingScaleMultiplier = new Vector2(1.4f, 0.6f);

    public enum EPlayerState {
        Idle,
        Moving,
        Jumping,
        OnWall
    }

    private EPlayerState m_currentState;


	void Awake()
	{
		m_controller = GetComponent<Prime31.CharacterController2D>();

		// listen to some events for illustration purposes
		m_controller.onControllerCollidedEvent += onControllerCollider;
		m_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		m_controller.onTriggerExitEvent += onTriggerExitEvent;

        /* Setting Up Gravity */
        m_gravity = gravity;
        m_originalScale = spriteChild.localScale;
        m_animator = spriteChild.GetComponent<Animator>();
        m_currentState = EPlayerState.Idle;
	}


	#region Event Listeners

	void onControllerCollider( RaycastHit2D hit )
	{
		// bail out on plain old ground hits cause they arent very interesting
		if( hit.normal.y == 1f )
			return;
	}


	void onTriggerEnterEvent( Collider2D col )
	{
		Debug.Log( "onTriggerEnterEvent: " + col.gameObject.name );
	}


	void onTriggerExitEvent( Collider2D col )
	{
		Debug.Log( "onTriggerExitEvent: " + col.gameObject.name );
	}

	#endregion


	// the Update loop contains a very simple example of moving the character around and controlling the animation
	void Update()
	{
		if( m_controller.isGrounded ) {
            m_groundedRemember = groundedRememberTime;
            m_velocity.y = 0;
            m_currentState = EPlayerState.Idle;
            m_gravity = gravity;

            if(!m_controller.collisionState.wasGroundedLastFrame) {
                StartCoroutine(ChangeScaleRoutine(spriteChild.localScale * m_groundingScaleMultiplier));
            }
        }

        ProcessState(m_currentState);
        AnimationHandling();

		// apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
		var smoothedMovementFactor = m_controller.isGrounded ? groundDamping : inAirDamping;
		m_velocity.x = Mathf.Lerp( m_velocity.x, m_normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );

		// apply gravity before moving
        m_velocity.y += gravity * Time.deltaTime;
        
        /* limiting gravity */
        // m_velocity.y = Mathf.Max(m_gravity, m_velocity.y + (m_gravity * Time.deltaTime + (.5f * m_gravity * (Time.deltaTime * Time.deltaTime))));

		m_controller.move( m_velocity * Time.deltaTime );
		m_velocity = m_controller.velocity;
	}

    private void ProcessState(EPlayerState state) {
        switch(state) {
            case EPlayerState.Idle:
            case EPlayerState.Moving:
                Move();
                Jump();
            break;
            case EPlayerState.Jumping:
                Move();
                // StickToWall();
                WallJump();
            break;
            case EPlayerState.OnWall:
                UnStickToWall();
                WallJump();
            break;
        }
    }

    private void Move() {
        float horizontalMovement = Input.GetAxis("Horizontal");
        m_normalizedHorizontalSpeed = horizontalMovement;

        if(horizontalMovement != 0) {
            spriteChild.localScale = new Vector3(Mathf.Sign(horizontalMovement) * Mathf.Abs(spriteChild.localScale.x), spriteChild.localScale.y, spriteChild.localScale.z);
        }
    }

    private void StickToWall() {
        float horizontalMovementValue = Input.GetAxis("Horizontal");

        if(horizontalMovementValue == 0) return;

        if(m_controller.IsColliding(Vector2.right) && Mathf.Sign(horizontalMovementValue) == 1 ||
        (m_controller.IsColliding(Vector2.left) && Mathf.Sign(horizontalMovementValue) == -1) ) {
            // Debug.Log("Player should stick to wall");
            m_velocity = Vector2.zero;
            m_gravity = onWallGravity;
            m_currentState = EPlayerState.OnWall;
        }
    }

    private void UnStickToWall() {
        float horizontalMovementValue = Input.GetAxis("Horizontal");

        if(horizontalMovementValue == 0) {
            m_gravity = gravity;
            m_currentState = EPlayerState.Jumping;
        }
    }

    private void WallJump() {
        bool isColliding = (m_controller.IsColliding(Vector2.right) || m_controller.IsColliding(Vector2.left));
        bool isJumping = Input.GetButtonDown("Jump");

        if(isColliding && isJumping) {
            m_gravity = gravity;

            m_velocity.y = Mathf.Sqrt(2f * jumpHeight * -m_gravity);
            m_velocity.x = -Mathf.Sign(spriteChild.localScale.x) * Mathf.Sqrt(75f * runSpeed);
            
            /* Updating Scale on Wall Jump */
            spriteChild.localScale = new Vector3(Mathf.Sign(m_velocity.x) * Mathf.Abs(spriteChild.localScale.x), spriteChild.localScale.y, spriteChild.localScale.z);

            m_currentState = EPlayerState.Jumping;
        }
    }

    private void Jump() {
        m_groundedRemember -= Time.deltaTime;
        m_jumpPressedRemember -= Time.deltaTime;

        if(Input.GetButtonDown("Jump")) {
            m_jumpPressedRemember = jumpPressedRememberTime;
        }

        if(Input.GetButtonUp("Jump")) {
            if(m_velocity.y > 0) {
                m_velocity.y = m_velocity.y * cutJumpHeight;
            }
        }

        /* Jumping */
        if((m_groundedRemember > 0) && (m_jumpPressedRemember > 0)) {
            m_jumpPressedRemember = 0;
            m_groundedRemember = 0;
            m_velocity.y = Mathf.Sqrt(2f * jumpHeight * -m_gravity);

            m_currentState = EPlayerState.Jumping;
            StartCoroutine(ChangeScaleRoutine(spriteChild.localScale * m_goingUpScaleMultiplier));
        }
    }

    private void AnimationHandling() {
        if(m_velocity.y > 0) {
            m_animator.Play("Jumping");
        } else if(m_velocity.y < 0) {
            m_animator.Play("Falling");
        } else if(m_normalizedHorizontalSpeed != 0) {
            m_animator.Play("Running");
        } else {
            m_animator.Play("Idle");
        }
    }

    private IEnumerator ChangeScaleRoutine(Vector2 scale) {
        spriteChild.localScale = scale;
        yield return new WaitForSeconds(0.09f);
        spriteChild.localScale = new Vector2(Mathf.Sign(spriteChild.localScale.x) * m_originalScale.x, m_originalScale.y);
    }

}