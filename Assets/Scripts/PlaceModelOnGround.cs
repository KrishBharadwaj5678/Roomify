// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.XR.ARFoundation;
// using UnityEngine.XR.ARSubsystems;
// using UnityEngine.UI;

// public class PlaceModelOnGround : MonoBehaviour
// {
//     public GameObject[] modelPrefabs; // Array of model prefabs
//     private GameObject placedModel;   // The currently placed model
//     private int currentModelIndex = 0; // Track the currently selected model index
//     private ARRaycastManager arRaycastManager; // AR Raycast Manager for detecting surfaces
//     private ARPlaneManager arPlaneManager; // AR Plane Manager for enabling/disabling plane detection
//     private List<ARRaycastHit> hits = new List<ARRaycastHit>(); // List to store raycast results
//     private List<GameObject> placedModels = new List<GameObject>(); // List to track placed models

//     // Joystick references
//     public FixedJoystick joystick;

//     [Header("Rotation Settings")]
//     public float rotationSpeed = 50f; // Speed of rotation (adjustable in Inspector)

//     [Header("Scale Settings")]
//     public float[] modelScaleFactors; // Array to store scale factors for each model
//     public float scaleSpeed = 0.1f; // Speed at which the scale changes (adjustable in Inspector)
//     private Vector3 initialScale; // Store the initial scale of the model
//     private Vector3 targetScale; // Target scale for the model

//     private Vector3 velocity = Vector3.zero; // Velocity used for smooth scaling
//     private float smoothTime = 0.2f; // Smoothing time for scale changes

//     [Header("Model Rotation Adjustments")]
//     public Vector3[] modelRotations; // Rotation values for each model in the array (can be adjusted in Inspector)

//     // Array to specify the rotation axis for each model
//     // 0 = X-axis, 1 = Y-axis, 2 = Z-axis
//     public int[] rotationAxes; // Array specifying the rotation axis for each model

//     [Header("Audio Settings")]
//     public AudioSource audioSource; // Assign the AudioSource component in the Inspector
//     public Button playButton; // Assign the Play button in the Inspector
//     public Button pauseButton; // Assign the Pause button in the Inspector

//     void Start()
//     {
//         arRaycastManager = FindObjectOfType<ARRaycastManager>();
//         arPlaneManager = FindObjectOfType<ARPlaneManager>(); // Get ARPlaneManager

//         // Ensure buttons have their listeners assigned
//         if (playButton != null)
//         {
//             playButton.onClick.AddListener(PlayAudio);
//         }
//         if (pauseButton != null)
//         {
//             pauseButton.onClick.AddListener(PauseAudio);
//         }

//         // Update button visibility initially
//         UpdateButtonVisibility();
//     }

//     void Update()
//     {
//         HandlePlacement();
//         HandleJoystickControl();
//     }

//     private void HandlePlacement()
//     {
//         if (Input.touchCount > 0 && placedModel == null) // Allow placement only if no model exists
//         {
//             Touch touch = Input.GetTouch(0);

//             if (touch.phase == TouchPhase.Began)
//             {
//                 if (arRaycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
//                 {
//                     Pose hitPose = hits[0].pose; // Get the pose of the hit
//                     PlaceModel(hitPose.position, hitPose.rotation);
//                 }
//             }
//         }
//     }

//     private void HandleJoystickControl()
//     {
//         if (placedModel == null || joystick == null)
//             return;

//         RotateModelWithJoystick();

//         // Scale up or down using the vertical joystick movement
//         float vertical = joystick.Vertical;
//         if (Mathf.Abs(vertical) > 0.1f) // Threshold to avoid small unintentional movements
//         {
//             float scaleFactor = modelScaleFactors[currentModelIndex]; // Use the specific scale factor for the current model
//             targetScale = initialScale * (1 + scaleFactor * vertical * scaleSpeed);

//             placedModel.transform.localScale = Vector3.SmoothDamp(placedModel.transform.localScale, targetScale, ref velocity, smoothTime);
//         }
//     }

//     private void RotateModelWithJoystick()
//     {
//         if (placedModel == null) return;

//         int rotationAxis = rotationAxes[currentModelIndex];
//         float joystickInput = joystick.Horizontal;

//         if (Mathf.Abs(joystickInput) > 0.1f)
//         {
//             if (rotationAxis == 0) // X-axis
//             {
//                 placedModel.transform.Rotate(Vector3.right, joystickInput * rotationSpeed * Time.deltaTime);
//             }
//             else if (rotationAxis == 1) // Y-axis
//             {
//                 placedModel.transform.Rotate(Vector3.up, joystickInput * rotationSpeed * Time.deltaTime);
//             }
//             else if (rotationAxis == 2) // Z-axis
//             {
//                 placedModel.transform.Rotate(Vector3.forward, joystickInput * rotationSpeed * Time.deltaTime);
//             }
//         }
//     }

//     private void PlaceModel(Vector3 position, Quaternion rotation)
//     {
//         if (placedModel != null)
//         {
//             Destroy(placedModel);
//         }

