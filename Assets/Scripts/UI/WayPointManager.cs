using UnityEngine;
using System.Collections.Generic;

public class WaypointManager : MonoBehaviour
{
    public static WaypointManager Instance;

    public Transform player;

    [Header("Prefab de Waypoint")]
    public GameObject waypointPrefab;
    public Transform canvasParent;

    private List<WaypointIndicatorUI> activeWaypoints = new List<WaypointIndicatorUI>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        player = FindFirstObjectByType<PlayerMoviment>().transform;
    }

    // Chame essa função quando spawnar um objeto que precisa de indicador
    public void TrackTarget(Transform target)
    {
        // Verifica se já existe um indicador para esse alvo
        foreach (var wp in activeWaypoints)
        {
            if (wp.target == target)
            {
                return;
            }
        }

        GameObject go = Instantiate(waypointPrefab, canvasParent);
        WaypointIndicatorUI indicator = go.GetComponent<WaypointIndicatorUI>();
        indicator.target = target;
        activeWaypoints.Add(indicator);
    }

    public void RemoveTarget(Transform target)
    {
        for (int i = activeWaypoints.Count - 1; i >= 0; i--)
        {
            if (activeWaypoints[i].target == target)
            {
                Destroy(activeWaypoints[i].gameObject);
                activeWaypoints.RemoveAt(i);
            }
        }
    }
}
