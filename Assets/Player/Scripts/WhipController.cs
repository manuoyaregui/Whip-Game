using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipController : MonoBehaviour
{

    [SerializeField] private GameObject reference;
    [SerializeField] private GameObject whip;
    [SerializeField] private float whipThrowForceMultiplier;

    private bool isWhipButtonPressed;

    private bool isAWhipBeingCasted;

    private Vector2 direction;

    private void Start()
    {
        WhipMovement.OnWhipUncastedEvent += OnWhipUncastedEventHandler;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && !isWhipButtonPressed && !isAWhipBeingCasted)
        {
            isWhipButtonPressed = true;
        }
    }

    private void FixedUpdate()
    {
        if (isWhipButtonPressed)
        {
            DefineDirection();
            CastWhip();
            isWhipButtonPressed = false;
        }
    }

    private void DefineDirection()
    {
        direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion newRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        reference.transform.rotation = Quaternion.Lerp(reference.transform.rotation, newRotation, 1000);
    }

    private void CastWhip()
    {
        isAWhipBeingCasted = true;
        GameObject whipInstance = Instantiate(whip, reference.transform.position, reference.transform.rotation);
    }

    public float GetWhipForceMultiplier()
    {
        return whipThrowForceMultiplier;
    }

    public void OnWhipUncastedEventHandler()
    {
        isAWhipBeingCasted = false;
    }

    private void OnDestroy()
    {
        WhipMovement.OnWhipUncastedEvent -= OnWhipUncastedEventHandler;   
    }
}
