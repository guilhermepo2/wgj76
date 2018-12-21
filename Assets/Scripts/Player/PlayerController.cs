using UnityEngine;
using System.Collections;


public class PlayerController : MonoBehaviour
{
    [Header("Movement Handling")]
    public float runSpeed = 8f;
    public float groundDamping = 20f; // how fast do we change direction? higher means faster

    [Header("Jumping Handling")]
	public float gravity = -25f;
    private float m_gravity;
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;
    /* Improving on Jump */
    public float jumpPressedRememberTime = 0.1f;
    public float groundedRememberTime = 0.1f;
    public float cutJumpHeight = 0.5f;
    private float m_jumpPressedRemember;
    private float m_groundedRemember;

    [Header("Sprite Child")]
    public Transform spriteChild;

	
    /* PRIVATE MEMBERS */
    [HideInInspector]
	private float m_normalizedHorizontalSpeed = 0;

	private Prime31.CharacterController2D m_controller;
	private RaycastHit2D m_lastControllerColliderHit;
	private Vector3 m_velocity;

    /* SCALE JUICING */
    private Vector2 m_originalScale;
    private Vector2 m_goingUpScaleMultiplier = new Vector2(0.8f, 1.2f);
    private Vector2 m_groundingScaleMultiplier = new Vector2(1.2f, 0.8f);

    public enum EPlayerState {
        Idle,
        Moving,
        Jumping,
        OnWall
    }


	void Awake()
	{
		m_controller = GetComponent<Prime31.CharacterController2D>();

		// listen to some events for illustration purposes
		m_controller.onControllerCollidedEvent += onControllerCollider;
		m_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		m_controller.onTriggerExitEvent += onTriggerExitEvent;

        /* Setting Up Gravity */
        m_gravity = gravity;
        m_originalScale = transform.localScale;
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

            if(!m_controller.collisionState.wasGroundedLastFrame) {
                StartCoroutine(ChangeScaleRoutine(transform.localScale * m_groundingScaleMultiplier));
            }
        }

		if( Input.GetKey( KeyCode.RightArrow ) )
		{
			m_normalizedHorizontalSpeed = 1;
			if( spriteChild.localScale.x < 0f )
				spriteChild.localScale = new Vector3( -spriteChild.localScale.x, spriteChild.localScale.y, spriteChild.localScale.z );
		}
		else if( Input.GetKey( KeyCode.LeftArrow ) )
		{
			m_normalizedHorizontalSpeed = -1;
			if( spriteChild.localScale.x > 0f )
				spriteChild.localScale = new Vector3( -spriteChild.localScale.x, spriteChild.localScale.y, spriteChild.localScale.z );
		}
		else
		{
			m_normalizedHorizontalSpeed = 0;
		}

        Jump();


		// apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
		var smoothedMovementFactor = m_controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		m_velocity.x = Mathf.Lerp( m_velocity.x, m_normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );

		// apply gravity before moving
		m_velocity.y += gravity * Time.deltaTime;

		// if holding down bump up our movement amount and turn off one way platform detection for a frame.
		// this lets us jump down through one way platforms
		if( m_controller.isGrounded && Input.GetKey( KeyCode.DownArrow ) )
		{
			m_velocity.y *= 3f;
			m_controller.ignoreOneWayPlatformsThisFrame = true;
		}

		m_controller.move( m_velocity * Time.deltaTime );

		// grab our current _velocity to use as a base for all calculations
		m_velocity = m_controller.velocity;
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

            StartCoroutine(ChangeScaleRoutine(spriteChild.localScale * m_goingUpScaleMultiplier));
        }
    }

    private IEnumerator ChangeScaleRoutine(Vector2 scale) {
        spriteChild.localScale = scale;
        yield return new WaitForSeconds(0.09f);
        spriteChild.localScale = new Vector2(Mathf.Sign(transform.localScale.x) * m_originalScale.x, m_originalScale.y);
    }

}