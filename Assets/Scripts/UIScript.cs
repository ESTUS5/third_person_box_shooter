using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    [SerializeField] private HealthScript Health;
    [SerializeField] private Text HealthBar;
    private float lerpTimer;
    [SerializeField] private float chipSpeed;
    private PlayerAttack playerAttack;
    [SerializeField] private Text normalCooldown;
    [SerializeField] private Text chargedCooldown;
    private Stats stats;
    [SerializeField] private Text statsText;
    private int oldHealth;
    void Start()
    {
        Health = GetComponent<HealthScript>();
        oldHealth = (int)Health.currentHealth;
        stats = GetComponent<Stats>();
        playerAttack = GetComponent<PlayerAttack>();
    }
    void LateUpdate()
    {
        if(oldHealth != (int)Health.currentHealth)
        {
            StartCoroutine(updateHealthUI(oldHealth));
            oldHealth = (int)Health.currentHealth;
        }

        DisplayStats();
        DisplayCooldown();

    }
    void DisplayStats()
    {
        statsText.text = "Stats:\n";
        stats.GetStatList().ForEach(x => {
            statsText.text += x.name + " " + x.GetValue() +"\n";
        });
    }
    void DisplayCooldown()
    {
        if(playerAttack.lastChargedShotTime < 0)
        {
            chargedCooldown.text = "Charged shot ready";
        }
        else
        {
            chargedCooldown.text = "Charged shot " + playerAttack.lastChargedShotTime;
        }
        if(playerAttack.lastNormalShotTime < 0)
        {
            normalCooldown.text = "Normal shot ready";
        }
        else
        {
            normalCooldown.text = "Normal shot " + playerAttack.lastNormalShotTime;
        }
    }

    public IEnumerator updateHealthUI(int oldHealth)
    {
        lerpTimer = 0f;
        float percentComplete = 0f;
        while(percentComplete < 1f)
        {
            lerpTimer +=Time.deltaTime;
            percentComplete = (lerpTimer / chipSpeed);
            HealthBar.text = (int)Mathf.Lerp(oldHealth,Health.currentHealth,percentComplete) + " Health";
            yield return null;
        }
    }
}
