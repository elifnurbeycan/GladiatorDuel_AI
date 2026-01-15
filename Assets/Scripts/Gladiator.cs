using UnityEngine;

public class Gladiator : MonoBehaviour
{
    [Header("Animation")]
    public Animator animator; 

    [Header("Base Stats")]
    public int maxHP = 100;
    public int currentHP;

    public int maxMana = 120;
    public int startMana = 80;
    public int currentMana;

    [Header("Ranged")]
    public int maxAmmo = 10;
    public int currentAmmo;

    [Header("Armor Up")]
    public bool armorUpActive = false;
    public int armorUpTurnsRemaining = 0; 

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip attackSound;
    public AudioClip hitSound;
    public AudioClip walkSound;

    [Header("Projectile Settings")]
    public GameObject arrowPrefab;
    public Transform firePoint;

    private void Awake()
    {
        currentHP = maxHP;
        currentMana = Mathf.Clamp(startMana, 0, maxMana);
        currentAmmo = maxAmmo;
    }

    private void Start()
    {
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        if (audioSource != null) audioSource.volume = sfxVol;
    }

    //  ATIŞ MANTIĞI 
    public void ShootProjectile(string targetTag, float damage)
    {
        // 1. EĞİTİM MODU 
        // Fizik motorunu devre dışı bırak, direkt hasar ver.
        // Bu sayede oyun hızı 50x olsa bile oklar içinden geçip gitmez
        if (GameManager.Instance.isTrainingMode)
        {
            GameObject target = GameObject.FindGameObjectWithTag(targetTag);
            if (target != null)
            {
                Gladiator targetGladiator = target.GetComponent<Gladiator>();
                if (targetGladiator != null)
                {
                    targetGladiator.TakeDamage((int)damage);
                }
            }
            // Nesne yaratmadığımız için Destroy etmeye gerek yok. RAM dostu.
            return; 
        }

        // 2. NORMAL OYUN MODU 
        // Görsellik önemli, oku fiziksel olarak yarat.
        GameObject projectile = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        GameObject normalTarget = GameObject.FindGameObjectWithTag(targetTag);

        if (normalTarget != null)
        {
            Vector2 direction = (normalTarget.transform.position - firePoint.position).normalized;
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            
            // Ok hızı (Görsel)
            if (rb != null) rb.linearVelocity = direction * 15f; 

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            Projectile projScript = projectile.GetComponent<Projectile>();
            if (projScript != null)
            {
                projScript.damage = (int)damage; 
                projScript.targetTag = targetTag; 
            }
        }
        else
        {
            // Hedef yoksa düz fırlat (Debug amaçlı)
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                 if (transform.localScale.x < 0) rb.linearVelocity = Vector2.left * 15f;
                 else rb.linearVelocity = Vector2.right * 15f;
            }
        }

        // RAM KORUMASI: Ok bir yere çarpmazsa 5 saniye sonra yok olsun.
        Destroy(projectile, 5.0f);
    }

    public void SetMoveAnimation(bool isMoving)
    {
        if (animator != null) animator.SetBool("IsMoving", isMoving);
    }

    public void ToggleWalkSound(bool isWalking)
    {
        if (audioSource == null || walkSound == null) return;
        if (isWalking)
        {
            if (!audioSource.isPlaying || audioSource.clip != walkSound)
            {
                audioSource.clip = walkSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.clip == walkSound)
            {
                audioSource.Stop();
                audioSource.loop = false;
            }
        }
    }

    public void TriggerAttack()
    {
        if (animator != null) animator.SetTrigger("Attack");
        if (audioSource != null && attackSound != null) audioSource.PlayOneShot(attackSound);
    }

    public void TakeDamage(int amount)
    {
        if (currentHP <= 0) return;

        float finalDamage = amount;

        if (armorUpActive)
        {
            // %30 HASAR AZALTMA
            finalDamage *= 0.7f; 
        }

        currentHP -= Mathf.RoundToInt(finalDamage);
        if (currentHP < 0) currentHP = 0;

        if (animator != null)
        {
            if (currentHP <= 0) animator.SetTrigger("Death");
            else
            {
                animator.SetTrigger("Hit");
                if (audioSource != null && hitSound != null) audioSource.PlayOneShot(hitSound);
            }
        }
    }

    public bool SpendMana(int amount)
    {
        if (currentMana < amount) return false;
        currentMana -= amount;
        return true;
    }

    public void RestoreMana(int amount)
    {
        currentMana += amount;
        if (currentMana > maxMana) currentMana = maxMana;
    }

    public void RestoreHP(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP) currentHP = maxHP;
    }

    public void ActivateArmorUp(int turns)
    {
        armorUpActive = true;
        armorUpTurnsRemaining = turns;
    }

    public void OnTurnEnd()
    {
        if (armorUpActive)
        {
            armorUpTurnsRemaining--;
            if (armorUpTurnsRemaining <= 0) armorUpActive = false;
        }
    }
}