using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetIndicatorSystem : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public RectTransform canvasRect;

    [Header("Prefabs")]
    public GameObject enemyPrefab;
    public GameObject friendlyPrefab;
    public GameObject groundPrefab;

    [Header("Settings")]
    public float edgePadding = 40f;
    public bool showGroundTargets = true;

    Dictionary<Transform, RectTransform> indicators = new Dictionary<Transform, RectTransform>();

    void Start()
    {
        if (cam == null)
            cam = Camera.main;
    }

    void Update()
    {
        UpdateTargets("Enemy", enemyPrefab);
        UpdateTargets("Friendly", friendlyPrefab);

        if (showGroundTargets)
            UpdateTargets("Ground", groundPrefab);
    }

    void UpdateTargets(string tag, GameObject prefab)
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject target in targets)
        {
            if (!indicators.ContainsKey(target.transform))
            {
                GameObject ui = Instantiate(prefab, canvasRect);
                indicators.Add(target.transform, ui.GetComponent<RectTransform>());
            }

            UpdateIndicator(target.transform, indicators[target.transform]);
        }
    }

    void UpdateIndicator(Transform target, RectTransform indicator)
    {
        Vector3 screenPos = cam.WorldToScreenPoint(target.position);

        bool isBehind = screenPos.z < 0;

        if (isBehind)
        {
            screenPos *= -1;
        }

        // Clamp to screen edges
        float x = Mathf.Clamp(screenPos.x, edgePadding, Screen.width - edgePadding);
        float y = Mathf.Clamp(screenPos.y, edgePadding, Screen.height - edgePadding);

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            new Vector2(x, y),
            null,
            out pos
        );

        indicator.anchoredPosition = pos;

        // Rotate toward target
        Vector3 dir = (target.position - cam.transform.position).normalized;
        Vector3 screenDir = cam.transform.InverseTransformDirection(dir);

        float angle = Mathf.Atan2(screenDir.x, screenDir.y) * Mathf.Rad2Deg;
        indicator.rotation = Quaternion.Euler(0, 0, -angle);

        // Optional: scale based on distance
        float dist = Vector3.Distance(cam.transform.position, target.position);
        float scale = Mathf.Clamp(1f / (dist * 0.01f), 0.5f, 1.5f);
        indicator.localScale = Vector3.one * scale;
    }
}