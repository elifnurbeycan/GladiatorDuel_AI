using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAgent : MonoBehaviour
{
    public Gladiator enemy;  
    public Gladiator player; 
    private QLearningBrain brain;

    [Header("Eğitim Ayarları")]
    public float decisionDelay = 0.05f; // Düşünme hızı
    
    private float previousPlayerHP;

    void Start()
    {
        brain = GetComponent<QLearningBrain>();
        
        // Aksiyonları Tanımla.
        brain.RegisterAction("MoveForward",  _ => ExecuteAction(0), 0);
        brain.RegisterAction("MoveBackward", _ => ExecuteAction(1), 0);
        brain.RegisterAction("RangedAttack", _ => ExecuteAction(2), 0);
        brain.RegisterAction("MeleeAttack",  _ => ExecuteAction(3), 0);
        brain.RegisterAction("Sleep",        _ => ExecuteAction(4), 0);
        brain.RegisterAction("ArmorUp",      _ => ExecuteAction(5), 0);

        brain.learningRate = 0.5f; 
        brain.discount = 0.8f;     
        
        
        if (GameManager.useTrainedAI)
        {
            // EĞİTİLMİŞ MOD:
            // 1. Parametre (true): Dosya kullanılsın mı? EVET.
            // 2. Parametre: Hangi dosya? "BlueBrain.json".
            brain.ConfigureBrain(true, "BlueBrain.json");
            
            brain.exploration = 0.1f; // Bildiğini okusun
            Debug.Log("Mod: AKILLI (BlueBrain Yüklendi)");
        }
        else
        {
            // RASTGELE MOD:
            // 1. Parametre (false): Dosya kullanılsın mı? HAYIR.
            // 2. Parametre: İsim önemsiz ("").
            brain.ConfigureBrain(false, "");
            
            brain.exploration = 1.0f; // %100 Rastgele sallasın
            Debug.Log("Mod: RASTGELE (Dosya Yok, Hafıza Yok)");
        }
    }
    public void StartEnemyTurn() 
    { 
        previousPlayerHP = player.currentHP;
        StartCoroutine(ThinkAndAct()); 
    }

    private IEnumerator ThinkAndAct()
    {
        
        string actorName = (enemy == GameManager.Instance.player) ? "MAVİ" : "KIRMIZI";
        GameManager.Instance.uiManager.SetTurnText(actorName + "..."); 
        
        UpdateBrainState();

        yield return new WaitForSeconds(decisionDelay);
        
        int actionIndex = brain.DecideAction();

        // İMKANSIZ HAMLEYE AĞIR CEZA (-50)
        if (!CheckActionLogic(actionIndex)) 
        {
            brain.Punish(50f); // -5 yerine -50 yaptık ki aptallık etmesin
            ForceRandomValidMove();
        }
        else 
        {
            ExecuteAction(actionIndex);
        }

        if (GameManager.Instance.isTrainingMode)
        {
            yield return new WaitForSeconds(0.05f);
        }
        else
        {
            yield return new WaitForSeconds(1.5f); // Normal modda animasyon bekle
        }
        
        EvaluateOutcome(actionIndex);

        // Sırayı Devret
        if (enemy == GameManager.Instance.player)
            GameManager.Instance.EndPlayerTurn();
        else
            GameManager.Instance.EndEnemyTurn();
    }

    private void ExecuteAction(int actionCode)
    {
        // UI'DA NE YAPTIĞINI YAZMA
        string name = (enemy == GameManager.Instance.player) ? "MAVİ" : "KIRMIZI";
        bool amIPlayer = (GameManager.Instance.player == enemy);

        switch (actionCode)
        {
            case 0: // İleri
                GameManager.Instance.uiManager.SetTurnText(name + ": İLERİ GELDİ"); 
                GameManager.Instance.MoveCloser(amIPlayer); 
                enemy.SpendMana(4); 
                break;

            case 1: // Geri
                GameManager.Instance.uiManager.SetTurnText(name + ": GERİ KAÇTI"); 
                GameManager.Instance.MoveAway(amIPlayer); 
                enemy.SpendMana(4); 
                break;
            
            case 2: // Ranged
                GameManager.Instance.uiManager.SetTurnText(name + ": OK ATTI!"); 
                enemy.currentAmmo--;
                enemy.SpendMana(12);
                string targetTag = (enemy.CompareTag("Player")) ? "Enemy" : "Player";
                enemy.ShootProjectile(targetTag, Random.Range(15, 21));
                break;

            case 3: // Melee
                GameManager.Instance.uiManager.SetTurnText(name + ": KILIÇ VURDU!"); 
                enemy.SpendMana(20); 
                enemy.TriggerAttack(); 
                player.TakeDamage(Random.Range(20, 31)); 
                break;

            case 4: // Sleep
                GameManager.Instance.uiManager.SetTurnText(name + ": DİNLENİYOR"); 
                enemy.RestoreMana(40); 
                break;

            case 5: // Armor
                GameManager.Instance.uiManager.SetTurnText(name + ": KALKAN ALDI"); 
                enemy.SpendMana(15); 
                enemy.ActivateArmorUp(2); 
                break;
        }
    }

    private void UpdateBrainState()
    {
        List<float> inputs = new List<float>();
        inputs.Add((float)GameManager.Instance.currentDistance);
        inputs.Add(enemy.currentMana >= 12 ? 1.0f : 0.0f);
        inputs.Add(enemy.currentAmmo > 0 ? 1.0f : 0.0f);
        inputs.Add(enemy.currentHP < 30 ? 1.0f : 0.0f);
        brain.SetInputs(inputs);
    }

    private void EvaluateOutcome(int actionIndex)
    {
        float reward = 0f;
        float damageDealt = previousPlayerHP - player.currentHP;

        if (damageDealt > 0) reward += damageDealt;
        if (actionIndex == 2) reward += 10f; 

        // DUVAR CEZASI VE KAÇIŞ MANTIĞI
        if (actionIndex == 1) // Geri Git
        {
            if (GameManager.Instance.currentDistance == DistanceLevel.Far)
            {
                reward -= 20f; // Zaten en uzaktasın, duvara sürtme cezası
            }
            else
            {
                reward += 5f; // Mesafe açmak iyidir
            }
        }

        if (actionIndex == 0 && GameManager.Instance.currentDistance == DistanceLevel.Close) reward -= 5f;
        if (actionIndex == 4 && enemy.currentMana < 20) reward += 10f;

        if (reward > 0) brain.Reward(reward);
        else if (reward < 0) brain.Punish(Mathf.Abs(reward));
    }

    private bool CheckActionLogic(int code)
    {
        switch (code)
        {
            case 0: return GameManager.Instance.currentDistance != DistanceLevel.Close && enemy.currentMana >= 4;
            case 1: return GameManager.Instance.currentDistance != DistanceLevel.Far && enemy.currentMana >= 4;
            case 2: return enemy.currentAmmo > 0 && enemy.currentMana >= 12; 
            case 3: return GameManager.Instance.currentDistance == DistanceLevel.Close && enemy.currentMana >= 20;
            case 4: return enemy.currentMana < enemy.maxMana;
            case 5: return enemy.currentMana >= 15;
        }
        return false;
    }
    
    public void ProcessMatchResult(bool amIWinner) 
    { 
        if (amIWinner) { brain.Reward(50f); brain.SaveState(); }
        else { brain.Punish(20f); brain.SaveState(); }
    }

    private void ForceRandomValidMove() 
    { 
        List<int> validMoves = new List<int>();
        for(int i=0; i<6; i++) if(CheckActionLogic(i)) validMoves.Add(i);
        if(validMoves.Count > 0) ExecuteAction(validMoves[Random.Range(0, validMoves.Count)]);
        else ExecuteAction(4); 
    }
}