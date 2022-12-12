using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class AntPathing : MonoBehaviour
{
    
    public UnityEngine.Vector3 spawnpoint;
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
        //ant reached his waypoint, give it a new one
        if (Mathf.Abs(transform.position.x - waypoint.x) < 0.1f &&
            Mathf.Abs(transform.position.y - waypoint.y) < 0.1f)
        {
            waypoint = spawnpoint + new UnityEngine.Vector3(Random.value-0.25f,0,Random.value-0.25f);
        }

        //the ant got lost!
        if (Mathf.Abs(transform.position.x - spawnpoint.x) > 1f ||
            Mathf.Abs(transform.position.y - spawnpoint.y) > 1f ||
            Mathf.Abs(transform.position.z - spawnpoint.z) > 1f) { rg.position = spawnpoint; }

        //rotate the ant a bit towards the waypoint
        UnityEngine.Quaternion direction = UnityEngine.Quaternion.LookRotation(transform.position - waypoint, UnityEngine.Vector3.up);
        rg.MoveRotation(Quaternion.Lerp(transform.rotation, direction, Time.fixedDeltaTime*turnspeed));

        //help the ant back on it's feet!
        if (Mathf.Max(Mathf.Abs(transform.rotation.eulerAngles.x), Mathf.Abs(transform.rotation.eulerAngles.z)) >= 80f) { rg.MoveRotation(Quaternion.LookRotation(transform.forward, Vector3.up)); }
    }
}
