using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Construction : MonoBehaviour {

    public static Construction inst;

    [Header ("Prefabs")]
    public Buildable[] buildables;

    [Header ("UI")]
    public GameObject buildMenu;
    public TMP_Text placeErrorText;

    [Header ("Placing Behaviour")]
    public float rotationStepDeg = 45.0f;
    public LayerMask placeLayerMask;
    public Material placePreviewMatValid, placePreviewMatInvalid;

    [Header ("Keyboard Assignments")]
    public KeyCode buildKey = KeyCode.B;
    public KeyCode rotateKey = KeyCode.R;

    bool placeValid = false;

    ResourceManager rm;
    MazeMaker mm;
    GameManager gm;
    BuildMenu menu;
    GameObject placePreview;

    Buildable activeBuildable;
    Vector2Int placePreviewChunk;

    private void Awake () {
        inst = this;
        rm = FindObjectOfType<ResourceManager> ();
        mm = FindObjectOfType<MazeMaker> ();
        gm = FindObjectOfType<GameManager> ();
        menu = buildMenu.GetComponent<BuildMenu> ();
        placePreview = new GameObject ("Place Preview");
        placePreview.SetActive (false);
    }

    bool wasBuildMenuOpen = false;
    bool wasPlacing = false;

    private void Update () {
        wasBuildMenuOpen = buildMenu.activeInHierarchy;
        wasPlacing = placePreview.activeInHierarchy;

        // Open build menu if key pressed
        if (Input.GetKeyDown (buildKey) && !buildMenu.activeInHierarchy && !gm.isPaused) {
            buildMenu.SetActive (true);
            placePreview.SetActive (false); // cancel place if build reopened
        }

        // Esc closes build menu
        if (Input.GetKeyDown (KeyCode.Escape)) buildMenu.SetActive (false);

        // Manipulate place preview
        if (placePreview.activeInHierarchy) {

            placePreviewChunk = mm.WorldPosToChunkNum (placePreview.transform.position);

            // set valid/invalid
            SetPlacePreviewValid (rm.SufficientResources (activeBuildable.costs) && placePreviewChunk == Vector2Int.zero);

            // cancel with Esc or RMB
            if (Input.GetKeyDown (KeyCode.Escape) || Input.GetMouseButtonDown (1)) placePreview.SetActive (false);

            // move to cursor position
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast (ray, out hit, maxDistance: 1000f, layerMask: placeLayerMask)) {
                placePreview.transform.position = hit.point;
            }

            // rotate
            if (Input.GetKeyDown (rotateKey)) {
                if (!Input.GetKey (KeyCode.LeftShift)) placePreview.transform.Rotate (Vector3.up, rotationStepDeg);
                else placePreview.transform.Rotate (Vector3.down, rotationStepDeg);
            }

            // Place object
            if (placeValid && Input.GetMouseButtonDown (0) && rm.Remove (activeBuildable.costs)) {
                GameObject g = Instantiate (activeBuildable.prefab);
                g.transform.position = placePreview.transform.position;
                g.transform.rotation = placePreview.transform.rotation;
                // re-bake chunk nav mesh
                mm.chunks[placePreviewChunk].GetComponent<MazeChunk> ().RebuildNavMesh ();
            }
        }

        bool invalidPlace = !placeValid && placePreview.activeInHierarchy;
        placeErrorText.gameObject.SetActive (invalidPlace);
        if (invalidPlace) {
            if (!rm.SufficientResources (activeBuildable.costs)) placeErrorText.text = "Not enough resources";
            if (placePreviewChunk != Vector2Int.zero) placeErrorText.text = "Cannot build outside the centre";
            placeErrorText.rectTransform.position = Input.mousePosition;
        }
    }

    public void SelectBuildable (Buildable b) {
        activeBuildable = b;
        buildMenu.SetActive (false);
        placePreview.SetActive (true);
        placePreview.transform.position = Vector3.zero;
        placePreview.transform.rotation = Quaternion.identity;

        // Remove any prior children of the place preview
        for (int i = 0; i < placePreview.transform.childCount; i++) Destroy (placePreview.transform.GetChild (i).gameObject);

        // Instantiate prefab as the place preview
        GameObject g = Instantiate (b.prefab, placePreview.transform);
        foreach (Collider col in placePreview.GetComponentsInChildren<Collider> ()) Destroy (col);
        SetPlacePreviewValid (true);
    }

    public void CancelPlace () {
        placePreview.SetActive (false);
    }

    public bool isPlacing { get => wasPlacing; }
    public bool buildMenuOpen { get => wasBuildMenuOpen; }

    public void SetPlacePreviewValid (bool valid) {
        _SetPlacePreviewMaterial (valid ? placePreviewMatValid : placePreviewMatInvalid);
        placeValid = valid;
    }

    public void _SetPlacePreviewMaterial (Material mat) {
        foreach (MeshRenderer mr in placePreview.GetComponentsInChildren<MeshRenderer> ()) mr.material = mat;
    }
}