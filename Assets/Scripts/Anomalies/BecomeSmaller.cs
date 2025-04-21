using UnityEngine;

public class ImperceptibleShrink : MonoBehaviour
{
    [Tooltip("每秒缩小的比例（值越小，缩小速度越慢）")]
    [Range(0.0001f, 0.01f)]
    public float shrinkRate = 0.01f;
    
    [Tooltip("最小缩放值，达到后停止缩小")]
    public float minScale = 0.01f;
    
    private Vector3 originalScale;
    
    void Start()
    {
        // 记录初始缩放值
        originalScale = transform.localScale;
    }
    
    void Update()
    {
        // 检查当前缩放是否已达到最小值
        if (transform.localScale.x > minScale)
        {
            // 使用线性插值进行非常缓慢的缩小
            transform.localScale = Vector3.Lerp(
                transform.localScale, 
                Vector3.zero, 
                shrinkRate * Time.deltaTime
            );
        }
    }
    
    // 重置对象到原始大小
    public void ResetScale()
    {
        transform.localScale = originalScale;
    }
}