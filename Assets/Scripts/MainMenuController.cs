using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("UI References")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Audio")]
    public AudioSource menuMusicSource; // Menüdeki "MenuMusic" objesini buraya bağlayacağız

    private void Start()
    {
        // 1. Kayıtlı ses ayarlarını yükle (Yoksa 0.5 yap)
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float sfxVol   = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        // 2. Sliderları ayarla
        if (musicSlider != null) musicSlider.value = musicVol;
        if (sfxSlider != null)   sfxSlider.value   = sfxVol;

        // 3. Menü müziğinin sesini ayarla
        if (menuMusicSource != null)
        {
            menuMusicSource.volume = musicVol;
        }
    }


    // 1. "Rastgele Başla" butonu (Btn_RandomStart) buna bağlanacak.
    // Bu butona basınca AI kapalı gider
    public void OnStartRandomAI()
    {
        Debug.Log("Oyun Başlatılıyor: Mod -> RASTGELE (Eğitimsiz)");
        
        // GameManager'a "Sakın beyni kullanma" diyoruz
        GameManager.useTrainedAI = false; 
        
        // Oyun sahnesini (Index 1) yüklüyoruz
        SceneManager.LoadScene(1);
    }

    // 2. "Yapay Zeka Yükle" butonu (Btn_LoadAI) buna bağlanacak.
    // Bu butona basınca AI açık gider, düşman akıllı oynar.
    public void OnStartTrainedAI()
    {
        Debug.Log("Oyun Başlatılıyor: Mod -> EĞİTİLMİŞ (Akıllı)");
        
        // GameManager'a "Eğittiğimiz JSON dosyasını kullan" diyoruz
        GameManager.useTrainedAI = true; 
        
        // Oyun sahnesini (Index 1) yüklüyoruz
        SceneManager.LoadScene(1);
    }

    // (Eski buton fonksiyonu - Artık kullanmana gerek yok ama hata vermesin diye dursun)
    public void OnNewGameClicked()
    {
        OnStartRandomAI(); 
    }

    // SES AYARLARI
    

    public void OnMusicSliderChanged(float value)
    {
        // Değeri kaydet
        PlayerPrefs.SetFloat("MusicVolume", value);
        
        // Anlık olarak menü müziğini değiştir (Duyarak test etmek için)
        if (menuMusicSource != null)
        {
            menuMusicSource.volume = value;
        }
    }

    public void OnSFXSliderChanged(float value)
    {
        // Sadece kaydet (Efekt sesi menüde çalmadığı için burada duyulmaz)
        PlayerPrefs.SetFloat("SFXVolume", value);
    }
}