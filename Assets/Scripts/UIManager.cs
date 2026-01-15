using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Battle Log")]
    public TextMeshProUGUI battleLogText;

    [Header("Health Bars")]
    public Slider playerHealthSlider;
    public Slider enemyHealthSlider;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI enemyHealthText;

    [Header("Mana Bars")]
    public Slider playerManaSlider;
    public Slider enemyManaSlider;
    public TextMeshProUGUI playerManaText;
    public TextMeshProUGUI enemyManaText;

    [Header("Ammo & Distance")]
    public TextMeshProUGUI playerAmmoText;
    public TextMeshProUGUI enemyAmmoText;
    public TextMeshProUGUI distanceText; 

    [Header("Turn Text")]
    public TextMeshProUGUI turnText; 

    [Header("Turn Indicators")]
    public GameObject playerTurnIndicator;
    public GameObject enemyTurnIndicator;

    [Header("Action Buttons")]
    public Button[] actionButtons; 
    public GameObject meleeChoicePanel;

    [Header("End Game")]
    public GameObject endGamePanel;
    public TextMeshProUGUI endGameMessageText;

    // --- TEMEL GÜNCELLEMELER ---

    public void SetTurnText(string txt)
    {
        if (turnText != null) 
        {
            turnText.text = txt;
            
            turnText.enableAutoSizing = true;
            turnText.fontSizeMin = 18; // En küçük bu kadar olsun
            turnText.fontSizeMax = 50; // En büyük bu kadar olsun
        }
    }

    public void UpdateDistanceText(DistanceLevel distance)
    {
        if (distanceText == null) return;
        // Sadece enum ismini yazdır (Distance: Mid)
        distanceText.text = "Distance: " + distance.ToString();
    }

    public void UpdateBattleLog(string message)
    {
        if (battleLogText != null) battleLogText.text = message;
    }

    // --- BARLAR VE SLIDERLAR ---
    public void UpdateHealth(float playerHP, float enemyHP, float maxHP)
    {
        if (playerHealthSlider) playerHealthSlider.value = playerHP / maxHP;
        if (enemyHealthSlider) enemyHealthSlider.value = enemyHP / maxHP;
        
        if (playerHealthText) playerHealthText.text = $"{playerHP}/{maxHP}";
        if (enemyHealthText) enemyHealthText.text = $"{enemyHP}/{maxHP}";
    }

    public void UpdateMana(float playerMana, float enemyMana, float maxMana)
    {
        if (playerManaSlider) playerManaSlider.value = playerMana / maxMana;
        if (enemyManaSlider) enemyManaSlider.value = enemyMana / maxMana;

        if (playerManaText) playerManaText.text = $"{playerMana}/{maxMana}";
        if (enemyManaText) enemyManaText.text = $"{enemyMana}/{maxMana}";
    }

    public void UpdateAmmo(int playerAmmo, int enemyAmmo)
    {
        if (playerAmmoText) playerAmmoText.text = $"Ok: {playerAmmo}";
        if (enemyAmmoText) enemyAmmoText.text = $"Ok: {enemyAmmo}";
    }

    // --- BUTON VE OYUN KONTROLÜ ---
    public void UpdateTurnIndicator(bool isPlayerTurn)
    {
        if (playerTurnIndicator) playerTurnIndicator.SetActive(isPlayerTurn);
        if (enemyTurnIndicator) enemyTurnIndicator.SetActive(!isPlayerTurn);
    }

    public void UpdateActionButtonsInteractable(bool interactable)
    {
        foreach (var btn in actionButtons)
        {
            if (btn != null) btn.interactable = interactable;
        }
    }

    public void ShowMeleeChoicePanel(bool show)
    {
        if (meleeChoicePanel) meleeChoicePanel.SetActive(show);
    }

    public void ShowEndGamePanel(string message)
    {
        if (endGamePanel)
        {
            endGamePanel.SetActive(true);
            if (endGameMessageText) endGameMessageText.text = message;
        }
    }
}