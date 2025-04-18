using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Generic Singleton base class for MonoBehaviours that need to follow the Singleton pattern.
/// Inherit from this class to create manager classes that are globally accessible.
/// Note that the singleton class is even independent of the scene, so it persists between scene changes.
/// </summary>
/// <typeparam name="T">The type of the singleton class</typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // Protected variable for the instance
    private static T _instance;
    
    [SerializeField] private bool dontDestroyOnLoad = false;
    
    // Public property to access the singleton instance
    public static T Instance
    {
        get
        {
            // If instance doesn't exist yet, try to find it in the scene
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>();
                
                // If it still doesn't exist, create a new GameObject with the component
                if (_instance == null)
                {
                    // Create at root level to ensure proper scene transitions
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();
                    Debug.Log($"[Singleton] An instance of {typeof(T)} was created at the root level.");
                }
                else
                {
                    // Check if the found instance should be DontDestroyOnLoad but isn't at root level
                    Singleton<T> singletonComponent = _instance as Singleton<T>;
                    if (singletonComponent != null && singletonComponent.dontDestroyOnLoad && 
                        singletonComponent.transform.parent != null)
                    {
                        // Move to root if it has a parent but needs to persist between scenes
                        Debug.Log($"[Singleton] Moving {typeof(T)} to root level for proper scene transition handling.");
                        singletonComponent.transform.SetParent(null);
                    }
                }
            }
            
            return _instance;
        }
    }
    
    /// <summary>
    /// Virtual Awake method that sets up the singleton.
    /// Override this in child classes but make sure to call base.Awake()
    /// </summary>
    protected virtual void Awake()
    {
        // Check if there's already an instance of this singleton
        if (_instance != null && _instance != this)
        {
            // Destroy this instance if another one already exists
            Debug.LogWarning($"[Singleton] Multiple instances of {typeof(T)} detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        
        // Assign this as the instance if none exists
        _instance = this as T;
        
        // If dontDestroyOnLoad is true, ensure we're at root level then make persistent
        if (dontDestroyOnLoad)
        {
            // Ensure the object is at the root level for proper DontDestroyOnLoad behavior
            if (transform.parent != null)
            {
                transform.SetParent(null);
                Debug.Log($"[Singleton] {typeof(T)} moved to scene root for proper persistence between scenes.");
            }
            DontDestroyOnLoad(gameObject);
        }
        
        OnAwake();
    }
    
    /// <summary>
    /// Override this method for initialization code instead of Awake
    /// This ensures the singleton setup happens before your custom initialization
    /// </summary>
    protected virtual void OnAwake() { }
    
    /// <summary>
    /// Virtual OnDestroy method that cleans up the singleton.
    /// Override this in child classes but make sure to call base.OnDestroy()
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
    
    /// <summary>
    /// Call this from any child class to force the singleton to be destroyed
    /// and reinitialize itself next time it's accessed.
    /// </summary>
    public void Reset()
    {
        _instance = null;
        Destroy(gameObject);
    }
}
