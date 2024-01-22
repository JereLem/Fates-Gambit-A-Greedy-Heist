using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{   
    [Header("Prefabs")]
    [SerializeField] private GameObject policePrefab;
    [SerializeField] private GameObject pedestrianPrefab;

    [Header("Max NPCs")]
    [SerializeField] private int maxPoliceCount = 0;
    [SerializeField] private int maxPedestrianCount = 0;

    [Header("Spawn Intervals")]
    [SerializeField] private float spawnIntervalPolice = 10f;
    [SerializeField] private float spawnIntervalPedestrian = 5f;

    private List<GameObject> activePoliceNPCs = new List<GameObject>();
    private List<GameObject> activePedestrians = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnNPCs(spawnIntervalPolice, policePrefab, "PoliceNPCs", maxPoliceCount, activePoliceNPCs));
        StartCoroutine(SpawnNPCs(spawnIntervalPedestrian, pedestrianPrefab, "PedestrianNPCs", maxPedestrianCount, activePedestrians));
    }

    private IEnumerator SpawnNPCs(float interval, GameObject npcPrefab, string folderName, int maxCount, List<GameObject> activeList)
    {
        Transform folderTransform = null;

        if (!string.IsNullOrEmpty(folderName))
        {
            folderTransform = new GameObject(folderName).transform;
            folderTransform.parent = transform;
        }

        while (true)
        {
            yield return new WaitForSeconds(interval);

            if (activeList.Count < maxCount)
            {
                GameObject newNPC = Instantiate(npcPrefab, folderTransform);
                activeList.Add(newNPC);
            }
        }

    }

    public void NPCCompletedCycle(GameObject npc)
    {
        if (npc != null)
        {
            if (npc.CompareTag("PoliceNPC"))
            {
                PoliceNPC policeScript = npc.GetComponent<PoliceNPC>();
                if (policeScript != null && policeScript.completedCycles >= policeScript.maxCycles)
                {
                    activePoliceNPCs.Remove(npc);
                    Destroy(npc);
                }
            }
            else if (npc.CompareTag("PedestrianNPC"))
            {
                PedestrianNPC pedestrianScript = npc.GetComponent<PedestrianNPC>();
                if (pedestrianScript != null && pedestrianScript.completedCycles >= pedestrianScript.maxCycles)
                {
                    activePedestrians.Remove(npc);
                    Destroy(npc);
                }
            }
        }
    }
}
