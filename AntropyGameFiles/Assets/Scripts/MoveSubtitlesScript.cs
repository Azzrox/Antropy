using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class MoveSubtitlesScript : MonoBehaviour
{
    public GameObject names;
    public GameObject roles;

    // Start is called before the first frame update
    void Start()
    {
        var texts = GameObject.FindGameObjectsWithTag("CreditsText");
        names = texts[0];
        roles = texts[1];
    }

    // Update is called once per frame
    void Update()
    {
        names.transform.position += new Vector3(0, 2, 0) * 8.0f * Time.deltaTime;
        roles.transform.position += new Vector3(0, 2, 0) * 8.0f * Time.deltaTime;

    }
}
