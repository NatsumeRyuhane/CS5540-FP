using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TargetBehavior : InteractableObject
{
    public AudioClip completedSound;
    public Color completedColor;
    [HideInInspector] public bool Completed { get; private set; }
    private LevelManager _levelManager;
    private AudioSource _audioSource;

    private bool _isInitialActive;
    
    public void Start()
    {
        Completed = false;
        _levelManager = FindFirstObjectByType<LevelManager>();
        _levelManager.RegisterTarget(this);
        _audioSource = GetComponent<AudioSource>();
        _isInitialActive = gameObject.activeSelf;
    }
    
    public override void Interact()
    {
        StartCoroutine(base.DoLongHold(3));
    }
    
    protected override void InteractEffect()
    {
        Completed = true;
        GetComponent<Renderer>().material.color = completedColor;
        _levelManager.CheckLevelCompleteCondition();
        StartCoroutine(DoPostCompleteAnim(3f));
    }
    
    private IEnumerator DoPostCompleteAnim(float duration)
    {
        float time = 0;
        Renderer renderer = GetComponent<Renderer>();
        Color initialColor = renderer.material.color;
        _audioSource.PlayOneShot(completedSound);

        while (time < duration)
        {
            // Spin the object
            transform.Rotate(Vector3.up, 10);

            // Gradually fade out
            float alpha = Mathf.Lerp(1, 0, time / duration);
            renderer.material.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);

            time += Time.deltaTime;
            yield return null;
        }

        // Ensure the object is fully transparent at the end
        renderer.material.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0);
        gameObject.SetActive(false); // Optionally deactivate the object
    }
    
    public void Reset()
    {
        Completed = false;
        GetComponent<Renderer>().material.color = Color.white;
        gameObject.SetActive(_isInitialActive);
    }
}
