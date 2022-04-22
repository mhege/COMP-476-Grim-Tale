using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldScript : MonoBehaviour
{
    private int value;
    private goldText goldtext;
    private int currentGold;

    void Start()
    {
        goldtext = GameObject.FindGameObjectWithTag("goldtext").GetComponent<goldText>();
        value = currentGold + Random.Range(1, 10);
    }

    public void SetValue(int val)
    {
        value = val;
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerController>();

        if (!player) return;
        
        FindObjectOfType<AudioManager>().PlayOneShot(Clip.GoldInteract);
        
        player.Loot(value);
        currentGold = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().getGold();
        goldtext.updateGoldText(currentGold);
        Destroy(gameObject);
    }
}
