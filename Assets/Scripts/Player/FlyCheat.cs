using UnityEngine;

public class FlyCheat : MonoBehaviour
{
    private bool isFlying = false;
    private Rigidbody rb;

    public float flySpeed = 70f;
    public float verticalSpeed = 40f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ToggleFly();
        }

        if (isFlying)
        {
            FlyMovement();
        }
    }

    void ToggleFly()
    {
        isFlying = !isFlying;
        rb.useGravity = !isFlying;
        rb.velocity = Vector3.zero;  // 기존 속도 초기화
    }

    void FlyMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = (transform.right * moveX + transform.forward * moveZ).normalized;

        // 기본 이동
        rb.MovePosition(transform.position + moveDir * flySpeed * Time.deltaTime);

        // 상승 (Space), 하강 (C)
        if (Input.GetKey(KeyCode.Space))
        {
            rb.MovePosition(transform.position + Vector3.up * verticalSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.C))
        {
            rb.MovePosition(transform.position + Vector3.down * verticalSpeed * Time.deltaTime);
        }
    }
}

