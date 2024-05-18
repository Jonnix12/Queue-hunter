using System.Collections;
using Game.Queue;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovementController : MonoBehaviour
{
    #region MovementParams

    [SerializeField] float rotationSpeed = 360;
    [SerializeField] private Vector3 lastDir;
    [SerializeField] private MarbleShooter _shooter;

    [SerializeField, BoxGroup("Speed change after shooting")]
    private float newSpeed;

    [SerializeField, BoxGroup("Speed change after shooting")]
    private float speedChangeDurationInSeconds;

    Vector3 movementInput;
    Vector3 relative;
    bool isRunning;
    public bool canMove = true;
    public bool canRotate = true;

    public float Speed
    {
        get => _speed;
        set => _speed = value;
    }

    public UnityEvent onMovementSpeedChangeStart;
    public UnityEvent onMovementSpeedChangeEnd;
    public UnityEvent<string, bool> onMove;
    public UnityEvent<string, bool> onMoveEnd;
    public UnityEvent<string> onDash;

    #endregion

    #region DashParams

    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldown;
    private bool canDash = true;
    private bool isDashing;
    private Vector3 dashDirection;
    private float dashStartTime;

    #endregion

    //Animator anim;

    Rigidbody rb;
    [SerializeField] private float _speed;
    [SerializeField] private float gravityFactor;

    private void Start()
    {
        //anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        _shooter = GetComponentInChildren<MarbleShooter>();

        _shooter.onShootingMarbleAttempted?.AddListener((result) =>
        {
            if (result)
            {
                SetSpeedForDuration(/*newSpeed, speedChangeDurationInSeconds*/);
            }
        });
    }


    private void FixedUpdate()
    {
        movementInput = new Vector3(UnityEngine.Input.GetAxisRaw("Horizontal"), 0,
            UnityEngine.Input.GetAxisRaw("Vertical"));
        var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
        var skewedInput = matrix.MultiplyPoint3x4(movementInput);

        //rot detection
        if (skewedInput != Vector3.zero && canMove)
        {
            relative = GetRelativeRotation();
            UpdateRotation(relative, rotationSpeed);
        }

        //move detection
        if (skewedInput != Vector3.zero && canMove)
        {
            lastDir = skewedInput.normalized;
            Move();
        }
        else
        {
            rb.velocity = Vector3.zero;
            onMoveEnd?.Invoke("isRunning", false);
            //anim.SetBool("isRunning", false);
        }

        if (isDashing) DuringDash();
    }

    #region MoveFunctions

    public void SetSpeedForDuration(/*float newSpeed, float durationInSeconds*/)
    {
        float currSpeed = Speed;
        StartCoroutine(SetSpeedForDurationEnum(newSpeed, speedChangeDurationInSeconds));
        Speed = currSpeed;
    }

    IEnumerator SetSpeedForDurationEnum(float newSpeed, float durationInSeconds)
    {
        onMovementSpeedChangeStart?.Invoke();
        Speed = newSpeed;
        yield return new WaitForSeconds(durationInSeconds);
        onMovementSpeedChangeEnd?.Invoke();
    }

    void Move()
    {
        rb.velocity = new Vector3(lastDir.x * Time.fixedDeltaTime * (Speed * 100),
            rb.velocity.y,
            lastDir.z * Time.fixedDeltaTime * (Speed * 100));
        rb.AddForce(new Vector3(0, -1, 0) * gravityFactor, ForceMode.Acceleration);
        onMove?.Invoke("isRunning", true);
        //anim.SetBool("isRunning", true);
    }

    Vector3 GetRelativeRotation()
    {
        var relative = (transform.position + lastDir) - transform.position;
        return relative;
    }

    public void UpdateRotation(Vector3 relative, float rotSpeed)
    {
        if (relative != Vector3.zero && canRotate)
        {
            var rot = Quaternion.LookRotation(relative, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rotSpeed * Time.deltaTime);
        }
    }

    public void DisableMovement()
    {
        canMove = false;
    }

    public void EnableMovement()
    {
        canMove = true;
    }

    public void RotateTowardsAttack()
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.LookAt(new Vector3(pos.x, 1, pos.z));
    }

    #endregion

    #region DashFunctions

    public void StartDash()
    {
        if (!canDash) return;
        canDash = false;
        isDashing = true;

        //animation trigger
        onDash?.Invoke("DashTrigger");
        //anim.SetTrigger("DashTrigger");

        // Get the dash direction based on player input
        dashDirection = lastDir;

        // Record start time of dash
        dashStartTime = Time.fixedTime;
    }

    void DuringDash()
    {
        float dashTimeElapsed = Time.fixedTime - dashStartTime;
        if (dashTimeElapsed < dashDuration)
        {
            // Calculate progress of dash
            float t = dashTimeElapsed / dashDuration;
            //Rotate player towards dash direction
            relative = GetRelativeRotation();
            UpdateRotation(relative, rotationSpeed);
            //Actual dash logic
            rb.velocity = dashDirection * Time.fixedDeltaTime * (Speed * 400);
        }
        else
        {
            // End dash
            EndDash();
        }
    }

    void EndDash()
    {
        isDashing = false;
        StartCoroutine(ResetDash());
    }


    public IEnumerator ResetDash()
    {
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    #endregion
}