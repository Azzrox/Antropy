using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Camera Movement Script
/// </summary>
public class MapCameraScript : MonoBehaviour
{
  /// <summary>
  /// Movement vector of the camera
  /// </summary>
  private Vector3 movement;

  /// <summary>
  /// Speed the camera is moving
  /// </summary>
  public Vector3 speed = new Vector3(10, 10, 10);

  /// <summary>
  /// xMin camera clamp
  /// </summary>
  public float xMin;

  /// <summary>
  /// xMax camera clamp
  /// </summary>
  public float xMax;

  /// <summary>
  /// yMin camera clamp
  /// </summary>
  public float yMin;

  /// <summary>
  /// yMax camera clamp
  /// </summary>
  public float yMax;

  /// <summary>
  /// zMin camera clamp
  /// </summary>
  public float zMin;

  /// <summary>
  /// zMax camera clamp
  /// </summary>
  public float zMax;

  private void Start()
  {
    xMin = 209f;
    xMax = 257f;
    yMin = 93.77895f;
    yMax = 93.77895f;
    zMin = 0;
    zMax = 47f;
  }

  void Update()
  {
    float inputX = Input.GetAxis("Horizontal");
    float inputY = Input.GetAxis("Vertical");

    movement = new Vector3(
    speed.x * inputX,
    0f,
    speed.y * inputY);

    transform.Translate(movement * Time.deltaTime, Space.Self);
    transform.position = new Vector3(
    Mathf.Clamp(transform.position.x, xMin, xMax),
    Mathf.Clamp(transform.position.y, yMin, yMax),
    Mathf.Clamp(transform.position.z, zMin, zMax));
  }
 }
