using UnityEngine;

public class BuyMenu : MonoBehaviour
{
    public static bool GamePaused = false;
    public GameObject buyMenuUI;
    public GameObject ally;
    private PlayerController pc;
    //If these are changed, change the text in the buy Menu!
    [SerializeField] private int HpCost;
    [SerializeField] private int ManaCost;
    [SerializeField] private int AtkSPDCost;
    [SerializeField] private int AtkDMGCost;
    [SerializeField] private int AllyCost;
    /////////////////////////////////////////////////////////

    private void Awake()
    {
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (GamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void BuyHP()
    {
        FindObjectOfType<AudioManager>().PlayOneShot(Clip.GoldInteract);
        
        if (pc.getGold() >= HpCost)
        {
            pc.Heal();
            pc.setGold(pc.getGold() - HpCost);
        }
    }

    public void BuyMana()
    {
        FindObjectOfType<AudioManager>().PlayOneShot(Clip.GoldInteract);

        if (pc.getGold() >= ManaCost)
        {
            pc.RegenMana();
            pc.setGold(pc.getGold() - HpCost);
        }
    }

    public void BuyAtkSPD()
    {
        FindObjectOfType<AudioManager>().PlayOneShot(Clip.GoldInteract);

        if(pc.getGold() >= AtkSPDCost)
        {
            pc.incrementLightAttackSPD();
            pc.incrementHeavyAttackSPD();
            pc.setGold(pc.getGold() - AtkSPDCost);
        }
    }

    public void BuyAtkDMG()
    {
        FindObjectOfType<AudioManager>().PlayOneShot(Clip.GoldInteract);

        if (pc.getGold() >= AtkDMGCost)
        {
            pc.incrementLightAttackDMG();
            pc.incrementHeavyAttackDMG();
            pc.setGold(pc.getGold() - AtkDMGCost);
        }
    }

    public void BuyAlly()
    {
        FindObjectOfType<AudioManager>().PlayOneShot(Clip.GoldInteract);

        if (pc.getGold() >= AllyCost)
        {
            Instantiate(ally, new Vector3(10.0f, 0f, 12.5f), Quaternion.identity);
            pc.setGold(pc.getGold() - AllyCost);
        }
    }

    public void Resume()
    {
        buyMenuUI.SetActive(false);
        pc.input.Controls.Enable();
        Time.timeScale = 1f;
        GamePaused = false;
    }

    private void Pause()
    {
        buyMenuUI.SetActive(true);
        pc.input.Controls.Disable();
        Time.timeScale = 0f;
        GamePaused = true;
    }
}
