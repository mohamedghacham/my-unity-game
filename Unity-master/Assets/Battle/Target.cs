using UnityEngine;
using System.Collections.Generic;

public class Target : MonoBehaviour
{
    private List<Transform> allPokemon = new List<Transform>();
    private Transform targetedPokemon;
    private Transform playerTransform;
    public bool activeTarget = false;
    public GameObject highlightSparkles;

    private GameGUI gamegui;

    void Start()
    {
        Debug.Log("Target started");
        allPokemon = new List<Transform>();
        targetedPokemon = null;

        // Assigne le GameGUI du joueur si présent dans la scène
        gamegui = FindObjectOfType<GameGUI>();
        playerTransform = Player.trainer?.transform;

        if (playerTransform == null)
            Debug.LogWarning("Player transform not found at Target.Start()");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag("pokemon"))
            {
                TargetPokemon(hit.transform);
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (activeTarget)
                UnHighlightTarget();

            Transform nearest = FindNearestPokemon();
            if (nearest != null)
                TargetPokemon(nearest);
        }

        if (activeTarget && targetedPokemon != null && highlightSparkles != null)
        {
            highlightSparkles.transform.position = targetedPokemon.position;
        }
    }

    private void AddTargetPokemon()
    {
        foreach (GameObject tmpPokemon in GameObject.FindGameObjectsWithTag("pokemon"))
        {
            AddTarget(tmpPokemon.transform);
        }
    }

    private void AddTarget(Transform addThisPokemon)
    {
        if (!allPokemon.Contains(addThisPokemon))
            allPokemon.Add(addThisPokemon);
    }

    private void SortTargetsByDistance()
    {
        if (playerTransform == null)
        {
            playerTransform = Player.trainer?.transform;
            if (playerTransform == null) return;
        }

        allPokemon.Sort((t1, t2) =>
            Vector3.Distance(t1.position, playerTransform.position)
            .CompareTo(Vector3.Distance(t2.position, playerTransform.position))
        );
    }

    private void LimitTargetDistance(float limit)
    {
        if (playerTransform == null)
        {
            playerTransform = Player.trainer?.transform;
            if (playerTransform == null) return;
        }

        allPokemon.RemoveAll(t => Vector3.Distance(t.position, playerTransform.position) > limit);
    }

    private Transform FindNearestPokemon()
    {
        if (playerTransform == null)
        {
            playerTransform = Player.trainer?.transform;
            if (playerTransform == null) return null;
        }

        if (allPokemon.Count == 0)
            AddTargetPokemon();

        SortTargetsByDistance();

        return allPokemon.Count > 0 ? allPokemon[0] : null;
    }

    public void TargetPokemon(Transform targetThis)
    {
        Debug.Log("Targeting Pokemon: " + targetThis.name);

        if (targetedPokemon != null)
            UnHighlightTarget();

        SetTarget(targetThis);
        if (gamegui != null)
            gamegui.SetChatWindow("Targeted " + targetThis.name);

        HighlightTarget();
    }

    public void HighlightTarget()
    {
        highlightSparkles = Instantiate(Resources.Load<GameObject>("ReturnEffect"));
        SetActiveTarget(true);
    }

    public void UnHighlightTarget()
    {
        targetedPokemon = null;
        SetActiveTarget(false);

        if (highlightSparkles != null)
        {
            Destroy(highlightSparkles);
            highlightSparkles = null;
        }
    }

    public Transform GetTargetPokemon()
    {
        return targetedPokemon;
    }

    public Transform GetHighlightSparkles()
    {
        return highlightSparkles?.transform;
    }

    public Transform GetTarget()
    {
        return targetedPokemon;
    }

    public void SetTarget(Transform newTarget)
    {
        targetedPokemon = newTarget;
    }

    public bool GetActiveTarget()
    {
        return activeTarget;
    }

    public void SetActiveTarget(bool status)
    {
        activeTarget = status;
    }
}
