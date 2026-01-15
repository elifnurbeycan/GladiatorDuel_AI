using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public enum DistanceLevel { Close, Mid, Far }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public bool isTrainingMode = false;
    public static bool useTrainedAI = true;

    [Header("Gladiators")]
    public Gladiator player; // Kırmızı
    public Gladiator enemy;  // Mavi
    
    [Header("Transforms")]
    public Transform playerTransform;
    public Transform enemyTransform;

    [Header("Managers")]
    public UIManager uiManager;

    [Header("Game State")]
    public bool isPlayerTurn = true;
    public DistanceLevel currentDistance = DistanceLevel.Mid;
    public int turnCount = 0;

    private float stepSize = 2.0f;    
    private float mapBoundary = 7.5f; 
    private float minDistance = 1.5f; 

    private bool isAnimating = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (isTrainingMode)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = -1;
            
            Time.timeScale = 100.0f; 
        }
        else
        {
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = 60;
            Time.timeScale = 1.0f;
        }

        if (!isTrainingMode) InitPositions(); 
        UpdateDistanceState();
        UpdateUI();
        
        StartTurn();
    }

    private void InitPositions()
    {
        if (playerTransform) playerTransform.position = new Vector3(-mapBoundary + 1f, playerTransform.position.y, 0);
        if (enemyTransform) enemyTransform.position = new Vector3(mapBoundary - 1f, enemyTransform.position.y, 0);
    }

    
    public void StartTurn()
    {
        if (isAnimating) return;
        turnCount++;

        if (isPlayerTurn)
        {
            uiManager.SetTurnText("--- SENİN SIRAN ---");
            uiManager.UpdateTurnIndicator(true);
            uiManager.UpdateActionButtonsInteractable(true);
            uiManager.ShowMeleeChoicePanel(false);

            if (player.GetComponent<EnemyAgent>() != null)
                player.GetComponent<EnemyAgent>().StartEnemyTurn();
        }
        else
        {
            uiManager.SetTurnText("RAKİP BEKLENİYOR...");
            uiManager.UpdateTurnIndicator(false);
            uiManager.UpdateActionButtonsInteractable(false);
            
            if (enemy.GetComponent<EnemyAgent>() != null)
                enemy.GetComponent<EnemyAgent>().StartEnemyTurn();
            else
                enemy.GetComponent<EnemyController>().StartEnemyTurn();
        }
    }

    public void EndPlayerTurn()
    {
        // Eğer animasyon varsa bekle (Eğitim modunda isAnimating hiç true olmayacak)
        if (isAnimating) return;
        
        player.OnTurnEnd(); 
        isPlayerTurn = false;
        UpdateUI();
        if (CheckGameOver()) return;
        StartTurn();
    }

    public void EndEnemyTurn()
    {
        if (isAnimating) return;
        
        enemy.OnTurnEnd();
        isPlayerTurn = true;
        UpdateUI();
        if (CheckGameOver()) return;
        StartTurn();
    }


    public void MoveCloser(bool isPlayerAction)
    {
        Transform actor = isPlayerAction ? playerTransform : enemyTransform;
        Transform target = isPlayerAction ? enemyTransform : playerTransform;

        float currentX = actor.position.x;
        float targetX;

        if (isPlayerAction)
        {
            targetX = currentX + stepSize;
            if (targetX > target.position.x - minDistance) targetX = target.position.x - minDistance;
        }
        else
        {
            targetX = currentX - stepSize;
            if (targetX < target.position.x + minDistance) targetX = target.position.x + minDistance;
        }

        // Eğitim modundaysak animasyon başlatma, direkt ışınla
        if (isTrainingMode)
        {
            actor.position = new Vector3(targetX, actor.position.y, actor.position.z);
            UpdateDistanceState();
            UpdateUI();
            // isAnimating'i true YAPMIYORUZ. Böylece oyun kilitlenmiyor.
        }
        else
        {
            StartCoroutine(SmoothMoveRoutine(isPlayerAction, targetX));
        }
    }

    public void MoveAway(bool isPlayerAction)
    {
        Transform actor = isPlayerAction ? playerTransform : enemyTransform;
        float currentX = actor.position.x;
        float targetX;

        if (isPlayerAction)
        {
            targetX = currentX - stepSize;
            if (targetX < -mapBoundary) targetX = -mapBoundary;
        }
        else
        {
            targetX = currentX + stepSize;
            if (targetX > mapBoundary) targetX = mapBoundary;
        }

        // Eğitim modundaysak direkt ışınla
        if (isTrainingMode)
        {
            actor.position = new Vector3(targetX, actor.position.y, actor.position.z);
            UpdateDistanceState();
            UpdateUI();
        }
        else
        {
            StartCoroutine(SmoothMoveRoutine(isPlayerAction, targetX));
        }
    }

    private IEnumerator SmoothMoveRoutine(bool isPlayerAction, float targetX)
    {
        isAnimating = true; // Sadece normal oyunda kilit vurulur
        uiManager.UpdateActionButtonsInteractable(false);

        Gladiator actorGladiator = isPlayerAction ? player : enemy;
        Transform actorTransform = isPlayerAction ? playerTransform : enemyTransform;

        actorGladiator.SetMoveAnimation(true);
        actorGladiator.ToggleWalkSound(true);

        Vector3 startPos = actorTransform.position;
        Vector3 endPos = new Vector3(targetX, startPos.y, startPos.z);
        
        float duration = 0.5f; // Normal oyun hızı
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            actorTransform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        actorTransform.position = endPos;

        actorGladiator.SetMoveAnimation(false);
        actorGladiator.ToggleWalkSound(false);

        UpdateDistanceState();
        UpdateUI();

        isAnimating = false; // Kilit açılır

        if (isPlayerTurn) uiManager.UpdateActionButtonsInteractable(true);
    }

    private void UpdateDistanceState()
    {
        if(playerTransform == null || enemyTransform == null) return;

        float dist = Vector3.Distance(playerTransform.position, enemyTransform.position);
        
        if (dist <= 2.5f) currentDistance = DistanceLevel.Close;
        else if (dist > 2.5f && dist <= 7.0f) currentDistance = DistanceLevel.Mid;
        else currentDistance = DistanceLevel.Far;

        if(uiManager != null) uiManager.UpdateDistanceText(currentDistance);
    }

    public void UpdateUI()
    {
        uiManager.UpdateHealth(player.currentHP, enemy.currentHP, player.maxHP);
        uiManager.UpdateMana(player.currentMana, enemy.currentMana, player.maxMana);
        uiManager.UpdateAmmo(player.currentAmmo, enemy.currentAmmo);
    }

    private bool CheckGameOver()
    {
        if (player.currentHP <= 0 || enemy.currentHP <= 0)
        {
            string msg = (player.currentHP <= 0) ? "RAKİP KAZANDI!" : "KAZANDIN!";
            
            if (isTrainingMode)
            {
                // 1. Sonuçları Beyne İlet
                if (player.GetComponent<EnemyAgent>() != null)
                    player.GetComponent<EnemyAgent>().ProcessMatchResult(player.currentHP > 0);
                
                if (enemy.GetComponent<EnemyAgent>() != null)
                    enemy.GetComponent<EnemyAgent>().ProcessMatchResult(enemy.currentHP > 0);

                // 2. Oyunu Sıfırla (Döngü Devam Etsin)
                ResetGame();

                // 3. Hız Ayarı
                Time.timeScale = 100.0f; 
            }
            else 
            {
                if(useTrainedAI && enemy.GetComponent<EnemyAgent>() != null)
                {
                    enemy.GetComponent<EnemyAgent>().ProcessMatchResult(enemy.currentHP > 0);
                }

                uiManager.UpdateBattleLog(msg);
                uiManager.ShowEndGamePanel(msg);
            }
            return true;
        }
        return false;
    }

    private void ResetGame()
    {
        player.currentHP = player.maxHP;
        player.currentMana = player.startMana;
        player.currentAmmo = player.maxAmmo;
        player.armorUpActive = false;

        enemy.currentHP = enemy.maxHP;
        enemy.currentMana = enemy.startMana;
        enemy.currentAmmo = enemy.maxAmmo;
        enemy.armorUpActive = false;

        isPlayerTurn = true;
        turnCount = 0;

        InitPositions();
        UpdateDistanceState();
        UpdateUI();

        StartTurn();
    }
}