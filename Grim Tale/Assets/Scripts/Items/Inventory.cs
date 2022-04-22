using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject goldPrefab;

    void Start()
    {
        var stuff = Instantiate(goldPrefab, new Vector3(0f, 0f, 22.5f), Quaternion.identity);
        var stuff2 = Instantiate(goldPrefab, new Vector3(0f, 0f, 2.5f), Quaternion.identity);
        var stuff3 = Instantiate(goldPrefab, new Vector3(20f, 0f, 2.5f), Quaternion.identity);
        var stuff4 = Instantiate(goldPrefab, new Vector3(20f, 0f, 22.5f), Quaternion.identity);
    }
}