//         Vector3 modelRotation = modelRotations[currentModelIndex];
//         placedModel = Instantiate(modelPrefabs[currentModelIndex], position, Quaternion.Euler(modelRotation));
//         placedModels.Add(placedModel);

//         initialScale = placedModel.transform.localScale;
//         targetScale = initialScale; // Set initial target scale to the current model scale

//         // Stop plane detection when model is placed
//         if (arPlaneManager != null)
//         {
//             arPlaneManager.enabled = false; // Disable ARPlaneManager (stop plane detection)
//         }
//     }

//     public void PlayAudio()
//     {
//         if (audioSource != null && !audioSource.isPlaying)
//         {
//             audioSource.Play();
//             UpdateButtonVisibility();
//         }
//     }

//     public void PauseAudio()
//     {
//         if (audioSource != null && audioSource.isPlaying)
//         {
//             audioSource.Pause();
//             UpdateButtonVisibility();
//         }
//     }

//     private void UpdateButtonVisibility()
//     {
//         if (audioSource != null)
//         {
//             bool isPlaying = audioSource.isPlaying;

//             if (playButton != null) playButton.gameObject.SetActive(!isPlaying);
//             if (pauseButton != null) pauseButton.gameObject.SetActive(isPlaying);
//         }
//     }

//     public void LoadNextModel()
//     {
//         if (modelPrefabs.Length == 0) return;

//         currentModelIndex = (currentModelIndex + 1) % modelPrefabs.Length;
//         ReplaceCurrentModel();
//     }

//     public void LoadPreviousModel()
//     {
//         if (modelPrefabs.Length == 0) return;

//         currentModelIndex = (currentModelIndex - 1 + modelPrefabs.Length) % modelPrefabs.Length;
//         ReplaceCurrentModel();
//     }

//     private void ReplaceCurrentModel()
//     {
//         if (placedModel != null)
//         {
//             Vector3 currentPosition = placedModel.transform.position;
//             Quaternion currentRotation = placedModel.transform.rotation;

//             Destroy(placedModel);
//             placedModel = Instantiate(modelPrefabs[currentModelIndex], currentPosition, Quaternion.Euler(modelRotations[currentModelIndex]));
//             placedModels.Add(placedModel);

//             initialScale = placedModel.transform.localScale;
//             targetScale = initialScale;

//             placedModel.transform.position = currentPosition;
//             placedModel.transform.rotation = Quaternion.Euler(modelRotations[currentModelIndex]);
//         }
//     }

//     public void ResetPlacedModels()
//     {
//         foreach (GameObject model in placedModels)
//         {
//             if (model != null)
//             {
//                 Destroy(model);
//             }
//         }

//         placedModels.Clear();

//         // Reset plane detection when all models are removed
//         if (arPlaneManager != null)
//         {
//             arPlaneManager.enabled = true; // Re-enable ARPlaneManager (start plane detection)
//         }
//     }
// }



// ------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

public class PlaceModelOnGround : MonoBehaviour
{
    public GameObject[] modelPrefabs; // Array of model prefabs
    private GameObject placedModel;   // The currently placed model
    private int currentModelIndex = 0; // Track the currently selected model index
    private ARRaycastManager arRaycastManager; // AR Raycast Manager for detecting surfaces
    private ARPlaneManager arPlaneManager; // AR Plane Manager for enabling/disabling plane detection
    private List<ARRaycastHit> hits = new List<ARRaycastHit>(); // List to store raycast results
    private List<GameObject> placedModels = new List<GameObject>(); // List to track placed models

    // Joystick references
    public FixedJoystick joystick;

    [Header("Rotation Settings")]
    public float rotationSpeed = 50f; // Speed of rotation (adjustable in Inspector)

    [Header("Scale Settings")]
    public float[] modelScaleFactors; // Array to store scale factors for each model
    public float scaleSpeed = 0.1f; // Speed at which the scale changes (adjustable in Inspector)
    private Vector3 initialScale; // Store the initial scale of the model
    private Vector3 targetScale; // Target scale for the model

    private Vector3 velocity = Vector3.zero; // Velocity used for smooth scaling
    private float smoothTime = 0.2f; // Smoothing time for scale changes

    [Header("Model Rotation Adjustments")]
    public Vector3[] modelRotations; // Rotation values for each model in the array (can be adjusted in Inspector)

    // Array to specify the rotation axis for each model
    // 0 = X-axis, 1 = Y-axis, 2 = Z-axis
    public int[] rotationAxes; // Array specifying the rotation axis for each model

    [Header("Audio Settings")]
    public AudioSource audioSource; // Assign the AudioSource component in the Inspector
    public Button playButton; // Assign the Play button in the Inspector
    public Button pauseButton; // Assign the Pause button in the Inspector

    void Start()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();
        arPlaneManager = FindObjectOfType<ARPlaneManager>(); // Get ARPlaneManager

