﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public delegate void OnEnemyAction(GameObject enemy);
    public static OnEnemyAction OnEnemyClicked;

    public float speed;
    public GameObject target;

    private Vector3 destination;
    private float distanceToStop;
    private Rigidbody rig;
    private GameManager gm;

    public int currentTarget;
    public int firstSpawnWaypoint;
    public int finalSpawnWaypoint;
    public int entryWaypoint;
    public TorqueLookRotation torque;
    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
        gm = GameManager.Get();
        currentTarget = firstSpawnWaypoint;
        torque = GetComponent<TorqueLookRotation>();
        torque.target = gm.waypoints[currentTarget].transform;
        //destination = new Vector3(target.transform.position.x,transform.position.y, target.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(gm.waypoints[currentTarget].transform.position, transform.position);

        if (distance <= 2.0f)
        {
            Debug.Log("eh messi");

            if (currentTarget == finalSpawnWaypoint)
            {
                currentTarget = entryWaypoint;
            }
            else
            {
                currentTarget++;
            }

            if (currentTarget >= gm.waypoints.Length)
            {
                currentTarget = 0;
            }

            torque.target = gm.waypoints[currentTarget].transform;
        }

        Vector3 direction = (gm.waypoints[currentTarget].transform.position - transform.position).normalized;
        rig.MovePosition(rig.position + direction * speed * Time.deltaTime);
    }

    private void OnMouseDown()
    {
        if(OnEnemyClicked != null)
        {
            OnEnemyClicked(gameObject);
        }
       //Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Fui movido por : " + collision.gameObject.tag);

        if (collision.gameObject.tag == "proyectile")
        {
            Debug.Log("Collision Test");
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }


        if (collision.gameObject.tag == "base")
        {
            Debug.Log("Collision Test Base");
            speed = 1;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "explosion")
        {
            Debug.Log("Collision Test EXPLOSION");
            Destroy(gameObject);
        }
    }
}
