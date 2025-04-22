using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 具有空间感知的音乐播放器，能够模拟隔墙效果和距离衰减
/// </summary>
public class SpatialMusicPlayer : MonoBehaviour
{
    [Header("音频设置")]
    [Tooltip("要循环播放的音乐片段")]
    public AudioClip musicClip;
    
    [Tooltip("基础音量")]
    [Range(0f, 1f)]
    public float baseVolume = 0.7f;
    
    [Tooltip("是否自动开始播放")]
    public bool playOnStart = false;
    
    [Header("空间音频设置")]
    [Tooltip("空间混合度 (0=2D, 1=完全3D)")]
    [Range(0f, 1f)]
    public float spatialBlend = 1f;
    
    [Tooltip("声音最小距离 (在此距离内音量保持最大)")]
    public float minDistance = 5f;
    
    [Tooltip("声音最大距离 (超过此距离将听不到声音)")]
    public float maxDistance = 50f;
    
    [Tooltip("音量衰减曲线 (1=线性, 2=平方反比...)")]
    public float rolloffFactor = 1f;
    
    [Header("隔墙效果设置")]
    [Tooltip("是否启用隔墙效果")]
    public bool enableOcclusion = true;
    
    [Tooltip("射线检测频率 (每X秒检测一次)")]
    public float occlusionCheckInterval = 0.2f;
    
    [Tooltip("每个墙壁的音量衰减系数")]
    [Range(0f, 1f)]
    public float wallAttenuationFactor = 0.5f;
    
    [Tooltip("能够阻挡声音的图层")]
    public LayerMask wallLayers;
    
    [Tooltip("每个墙的最大音量衰减比例")]
    [Range(0f, 1f)]
    public float maxWallAttenuation = 0.2f;
    
    // 私有变量
    private AudioSource _audioSource;
    private Transform _listenerTransform;
    private int _wallsBetweenPlayerAndSource = 0;
    private float _currentOcclusionVolumeFactor = 1f;
    private float _targetVolume = 0f;
    
    private void Awake()
    {
        // 获取或添加AudioSource组件
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // 配置AudioSource基本设置
        _audioSource.clip = musicClip;
        _audioSource.loop = true;
        _audioSource.volume = baseVolume;
        _audioSource.playOnAwake = false;
        
        // 配置3D声音设置
        _audioSource.spatialBlend = spatialBlend;
        _audioSource.minDistance = minDistance;
        _audioSource.maxDistance = maxDistance;
        _audioSource.rolloffMode = AudioRolloffMode.Custom;
        
        // 设置音量衰减曲线
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0f, 1f);  // 最小距离时音量最大
        curve.AddKey(1f, 0f);  // 最大距离时音量为0
        
        for (float t = 0.1f; t < 0.9f; t += 0.1f)
        {
            float volume = Mathf.Pow(1f - t, rolloffFactor);
            curve.AddKey(t, volume);
        }
        
        _audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
        
        _targetVolume = baseVolume;
    }
    
    private void Start()
    {
        // 获取主音频监听器
        AudioListener listener = FindObjectOfType<AudioListener>();
        if (listener != null)
        {
            _listenerTransform = listener.transform;
        }
        else
        {
            // 如果没有找到音频监听器，通常在主摄像机上
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                _listenerTransform = mainCamera.transform;
            }
            else
            {
                Debug.LogWarning("未找到AudioListener或主摄像机，空间音频可能无法正常工作！");
            }
        }
        
        // 如果设置了自动播放，则开始播放
        if (playOnStart && musicClip != null)
        {
            PlayMusic();
        }
        
        // 开始墙壁检测
        if (enableOcclusion && _listenerTransform != null)
        {
            StartCoroutine(CheckOcclusion());
        }
    }
    
    private void Update()
    {
        // 应用墙壁衰减效果
        if (enableOcclusion)
        {
            _audioSource.volume = baseVolume * _currentOcclusionVolumeFactor;
        }
    }
    
    /// <summary>
    /// 检测播放器和监听器之间是否有墙壁阻隔
    /// </summary>
    private IEnumerator CheckOcclusion()
    {
        while (true)
        {
            if (_listenerTransform != null)
            {
                Vector3 directionToListener = (_listenerTransform.position - transform.position).normalized;
                float distanceToListener = Vector3.Distance(transform.position, _listenerTransform.position);
                
                // 使用射线检测墙壁
                RaycastHit[] hits = Physics.RaycastAll(
                    transform.position, 
                    directionToListener, 
                    distanceToListener, 
                    wallLayers
                );
                
                // 计算遇到的墙壁数量
                _wallsBetweenPlayerAndSource = hits.Length;
                
                // 计算衰减因子
                float attenuationFactor = 1f;
                
                for (int i = 0; i < _wallsBetweenPlayerAndSource; i++)
                {
                    attenuationFactor *= (1f - wallAttenuationFactor);
                }
                
                // 确保不会小于最大衰减值
                attenuationFactor = Mathf.Max(attenuationFactor, maxWallAttenuation);
                
                // 平滑过渡到新的衰减值
                _currentOcclusionVolumeFactor = Mathf.Lerp(
                    _currentOcclusionVolumeFactor, 
                    attenuationFactor, 
                    0.2f
                );
                
                // 可选：调试射线
                Debug.DrawRay(transform.position, directionToListener * distanceToListener, 
                    _wallsBetweenPlayerAndSource > 0 ? Color.red : Color.green, 
                    occlusionCheckInterval);
            }
            
            yield return new WaitForSeconds(occlusionCheckInterval);
        }
    }
    
    /// <summary>
    /// 开始播放音乐
    /// </summary>
    public void PlayMusic()
    {
        if (musicClip == null)
        {
            Debug.LogWarning("没有指定音乐片段！");
            return;
        }
        
        _audioSource.Play();
    }
    
    /// <summary>
    /// 停止播放音乐
    /// </summary>
    public void StopMusic()
    {
        _audioSource.Stop();
    }
    
    /// <summary>
    /// 设置音乐音量
    /// </summary>
    public void SetVolume(float newVolume)
    {
        baseVolume = Mathf.Clamp01(newVolume);
    }
    
    /// <summary>
    /// 获取当前墙壁数量（用于调试）
    /// </summary>
    public int GetWallCount()
    {
        return _wallsBetweenPlayerAndSource;
    }
    
    /// <summary>
    /// 绘制Debug信息
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // 绘制最小和最大距离范围
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, minDistance);
        
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}