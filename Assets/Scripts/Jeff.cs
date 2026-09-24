using UnityEngine;
using UnityEngine.InputSystem;

public class Jeff : MonoBehaviour
{
    [SerializeField] float moveSpeed = 15f;
    [SerializeField] float sneakSpeed = 7f;

    Rigidbody rb;
    Vector2 moveInput;
    bool isSneaking;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float currentSpeed = moveSpeed;

        if (isSneaking)
        {
            currentSpeed = sneakSpeed;
        }

        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);

        if (movement != Vector3.zero)
        {
            rb.AddForce(movement * currentSpeed, ForceMode.Acceleration);
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 v=ctx.ReadValue<Vector2>();
        moveInput = v;
    }

    public void OnSneak(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            isSneaking = true;
            Debug.Log("SNEAKING");
        }

        if (ctx.canceled)
        {
            isSneaking = false;
            Debug.Log("NOT SNEAKING");
        }
    }

    public bool IsSneaking()
    {
        return isSneaking;
    }
}
