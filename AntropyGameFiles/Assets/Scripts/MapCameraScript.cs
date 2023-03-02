using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


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

  private float inputX;
  private float inputY;
  
  private void Start()
  {
    xMin = 208f;
    xMax = xMin + GameManager.Instance.columns;
    yMin = 93.77895f;
    yMax = 93.77895f;
    zMin = -2;
    zMax = zMin + GameManager.Instance.rows;

    transform.position = new Vector3(
    Mathf.Clamp(xMin + GameManager.Instance.anthillX, xMin, xMax),
    Mathf.Clamp(93, yMin, yMax),
    Mathf.Clamp(zMin + GameManager.Instance.anthillY, zMin, zMax));
  }

  void Update()
  {
    //inputX = Input.GetAxis("Horizontal");
    //inputY = Input.GetAxis("Vertical");

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
  
  private void OnMove(InputValue movementValue)
  { 
    //Debug.Log("moving");
    Vector2 movementVector = movementValue.Get<Vector2>();
    inputX = movementVector.x;
    inputY = movementVector.y;
  }

  private void OnOpenMenu()
  {
    SceneManager.LoadScene("Start");
    Debug.Log("Back to menu");
    GameObject backToMenu = GameObject.Find("BackToGame");
    Debug.Log("should have been found.");
    //backToMenu.GetComponent<Canvas>().enabled = true;
  }
 }
