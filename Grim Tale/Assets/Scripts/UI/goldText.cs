using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class goldText : MonoBehaviour
{

    private TextMeshProUGUI gold;

    // Start is called before the first frame update
    void Start()
    {
        gold = GetComponent<TextMeshProUGUI>();
    }

    public void updateGoldText(int amount)
    {
        gold.text = amount.ToString();
    }

}
