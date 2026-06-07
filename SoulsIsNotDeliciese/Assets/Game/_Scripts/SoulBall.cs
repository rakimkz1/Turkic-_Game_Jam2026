using System;
using UnityEngine;

public class SoulBall : MonoBehaviour
{
    public event Action OnHitCap;
    public event Action OnHitBoiler;

    public event Action OnEscape;
    public event Action OnBoiled;
    public Func<bool> isMinistalAllowEscape;
    public Vector3 moveDirection;

    public float escapeLine;
    public bool isWorking;
    public SoundPackage capHitSound;
    public SoundPackage metalHitSound;
    public SoundPackage afterSteamSound;
    private float currentHealth;

    private float maxHealth;
    private float movementSpeed;
    private float maxMovementSpeed;
    private float startMovementSpeed;
    private float accelerationPerHit;
    private float damagePerHit;
    private Vector3 initalPos;
    private Rigidbody2D _rb;
    public void Init(float startMovementSpeed, float accelerationPerHit, float damagePerHit, float maxHealth, float maxSpeed)
    {
        this.startMovementSpeed = startMovementSpeed;
        this.accelerationPerHit = accelerationPerHit;
        this.damagePerHit = damagePerHit;
        movementSpeed = startMovementSpeed;
        currentHealth = maxHealth;
        this.maxHealth = maxHealth;
        maxMovementSpeed = maxSpeed;

        moveDirection = Vector3.down;
        transform.position = initalPos;
    }

    private void Start()
    {
        initalPos = transform.position;
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision");
        if (collision.collider != null && collision.collider.tag == "Cap")
        {
            OnHitCap?.Invoke();
            HitCap();
            Ricashet(collision.contacts[0].normal);
        }
        if (collision.collider != null && collision.collider.tag == "Boiler")
        {
            OnHitBoiler?.Invoke();
            TakeDamage();
            Ricashet(collision.contacts[0].normal);
        }
    }

    private void HitCap()
    {
        if(BoilerManager.Instance.damageFromCap == 0) return;
        currentHealth -= BoilerManager.Instance.damageFromCap;
        DemonKvotaManager.instance.AddKvota(BoilerManager.Instance.damageFromCap);
        AudioManager.instance.PlayOneShot(capHitSound);
        if (currentHealth <= 0)
        {
            Debug.Log("Boiled");
            OnBoiled?.Invoke();
            gameObject.SetActive(false);
        }
        movementSpeed += accelerationPerHit * 0.3f;
    }

    private void TakeDamage()
    {
        Debug.Log("TakeDamage");
        AudioManager.instance.PlayOneShot(metalHitSound);
        AudioManager.instance.PlayOneShotDelay(afterSteamSound, 0.1f);
        currentHealth -= damagePerHit;
        DemonKvotaManager.instance.AddKvota(damagePerHit);
        if (currentHealth <= 0)
        {
            Debug.Log("Boiled");
            OnBoiled?.Invoke();
            gameObject.SetActive(false);
        }
         movementSpeed += accelerationPerHit;
    }

    private void Ricashet(Vector3 normal)
    {
        Debug.Log("ricashet");
        moveDirection = Vector3.Reflect(moveDirection, normal).normalized;
    }

    private void Update()
    {
        _rb.linearVelocity = moveDirection * movementSpeed;
        _rb.linearVelocity = Vector3.ClampMagnitude(_rb.linearVelocity, maxMovementSpeed);
        if (transform.localPosition.y >= escapeLine && isWorking)
        {
            if(isMinistalAllowEscape != null && !isMinistalAllowEscape.Invoke())
            {
                ReturnToBoiler();
                return;
            }
            isWorking = false;
            OnEscape?.Invoke();
            gameObject.SetActive(false);
        }
    }

    private void ReturnToBoiler()
    {
        transform.position = initalPos;
        moveDirection = -moveDirection.normalized;
    }

    public void SlowDownBall(float slowAmount)
    {
        movementSpeed *= slowAmount;
    }
}