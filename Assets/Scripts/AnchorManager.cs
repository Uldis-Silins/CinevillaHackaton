using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_anchorPrefab;

    public Transform cameraTransform;
    public bool savingEnabled = true;

    private List<OVRSpatialAnchor> _anchorInstances = new(); // Active instances (red and green)

    private HashSet<Guid> _anchorUuids = new(); // Simulated external location, like PlayerPrefs

    private Action<bool, OVRSpatialAnchor.UnboundAnchor> _onLocalized;

    private List<GameObject> m_spawnedAnchors = new();

    public bool IsAnchorSpawned => m_spawnedAnchors.Count > 0;

    private void Start()
    {
        LoadAllAnchors();
    }

    public void SaveAnchor()
    {
        if (!savingEnabled || IsAnchorSpawned) return;

        var go = Instantiate(m_anchorPrefab, cameraTransform.position, Quaternion.Euler(0f, cameraTransform.eulerAngles.y, 0f));
        m_spawnedAnchors.Add(go);
        SetupAnchorAsync(go.AddComponent<OVRSpatialAnchor>(), saveAnchor: true);
    }

    public void EraseAnchors()
    {
        EraseAllAnchors();
    }

    // You need to make sure the anchor is ready to use before you save it.
    // Also, only save if specified
    private async void SetupAnchorAsync(OVRSpatialAnchor anchor, bool saveAnchor)
    {
        // Keep checking for a valid and localized anchor state
        if (!await anchor.WhenLocalizedAsync())
        {
            Debug.LogError($"Unable to create anchor.");
            Destroy(anchor.gameObject);
            return;
        }

        // Add the anchor to the list of all instances
        _anchorInstances.Add(anchor);

        // You save the savable (green) anchors only
        if (saveAnchor && (await anchor.SaveAnchorAsync()).Success)
        {
            // Remember UUID so you can load the anchor later
            _anchorUuids.Add(anchor.Uuid);
        }
    }

    /******************* Load Anchor Methods **********************/
    public async void LoadAllAnchors()
    {
        // Load and localize
        var unboundAnchors = new List<OVRSpatialAnchor.UnboundAnchor>();
        var result = await OVRSpatialAnchor.LoadUnboundAnchorsAsync(_anchorUuids, unboundAnchors);

        if (result.Success)
        {
            foreach (var anchor in unboundAnchors)
            {
                anchor.LocalizeAsync().ContinueWith(_onLocalized, anchor);
            }
        }
        else
        {
            Debug.LogError($"Load anchors failed with {result.Status}.");
        }
    }

    private void OnLocalized(bool success, OVRSpatialAnchor.UnboundAnchor unboundAnchor)
    {
        var pose = unboundAnchor.Pose;
        var go = Instantiate(m_anchorPrefab, pose.position, pose.rotation);
        m_spawnedAnchors.Add(go);
        var anchor = go.AddComponent<OVRSpatialAnchor>();

        unboundAnchor.BindTo(anchor);

        // Add the anchor to the running total
        _anchorInstances.Add(anchor);
    }

    /******************* Erase Anchor Methods *****************/
    // If the Y button is pressed, erase all anchors saved
    // in the headset, but don't destroy them. They should remain displayed.
    public async void EraseAllAnchors()
    {
        var result = await OVRSpatialAnchor.EraseAnchorsAsync(anchors: null, uuids: _anchorUuids);
        if (result.Success)
        {
            // Erase our reference lists
            _anchorUuids.Clear();

            Debug.Log($"Anchors erased.");
        }
        else
        {
            Debug.LogError($"Anchors NOT erased {result.Status}");
        }

        for (int i = m_spawnedAnchors.Count - 1; i >= 0; --i)
        {
            Destroy(m_spawnedAnchors[i]);
        }

        m_spawnedAnchors.Clear();
    }
}