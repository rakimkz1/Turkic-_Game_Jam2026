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
    public Transform capPosition;
    public float centerAttraction;
    public float maxReflectionTurnAngle;

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
    private HitStop hitStop;
    private bool isColider;
    private float isColideDuraction = 0.3f;
    public void Init(float startMovementSpeed, float accelerationPerHit, float damagePerHit, float maxHealth, float maxSpeed)
    {
        initalPos = transform.position;
        this.startMovementSpeed = startMovementSpeed;
        this.accelerationPerHit = accelerationPerHit;
        this.damagePerHit = damagePerHit;
        movementSpeed = startMovementSpeed;
        currentHealth = maxHealth;
        this.maxHealth = maxHealth;
        maxMovementSpeed = maxSpeed;

        moveDirection = Vector3.down;
    }

    private void Start()
    {
        hitStop = GetComponent<HitStop>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider != null && collision.collider.tag == "Cap")
        {
            OnHitCap?.Invoke();
            HitCap();
            Ricashet(collision.contacts[0].normal);
            isColider = true;
        }
        if (collision.collider != null && collision.collider.tag == "Boiler")
        {
            OnHitBoiler?.Invoke();
            TakeDamage();
            Ricashet(collision.contacts[0].normal);
            isColider = true;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if(collision.collider != null && collision.collider.tag == "Boiler" && isColider)
        {
            moveDirection = (capPosition.position - transform.position).normalized;
        }
        isColideDuraction -= Time.deltaTime;
        if (isColideDuraction < 0)
        {
            moveDirection = (capPosition.position - transform.position).normalized;
            isColideDuraction = 0.2f;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        isColider = false;
    }

    private void HitCap()
    {
        if(BoilerManager.Instance.damageFromCap == 0) return;
        currentHealth -= BoilerManager.Instance.damageFromCap;
        hitStop.Trigger(_rb.linearVelocity * 0.8f);
        DemonKvotaManager.instance.AddKvota(BoilerManager.Instance.damageFromCap);
        AudioManager.instance.PlayOneShot(capHitSound);
        if (currentHealth <= 0)
        {
            Debug.Log("Boiled");
            OnBoiled?.Invoke();
            hitStop.Cancel();
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
        hitStop.Trigger(_rb.linearVelocity);
        
        if (currentHealth <= 0)
        {
            Debug.Log("Boiled");
            hitStop.Cancel();
            OnBoiled?.Invoke();
            gameObject.SetActive(false);
        }
         movementSpeed += accelerationPerHit;
    }

    private void Ricashet(Vector3 normal)
    {
        Vector3 incoming = moveDirection.normalized;
        Vector3 reflected = Vector3.Reflect(incoming, normal).normalized;
        float turnAngle = Vector3.Angle(incoming, reflected);
        isColideDuraction = 0.2f;

        Vector3 capCenter = Vector3.zero;
        bool haveCenter = false;
        if (BoilerManager.Instance != null && BoilerManager.Instance.caps != null)
        {
            capCenter = BoilerManager.Instance.caps.transform.position;
            haveCenter = true;
        }

        if (!haveCenter)
        {
            if (turnAngle <= maxReflectionTurnAngle)
            {
                moveDirection = reflected;
                return;
            }
            Vector3 cross1 = Vector3.Cross(incoming, reflected);
            float sign1 = Mathf.Sign(cross1.z);
            if (Mathf.Approximately(sign1, 0f))
            {
                sign1 = Mathf.Sign(Vector3.Dot(incoming, new Vector3(-normal.y, normal.x, 0f)));
                if (Mathf.Approximately(sign1, 0f)) sign1 = 1f;
            }
            Quaternion q1 = Quaternion.AngleAxis(maxReflectionTurnAngle * sign1, Vector3.forward);
            moveDirection = (q1 * incoming).normalized;
            return;
        }

        Vector3 aimDir = (capCenter - transform.position).normalized;

        float angleFactor = Mathf.InverseLerp(0f, 180f, turnAngle); 
        float attraction = Mathf.Clamp01(centerAttraction * angleFactor);
        Vector3 biased = Vector3.Slerp(reflected, aimDir, attraction).normalized;
        float desiredAngle = Vector3.Angle(incoming, biased);
        float limitedAngle = Mathf.Min(desiredAngle, maxReflectionTurnAngle);

        Vector3 cross = Vector3.Cross(incoming, biased);
        float sign = Mathf.Sign(cross.z);
        if (Mathf.Approximately(sign, 0f))
        {
            sign = Mathf.Sign(Vector3.Dot(incoming, new Vector3(-normal.y, normal.x, 0f)));
            if (Mathf.Approximately(sign, 0f)) sign = 1f;
        }

        Quaternion q = Quaternion.AngleAxis(limitedAngle * sign, Vector3.forward);
        moveDirection = (q * incoming).normalized;
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
            hitStop.Cancel();
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
    private void OnDisable()
    {
        transform.position = initalPos;
    }
}