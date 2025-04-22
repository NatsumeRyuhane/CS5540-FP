using UnityEngine;

public class EnemyChasePlayer : MonoBehaviour
{
    [Header("追踪设置")]
    [Tooltip("敌人移动速度")]
    public float moveSpeed = 3.0f;
    
    [Tooltip("当距离玩家小于这个距离时停止移动")]
    public float minDistanceToPlayer = 1.5f;
    
    [Tooltip("加速度系数 - 值越高加速越快")]
    public float accelerationFactor = 2.0f;
    
    [Tooltip("最大速度倍数 - 用于限制最高速度")]
    public float maxSpeedMultiplier = 1.5f;
    
    [Header("引用")]
    [Tooltip("玩家的Transform组件，如果为空则自动查找标签为'Player'的对象")]
    public Transform playerTransform;
    
    // 当前速度
    private float currentSpeed = 0f;
    // 是否正在追逐
    private bool isChasing = true;
    // 最新的移动方向
    private Vector3 moveDirection;
    
    void Start()
    {
        // 如果没有指定玩家，自动查找标签为"Player"的游戏对象
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("找不到玩家对象，请确保玩家对象已标记为'Player'标签或手动分配playerTransform");
                isChasing = false;
            }
        }
    }
    
    void Update()
    {
        if (!isChasing || playerTransform == null)
            return;
            
        // 计算到玩家的距离（只考虑水平方向 X和Z）
        Vector3 playerPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
        Vector3 directionToPlayer = playerPosition - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        
        // 如果玩家距离太近，停止移动
        if (distanceToPlayer < minDistanceToPlayer)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * accelerationFactor);
            
            // 可选：让敌人面向玩家
            if (directionToPlayer != Vector3.zero)
            {
                transform.forward = directionToPlayer.normalized;
            }
            
            return;
        }
        
        // 计算移动方向（忽略Y轴）
        moveDirection = directionToPlayer.normalized;
        
        // 根据距离平滑地增加速度
        float targetSpeed = moveSpeed;
        // 可选：根据距离玩家的远近调整速度
        // float distanceFactor = Mathf.Clamp01(distanceToPlayer / 10f); // 10f是参考距离
        // targetSpeed = moveSpeed * (1f + distanceFactor * (maxSpeedMultiplier - 1f));
        
        // 平滑地调整当前速度
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * accelerationFactor);
        
        // 移动敌人
        transform.position += moveDirection * currentSpeed * Time.deltaTime;
        
        // 让敌人面向移动方向
        if (moveDirection != Vector3.zero)
        {
            transform.forward = moveDirection;
        }
    }
    
    // 公共方法：开始追逐
    public void StartChasing()
    {
        isChasing = true;
    }
    
    // 公共方法：停止追逐
    public void StopChasing()
    {
        isChasing = false;
        currentSpeed = 0f;
    }
    
    // 可选：在Scene视图中显示调试线条
    void OnDrawGizmosSelected()
    {
        if (playerTransform != null)
        {
            // 画一条到玩家的线
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, playerTransform.position);
            
            // 画出最小距离范围
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, minDistanceToPlayer);
        }
    }
}