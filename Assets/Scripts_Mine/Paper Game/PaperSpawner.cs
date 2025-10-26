using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperSpawner : MonoBehaviour
{
    public GameObject paperPrefab;
    public Transform spawnPoint;
    public float respawnDelay = 1f;

    private GameObject currentPaper;

    // Start is called before the first frame update
    void Start()
    {
        SpawnPaper();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPaper == null)
        {
            Invoke(nameof(SpawnPaper), respawnDelay);
        }
    }

    void SpawnPaper()
    {
        if (currentPaper != null) return; //Evita duplicados
        currentPaper = Instantiate(paperPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
