using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construction : MonoBehaviour
{

    public static Construction inst;

    [Header("Prefabs")]
    public Buildable[] buildables;

    [Header ("Menu")]
    public GameObject buildMenu;

    [Header ("Placing Behaviour")]
    public float rotationStepDeg = 45.0f;
    public Material placePreviewMatValid, placePreviewMatInvalid;

    [Header("Keyboard Assignments")]
    public KeyCode buildKey = KeyCode.B;
    public KeyCode rotateKey = KeyCode.R;

    BuildMenu menu;
    GameObject placePreview;
    Buildable activeBuildable;

    private void Awake () {
        inst = this;
        menu = buildMenu.GetComponent<BuildMenu> ();
        placePreview = new GameObject ("Place Preview");
        placePreview.SetActive (false);
    }

    private void Update () {

        // Open build menu if key pressed
        if(Input.GetKeyDown(buildKey) && !buildMenu.activeInHierarchy) {
            buildMenu.SetActive (true);
        }

        // Manipulate place preview
        if(placePreview.activeInHierarchy) {

            // move to cursor position
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)) {
                placePreview.transform.position = hit.point;
            }

            // rotate
            if(Input.GetKeyDown(rotateKey)) {
                if (!Input.GetKey (KeyCode.LeftShift)) placePreview.transform.Rotate (Vector3.up, rotationStepDeg);
                else placePreview.transform.Rotate (Vector3.down, rotationStepDeg);
            }

            // Place object
            if(Input.GetMouseButtonDown(0)) {
                GameObject g = Instantiate (activeBuildable.prefab);
                g.transform.position = placePreview.transform.position;
                g.transform.rotation = placePreview.transform.rotation;
            }
        }
    }

    public void SelectBuildable (Buildable b) {
        activeBuildable = b;
        buildMenu.SetActive (false);
        placePreview.SetActive (true);
        placePreview.transform.position = Vector3.zero;
        placePreview.transform.rotation = Quaternion.identity;

        // Instantiate prefab as the place preview
        GameObject g = Instantiate (b.prefab, placePreview.transform);
        foreach (Collider col in placePreview.GetComponentsInChildren<Collider> ()) Destroy (col);
        SetPlacePreviewMaterial (placePreviewMatValid);
    }

    public void SetPlacePreviewMaterial (Material mat) {
        foreach (MeshRenderer mr in placePreview.GetComponentsInChildren<MeshRenderer> ()) mr.material = mat;
    }
}
