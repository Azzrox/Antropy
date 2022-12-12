using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class AntPathing : MonoBehaviour
{
    
    UnityEngine.Vector3 spawnpoint;
    UnityEngine.Vector3 waypoint;

    Rigidbody rg;

    public float turnspeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        spawnpoint = transform.position;
        waypoint = spawnpoint;
        rg = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (UnityEngine.Vector3.Distance(transform.position, waypoint) < 0.1f)
        {
            waypoint = spawnpoint + new UnityEngine.Vector3(Random.value-0.5f,0,Random.value-0.5f);
        }

        UnityEngine.Quaternion direction = UnityEngine.Quaternion.LookRotation(transform.position - waypoint, UnityEngine.Vector3.up);
        rg.MoveRotation(Quaternion.Lerp(transform.rotation, direction, Time.fixedDeltaTime*turnspeed));
    }
}
