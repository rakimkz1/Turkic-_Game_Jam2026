using System;
using UnityEngine;

public class SoulBall : MonoBehaviour
{
    public event Action OnHitCap;
    public event Action OnHitBoiler;

    public event Action OnEscape;
    public Vector3 moveDirection;

    public float maxHealth;
    public float escapeLine;
    private float currentHealth;

    private float movementSpeed;
    private float startMovementSpeed;
    private float accelerationPerHit;
    private float damagePerHit;
    private Rigidbody _rb;
    public void Init(float startMovementSpeed, float accelerationPerHit, float damagePerHit)
    {
        this.startMovementSpeed = startMovementSpeed;
        this.accelerationPerHit = accelerationPerHit;
        this.damagePerHit = damagePerHit;
        movementSpeed = startMovementSpeed;
        currentHealth = maxHealth;
        moveDirection = Vector3.down;
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider != null && collision.collider.tag == "Cap")
        {
            OnHitCap?.Invoke();
            Ricashet(collision.contacts[0].normal);
        }
        if (collision.collider != null && collision.collider.tag == "Boiler")
        {
            OnHitBoiler?.Invoke();
            TakeDamage();
            Ricashet(collision.contacts[0].normal);
        }
    }

    private void TakeDamage()
    {
        currentHealth -= damagePerHit;
        if(currentHealth <= 0)
        {
            OnEscape?.Invoke();
            gameObject.SetActive(false);
        }
         movementSpeed += accelerationPerHit;
    }

    private void Ricashet(Vector3 normal)
    {
        moveDirection = Vector3.Reflect(moveDirection, normal).normalized;
    }

    private void Update()
    {
        _rb.linearVelocity = moveDirection * movementSpeed;
        if (transform.position.y <= escapeLine)
        {
            OnEscape?.Invoke();
            gameObject.SetActive(false);
        }
    }
}