using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public Gladiator player;
    public Gladiator enemy;

    private IEnumerator EndPlayerTurnWithDelay()
    {
        GameManager.Instance.uiManager.UpdateActionButtonsInteractable(false);
        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.EndPlayerTurn();
    }

    private void LockPlayerTurn()
    {
        GameManager.Instance.isPlayerTurn = false;
        GameManager.Instance.uiManager.UpdateActionButtonsInteractable(false);
    }

    public void OnMoveForward()
    {
        if (!GameManager.Instance.isPlayerTurn) return;
        if (!player.SpendMana(4)) return;
        GameManager.Instance.uiManager.SetTurnText("OYUNCU: İLERİ GİTTİ");
        LockPlayerTurn();
        GameManager.Instance.MoveCloser(true);
        StartCoroutine(EndPlayerTurnWithDelay());
    }

    public void OnMoveBackward()
    {
        if (!GameManager.Instance.isPlayerTurn) return;
        if (!player.SpendMana(4)) return;
        GameManager.Instance.uiManager.SetTurnText("OYUNCU: GERİ ÇEKİLDİ");
        LockPlayerTurn();
        GameManager.Instance.MoveAway(true);
        StartCoroutine(EndPlayerTurnWithDelay());
    }

    public void OnRangedAttack()
    {
        if (!GameManager.Instance.isPlayerTurn) return;
        if (GameManager.Instance.currentDistance == DistanceLevel.Close) return;
        if (player.currentAmmo <= 0 || !player.SpendMana(12)) return;

        GameManager.Instance.uiManager.SetTurnText("OYUNCU: OK ATTI! (15-20 Hsr)");
        LockPlayerTurn();
        player.currentAmmo--;
        player.ShootProjectile("Enemy", Random.Range(15, 21)); 
        StartCoroutine(EndPlayerTurnWithDelay());
    }

    public void OnMeleeButton()
    {
        if (!GameManager.Instance.isPlayerTurn) return;
        if (GameManager.Instance.currentDistance != DistanceLevel.Close) return;
        if (!player.SpendMana(20)) return;

        GameManager.Instance.uiManager.SetTurnText("OYUNCU: KILIÇ VURDU! (20-30 Hsr)");
        LockPlayerTurn();
        player.TriggerAttack();
        int damage = Random.Range(20, 31); 
        if(enemy != null) enemy.TakeDamage(damage);
        else GameManager.Instance.enemy.TakeDamage(damage);
        StartCoroutine(EndPlayerTurnWithDelay());
    }

    public void OnSleep()
    {
        if (!GameManager.Instance.isPlayerTurn) return;
        GameManager.Instance.uiManager.SetTurnText("OYUNCU: DİNLENİYOR (+MANA)");
        LockPlayerTurn();
        player.RestoreMana(40); // Sadece mana yeniler
        StartCoroutine(EndPlayerTurnWithDelay());
    }

    public void OnArmorUp()
    {
        if (!GameManager.Instance.isPlayerTurn) return;
        if (!player.SpendMana(15)) return;
        GameManager.Instance.uiManager.SetTurnText("OYUNCU: KALKAN KALDIRDI");
        LockPlayerTurn();
        player.ActivateArmorUp(2);
        StartCoroutine(EndPlayerTurnWithDelay());
    }
}