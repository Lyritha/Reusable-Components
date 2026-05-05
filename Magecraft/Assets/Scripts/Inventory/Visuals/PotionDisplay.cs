using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PotionDisplay : MonoBehaviour
{
    [SerializeField, Header("Animation")] private AnimateTowards shelfParent;
    [SerializeField] private float animationTime = 1f;
    [SerializeField] private float timePerInstance = .05f;

    [SerializeField, Header("Prefabs")] private GameObject shelfPrefab;
    [SerializeField] private GameObject potionPrefab;

    [SerializeField, Header("Shelf positioning")] private float shelfOverlap = .01f;
    [SerializeField] private float shelfEndOffset = 1;

    [SerializeField, Header("Potion positioning")] private int potionsPerShelf = 4;

    // keep track of exisiting shelfs
    private GameObject[] spawnedShelfs;
    private GameObject[] spawnedBufferShelfs;

    private float height = 0;

    [ContextMenu("show shelf")]
    public void Show() => ShowPotions(7);
    [ContextMenu("show fun shelf")]
    public void Showfun() => ShowPotions(400);


    [ContextMenu("hide")]
    public void HidePotions()
    {
        float animateTime = animationTime + (timePerInstance * spawnedShelfs.Length);

        Vector3 targetPos = shelfParent.transform.localPosition;
        targetPos.y -= height + shelfEndOffset;

        shelfParent.Animate(targetPos, animateTime, true);

        shelfParent.OnFinishedAnimating += ClearShelfs;
    }

    private void ClearShelfs()
    {
        foreach (Transform child in shelfParent.transform) Destroy(child.gameObject);
        shelfParent.Reset();

        shelfParent.OnFinishedAnimating -= ClearShelfs;
    }

    public void ShowPotions(int potionCount)
    {
        // clear existing gameobjects from shelf parent.
        foreach (Transform child in shelfParent.transform) Destroy(child.gameObject);
        shelfParent.Reset();

        int shelfCount = Mathf.CeilToInt(potionCount / (float)potionsPerShelf);
        float totalHeight = SpawnShelfs(shelfCount, shelfCount / 4);
        SpawnPotions(potionCount);

        Vector3 targetPos = shelfParent.transform.localPosition;
        targetPos.y += totalHeight + shelfEndOffset;

        float animateTime = animationTime + (timePerInstance * spawnedShelfs.Length);
        shelfParent.Animate(targetPos, animateTime);

        height = totalHeight;
    }

    private float SpawnShelfs(int count, int bufferCount)
    {
        // avoid trying to run with 0 required shelfs
        if (count <= 0) return 0;

        spawnedShelfs = new GameObject[count];
        spawnedBufferShelfs = new GameObject[bufferCount];

        Renderer prefabRenderer = shelfPrefab.GetComponentInChildren<Renderer>();
        Bounds shelfSize = prefabRenderer.localBounds;
        float offsetPerShelf = shelfSize.size.y - shelfOverlap;

        float heigtOffset = SpawnShelfGroup(count, 0, offsetPerShelf, spawnedShelfs);
        SpawnShelfGroup(bufferCount, heigtOffset, offsetPerShelf, spawnedBufferShelfs);

        return offsetPerShelf * count;
    }
    private float SpawnShelfGroup(int count, float startOffset, float addedOffset, GameObject[] array)
    {
        for (int i = 0; i < count; i++)
        {
            startOffset += addedOffset;

            GameObject shelf = Instantiate(shelfPrefab, shelfParent.transform);
            array[i] = shelf;

            Vector3 pos = shelf.transform.localPosition;
            pos.y -= startOffset;

            shelf.transform.localPosition = pos;
        }

        return startOffset;
    }
    private void ClearBufferShelfs()
    {
        foreach (GameObject child in spawnedBufferShelfs)
            Destroy(child);
    }


    private void SpawnPotions(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int shelfIndex = i / potionsPerShelf;
            GameObject targetShelf = spawnedShelfs[shelfIndex];

            GameObject potion = Instantiate(potionPrefab, targetShelf.transform);
        }
    }
}
