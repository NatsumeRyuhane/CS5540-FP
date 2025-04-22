using UnityEngine;
using System.Collections;

public class WallVerticalMovement : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("墙体向上移动的距离")]
    public float moveDistance = 5f;
    
    [Tooltip("单向移动的时间(秒)")]
    public float moveDuration = 2f;
    
    [Tooltip("在最高/最低点停留的时间(秒)")]
    public float waitTime = 1f;
    
    [Tooltip("是否自动循环播放动画")]
    public bool loopAnimation = true;
    
    [Tooltip("初始移动方向是否向上")]
    public bool startMovingUp = true;
    
    // 存储初始位置
    private Vector3 initialPosition;
    private Vector3 topPosition;
    
    // 动画是否正在播放
    private bool isMoving = false;
    
    void Start()
    {
        // 保存初始位置
        initialPosition = transform.position;
        // 计算顶部位置(向上移动)
        topPosition = initialPosition + new Vector3(0, moveDistance, 0);
        
        // 自动开始动画
        if (loopAnimation)
            StartMovement();
    }
    
    // 开始移动循环
    public void StartMovement()
    {
        if (!isMoving)
            StartCoroutine(MovementCycle());
    }
    
    IEnumerator MovementCycle()
    {
        isMoving = true;
        
        // 确定起始位置和方向
        Vector3 startPos = startMovingUp ? initialPosition : topPosition;
        Vector3 endPos = startMovingUp ? topPosition : initialPosition;
        
        while (true)
        {
            // 第一次移动(可能是向上或向下，取决于startMovingUp)
            yield return StartCoroutine(MoveWithEasing(startPos, endPos, moveDuration));
            
            // 在端点等待
            yield return new WaitForSeconds(waitTime);
            
            // 返回移动
            yield return StartCoroutine(MoveWithEasing(endPos, startPos, moveDuration));
            
            // 在另一端点等待
            yield return new WaitForSeconds(waitTime);
            
            // 如果不是循环，就退出
            if (!loopAnimation)
                break;
        }
        
        isMoving = false;
    }
    
    // 平滑移动协程
    IEnumerator MoveWithEasing(Vector3 startPos, Vector3 endPos, float duration)
    {
        float elapsedTime = 0;
        
        while (elapsedTime < duration)
        {
            // 计算已经过的时间比例
            float t = elapsedTime / duration;
            
            // 应用平滑插值 - 使用SmoothStep实现缓入缓出效果
            float smoothT = Mathf.SmoothStep(0, 1, t);
            
            // 更新位置
            transform.position = Vector3.Lerp(startPos, endPos, smoothT);
            
            // 增加时间
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // 确保准确到达终点
        transform.position = endPos;
    }
    
    // 手动触发移动的公共方法
    public void TriggerMove()
    {
        if (!isMoving)
            StartCoroutine(MovementCycle());
    }
    
    // 手动停止移动的公共方法
    public void StopMovement()
    {
        StopAllCoroutines();
        isMoving = false;
    }
    
    // 设置移动方向的公共方法
    public void SetMovementDirection(bool moveUp)
    {
        startMovingUp = moveUp;
    }
}