using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public Gladiator enemy;
    public Gladiator player;

    public void StartEnemyTurn()
    {
        StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator EnemyTurnRoutine()
    {
        GameManager.Instance.uiManager.SetTurnText("RAKİP DÜŞÜNÜYOR...");
        yield return new WaitForSeconds(1.0f);

        bool actionDone = false;
        int safety = 0;

        while (!actionDone && safety < 10)
        {
            safety++;
            // 0: Move, 1: Ranged, 2: Melee, 3: Sleep, 4: ArmorUp
            int choice = Random.Range(0, 5); 

            switch (choice)
            {
                case 0: actionDone = EnemyMove(); break;
                case 1: actionDone = EnemyRanged(); break;
                case 2: actionDone = EnemyMelee(); break;
                case 3: actionDone = EnemySleep(); break;
                case 4: actionDone = EnemyArmorUp(); break;
            }
            yield return null; 
        }

        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.EndEnemyTurn();
    }

    private bool EnemyMove()
    {
        if (!enemy.SpendMana(4)) return false;

        bool forward = Random.value > 0.5f;
        if (forward) 
        {
            GameManager.Instance.uiManager.SetTurnText("RAKİP: İLERİ GELDİ");
            GameManager.Instance.MoveCloser(false); 
        }
        else 
        {
            GameManager.Instance.uiManager.SetTurnText("RAKİP: GERİ KAÇTI");
            GameManager.Instance.MoveAway(false);
        }
        return true;
    }

    private bool EnemyRanged()
    {
        if (enemy.currentAmmo <= 0) return false;
        // 12 MANA
        if (!enemy.SpendMana(12)) return false; 
        if (GameManager.Instance.currentDistance == DistanceLevel.Close) return false;

        GameManager.Instance.uiManager.SetTurnText("RAKİP: OK ATTI! (15-20 Hsr)");
        enemy.currentAmmo--;
        enemy.ShootProjectile(player.tag, Random.Range(15, 21));
        return true;
    }

    private bool EnemyMelee()
    {
        if (GameManager.Instance.currentDistance != DistanceLevel.Close) return false;
        // 20 MANA
        if (!enemy.SpendMana(20)) return false;

        GameManager.Instance.uiManager.SetTurnText("RAKİP: KILIÇ VURDU! (20-30 Hsr)");
        enemy.TriggerAttack();
        // 20-30 HASAR
        player.TakeDamage(Random.Range(20, 31));
        return true;
    }

    private bool EnemySleep()
    {
        if (enemy.currentMana >= enemy.maxMana) return false;

        GameManager.Instance.uiManager.SetTurnText("RAKİP: DİNLENİYOR (++Mana)");
        // +40 MANA / +15 CAN
        enemy.RestoreMana(40);
        enemy.RestoreHP(15);
        return true;
    }

    private bool EnemyArmorUp()
    {
        // 5 MANA
        if (!enemy.SpendMana(15)) return false; 

        GameManager.Instance.uiManager.SetTurnText("RAKİP: KALKAN ALDI");
        enemy.ActivateArmorUp(2);
        return true;
    }
}