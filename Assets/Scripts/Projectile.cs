using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigidbody2D;
    // Start is called before the first frame update
    void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(transform.position.magnitude > 1000.0f)
        {
            Destroy(gameObject);
        }
    }
    // Update is called once per frame

    public void Launch(Vector2 direction, float force)
    {
        rigidbody2D.AddForce(direction * force); 
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        EnemyController e = other.collider.GetComponent<EnemyController>();
        if (e != null)
        {
            e.Fix();
        }
        HardEnemyController d = other.collider.GetComponent<HardEnemyController>();
        if (d != null)
        {
            d.Fix();
        }

        Destroy(gameObject);
    }
}