        // Ensure buttons have their listeners assigned
        if (playButton != null)
        {
            playButton.onClick.AddListener(PlayAudio);
        }
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(PauseAudio);
        }

        // Update button visibility initially
        UpdateButtonVisibility();
    }

    void Update()
    {
        HandlePlacement();
        HandleJoystickControl();
    }

    private void HandlePlacement()
    {
        if (Input.touchCount > 0 && placedModel == null) // Allow placement only if no model exists
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (arRaycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose; // Get the pose of the hit
                    PlaceModel(hitPose.position, hitPose.rotation);
                }
            }
        }
    }

    private void HandleJoystickControl()
    {
        if (placedModel == null || joystick == null)
            return;

        RotateModelWithJoystick();

        // Scale up or down using the vertical joystick movement
        float vertical = joystick.Vertical;
        if (Mathf.Abs(vertical) > 0.1f) // Threshold to avoid small unintentional movements
        {
            float scaleFactor = modelScaleFactors[currentModelIndex]; // Use the specific scale factor for the current model
            targetScale = initialScale * (1 + scaleFactor * vertical * scaleSpeed);

            placedModel.transform.localScale = Vector3.SmoothDamp(placedModel.transform.localScale, targetScale, ref velocity, smoothTime);
        }
    }

    private void RotateModelWithJoystick()
    {
        if (placedModel == null) return;

        int rotationAxis = rotationAxes[currentModelIndex];
        float joystickInput = joystick.Horizontal;

        if (Mathf.Abs(joystickInput) > 0.1f)
        {
            if (rotationAxis == 0) // X-axis
            {
                placedModel.transform.Rotate(Vector3.right, joystickInput * rotationSpeed * Time.deltaTime);
            }
            else if (rotationAxis == 1) // Y-axis
            {
                placedModel.transform.Rotate(Vector3.up, joystickInput * rotationSpeed * Time.deltaTime);
            }
            else if (rotationAxis == 2) // Z-axis
            {
                placedModel.transform.Rotate(Vector3.forward, joystickInput * rotationSpeed * Time.deltaTime);
            }
        }
    }

    private void PlaceModel(Vector3 position, Quaternion rotation)
    {
        if (placedModel != null)
        {
            Destroy(placedModel);
        }

        Vector3 modelRotation = modelRotations[currentModelIndex];
        placedModel = Instantiate(modelPrefabs[currentModelIndex], position, Quaternion.Euler(modelRotation));
        placedModels.Add(placedModel);

        initialScale = placedModel.transform.localScale;
        targetScale = initialScale; // Set initial target scale to the current model scale

        // Stop plane detection when model is placed
        if (arPlaneManager != null)
        {
            arPlaneManager.enabled = false; // Disable ARPlaneManager (stop plane detection)
            foreach (var plane in arPlaneManager.trackables)
            {
                plane.gameObject.SetActive(false); // Hide detected planes
            }
        }
    }


    public void PlayAudio()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
            UpdateButtonVisibility();
        }
    }

    public void PauseAudio()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
            UpdateButtonVisibility();
        }
    }

    private void UpdateButtonVisibility()
    {
        if (audioSource != null)
        {
            bool isPlaying = audioSource.isPlaying;

            if (playButton != null) playButton.gameObject.SetActive(!isPlaying);
            if (pauseButton != null) pauseButton.gameObject.SetActive(isPlaying);
        }
    }

    public void LoadNextModel()
    {
        if (modelPrefabs.Length == 0) return;

        currentModelIndex = (currentModelIndex + 1) % modelPrefabs.Length;
        ReplaceCurrentModel();
    }

    public void LoadPreviousModel()
    {
        if (modelPrefabs.Length == 0) return;

        currentModelIndex = (currentModelIndex - 1 + modelPrefabs.Length) % modelPrefabs.Length;
        ReplaceCurrentModel();
    }

    private void ReplaceCurrentModel()
    {
        if (placedModel != null)
        {
            Vector3 currentPosition = placedModel.transform.position;
            Quaternion currentRotation = placedModel.transform.rotation;

            Destroy(placedModel);
            placedModel = Instantiate(modelPrefabs[currentModelIndex], currentPosition, Quaternion.Euler(modelRotations[currentModelIndex]));
            placedModels.Add(placedModel);

            initialScale = placedModel.transform.localScale;
            targetScale = initialScale;

            placedModel.transform.position = currentPosition;
            placedModel.transform.rotation = Quaternion.Euler(modelRotations[currentModelIndex]);
        }
    }

    public void ResetPlacedModels()
    {
        foreach (GameObject model in placedModels)
        {
            if (model != null)
            {
                Destroy(model);
            }
        }

        placedModels.Clear();

        // Reset plane detection when all models are removed
        if (arPlaneManager != null)
        {
            arPlaneManager.enabled = true; // Re-enable ARPlaneManager (start plane detection)
            foreach (var plane in arPlaneManager.trackables)
            {
                plane.gameObject.SetActive(true); // Show detected planes again
            }
        }
    }

}