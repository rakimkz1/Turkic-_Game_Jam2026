using System;
using UnityEngine;

public class SoulBall : MonoBehaviour
{
    public event Action OnHitCap;
    public event Action OnHitBoiler;

    public event Action OnEscape;
    public event Action OnBoiled;
    public Vector3 moveDirection;

    public float escapeLine;
    public bool isWorking;
    private float currentHealth;

    private float maxHealth;
    private float movementSpeed;
    private float startMovementSpeed;
    private float accelerationPerHit;
    private float damagePerHit;
    private Rigidbody2D _rb;
    public void Init(float startMovementSpeed, float accelerationPerHit, float damagePerHit, float maxHealth)
    {
        this.startMovementSpeed = startMovementSpeed;
        this.accelerationPerHit = accelerationPerHit;
        this.damagePerHit = damagePerHit;
        movementSpeed = startMovementSpeed;
        currentHealth = maxHealth;
        this.maxHealth = maxHealth;
        moveDirection = Vector3.down;
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision");
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
        Debug.Log("TakeDamage");
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
        if (transform.localPosition.y >= escapeLine && isWorking)
        {
            isWorking = false;
            OnEscape?.Invoke();
            gameObject.SetActive(false);
        }
    }
}