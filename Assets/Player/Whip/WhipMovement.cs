using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WhipMovement : MonoBehaviour
{
    private float whipForceMultiplier = 800; // You get it from the WhipController attached to the player, because the force is apllied from the player
    [SerializeField] private float pullForceMultiplier = 80; // 
    [SerializeField] private float maxWhipDistance = 12; // max distance that the whip can make
    [SerializeField] private float minWhipDistance = 1.5f; // Min distance betw 2 objects b4 the whip breaks
    [SerializeField] private float whipTimeAliveInAir = 3;

    private float timeCounter;

    Rigidbody2D whipRb;

    private GameObject player;
    private Rigidbody2D playerRB;
    private Collider2D whipCollider;



    bool forceApplied; //flag to make sure that only apllies force to the whip one time;
    private bool isPulling; // State that defines if its grabbed to an object or if its still flying
    private float distance; // Distance done since the beggining on the wh?p
    Vector3 forceDirection;

    //Manage Collisions
    GameObject collisionObject;
    Rigidbody2D collisionRb;
    ContactPoint2D contactPoint;

    //When The whip breaks
    public static event Action OnWhipUncastedEvent;


    //Line renderer / rope
    private LineRenderer rope;
    private int indexLineRenderer = 1;

    // Start is called before the first frame update
    void Start()
    {
        //Get all the components
        player = GameObject.FindGameObjectWithTag("Player");
        playerRB = player.GetComponent<Rigidbody2D>();
        whipForceMultiplier = player.GetComponent<WhipController>().GetWhipForceMultiplier();
        whipRb = GetComponent<Rigidbody2D>();
        rope = GetComponentInChildren<LineRenderer>();
        whipCollider = GetComponent<CircleCollider2D>();

        //Events
        PlayerCollisionManager.OnPlayerCollidesWithEnemyEvent += BreakTheWhip;
    }

    private void Update()
    {
        ModifyLineRendererVertices();
        CheckIfPlayerWannaBreakTheWhip();
        
    }

    

    private void FixedUpdate()
    {
        CalcDistanceBetwPlayerAndWhipTip();
        

        if( ! isPulling)
        {
            ThrowTheWhip();
        }
        else
        {
            PullAction();
        }

    }

    private void CalcDistanceBetwPlayerAndWhipTip()
    {
        distance = (transform.position - player.transform.position).magnitude;


        if (distance >= maxWhipDistance)
        {
            Destroy(gameObject);
        }
    }

    private void ThrowTheWhip()
    {
        if(timeCounter < whipTimeAliveInAir)
        {
            timeCounter += Time.fixedDeltaTime;
        }
        else
        {
            BreakTheWhip();
        }


        if (!forceApplied)
        {
            forceApplied = true;
            whipRb.AddForce(transform.right * whipForceMultiplier);
        }
    }

    private void PullAction()
    {
        if (collisionObject.CompareTag("Enemy"))
        {
            transform.position = collisionObject.transform.position;
        }
        else if (collisionObject.CompareTag("WrappableObject"))
        {
            transform.position = contactPoint.point;
        }

        if (distance >= minWhipDistance)
        {
            forceDirection = (transform.position - player.transform.position).normalized;
            playerRB.AddForce(forceDirection * pullForceMultiplier);
            if (collisionRb != null)
                collisionRb.AddForce(-1 * pullForceMultiplier * forceDirection);

        }
        else  Destroy(gameObject);
    }

    private void ModifyLineRendererVertices()
    {
        rope.SetPosition(0, player.transform.position);
        rope.SetPosition(1, transform.position);
    }

    private void CheckIfPlayerWannaBreakTheWhip()
    {
        if (isPulling && Input.GetButtonDown("Fire1"))
        {
            BreakTheWhip();
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        collisionObject = collision.gameObject;
        contactPoint = collision.GetContact(0);
        if(collisionObject.CompareTag("Enemy") || collisionObject.CompareTag("WrappableObject"))
        {
            isPulling = true;

            CalmDownTheWhip();

            collisionRb = collisionObject.GetComponent<Rigidbody2D>();

            
        }
        else
        {
            BreakTheWhip();
        }
    }

    private void CalmDownTheWhip()
    {
        whipRb.velocity = Vector2.zero;
        whipRb.angularVelocity = 0;
        whipRb.gravityScale = 0;
        whipCollider.enabled = false;
    }
    private void BreakTheWhip()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        PlayerCollisionManager.OnPlayerCollidesWithEnemyEvent -= BreakTheWhip;
        OnWhipUncastedEvent?.Invoke();
    }
}
