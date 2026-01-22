using UnityEngine;

public class PlayerMovment2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
        void Start()
    {
        // 获取 Rigidbody2D 组件
        rb = GetComponent<Rigidbody2D>();
        
        // 设置 Rigidbody 属性（可选）
        rb.gravityScale = 0; // 2D俯视游戏通常不需要重力
        rb.linearDamping = 10f; // 添加一些阻力，让停止更自然
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // 防止旋转
    }
        void Update()
    {
        // 获取输入（使用 -1, 0, 1 的值）
        movement.x = Input.GetAxisRaw("Horizontal"); // A/D 或 左右箭头
        movement.y = Input.GetAxisRaw("Vertical");   // W/S 或 上下箭头
        
        // 标准化向量，防止斜向移动更快
        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }
    }
        void FixedUpdate()
    {
        // 使用 Rigidbody2D 进行移动（在 FixedUpdate 中处理物理）
        MoveCharacter();
    }
        void MoveCharacter()
    {
        // 计算目标速度
        Vector2 targetVelocity = movement * moveSpeed;
        
        // 设置速度
        rb.linearVelocity = targetVelocity;
        
        // 或者使用力来移动（更平滑的加减速）
        // rb.AddForce(movement * moveSpeed);
        
        // 或者使用 MovePosition（需要关闭插值）
        // rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
