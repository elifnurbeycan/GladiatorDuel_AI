# ⚔️ Gladiator Duel AI - Q-Learning Project

> **Bu proje, Unity Oyun Motoru kullanılarak geliştirilmiş, 2. aşamasında Q-Learning (Reinforcement Learning) yapay zeka algoritması entegre edilmiş sıra tabanlı bir strateji oyunudur.**

🎮 **Tarayıcıda Oyna (WebGL):** https://elifnurbeycan.itch.io/gladiator-duel

---

## 📸 Oyun İçi Görseller


<img width="957" height="597" alt="Oyun Ekranı" src="https://github.com/user-attachments/assets/04f3d776-8472-49c5-9ace-be3454e1ed6d" />

---

## 🧠 Yapay Zeka (Q-Learning) Mimarisi

Projenin bu aşamasında, düşman karakteri (`EnemyAgent.cs`) önceden tanımlanmış kurallar yerine, çevresini gözlemleyerek ve deneme-yanılma yoluyla öğrenen bir **Q-Learning Ajanına** dönüştürülmüştür.

### 🎯 Ödül ve Ceza Tablosu (Reward System)
Ajanın eğitimi sırasında davranışlarını şekillendirmek için kod içerisinde (`EnemyAgent.cs`) aşağıdaki ödül/ceza mekanizması kurulmuştur:

| Durum / Aksiyon | Puan (Ödül/Ceza) | Amaç |
| :--- | :--- | :--- |
| **🏆 Maçı Kazanma** | `+50 Puan` | Ana hedefi gerçekleştirmek. |
| **💀 Maçı Kaybetme** | `-20 Puan` | Hayatta kalmayı teşvik etmek. |
| **⚔️ Rakibe Hasar Verme** | `+Hasar Miktarı` | Saldırganlığı artırmak (Örn: 20 hasar = +20 Puan). |
| **🏹 Uzaktan Saldırı (Ranged)** | `+10 Puan` | Mermisi varken uzaktan dövüşü teşvik etmek. |
| **🚫 İmkansız Hamle** | `-50 Puan` | Manası yetmediği veya mesafesi yetmediği halde hamle yapmaya çalışırsa ağır ceza alır. |
| **🧱 Duvara Çarpma** | `-20 Puan` | Harita sınırındayken daha fazla geri gitmeye çalışmasını engellemek. |
| **🏃 Rakibe Yaklaşma** | `-5 Puan (Yakınken)` | Zaten "Close" mesafedeyken üzerine yürümeye çalışmasını engellemek. |
| **💤 Mantıklı Dinlenme** | `+10 Puan` | Manası azaldığında (`<20`) dinlenmeyi öğrenmesi için teşvik. |

### ⚙️ Hiperparametreler (Hyperparameters)
Eğitim sürecinde aşağıdaki Q-Learning parametreleri kullanılmıştır:

* **Learning Rate (Alpha):** `0.5` (Yeni tecrübelerin, eski bilgilerin üzerine yazılma hızı.)
* **Discount Factor (Gamma):** `0.8` (Gelecekteki ödüllerin şimdiki karara etkisi.)
* **Exploration Rate (Epsilon):** `0.1` (Eğitilmiş modda rastgele hareket etme ihtimali minimuma indirilmiştir.)

### 💾 Model Yönetimi
Eğitilen veriler (Q-Table), oyun içerisinde **`Resources/BlueBrain.json`** dosyasında saklanmaktadır. WebGL sürümünde oyun, bu dosyayı otomatik olarak belleğe yükler ve ajan eğitimli verilerle oynar.

---

## 🕹️ Oynanış ve Kontrol Mekaniği

Oyun, butonlar aracılığıyla sıra tabanlı (Turn-Based) olarak oynanır.

* **Kontroller:** Ekrandaki butonlara tıklayarak aksiyon seçimi yapılır.
* **Menü:** `ESC` tuşu ile ana menüye dönülebilir.

### 🎮 Aksiyon Listesi ve Maliyetler

Her karakterin **Can (HP)**, **Mana** ve **Mermi (Ammo)** kaynakları vardır. Kod içerisinde tanımlı maliyetler şöyledir:

| Aksiyon | Gereksinim (Cost) | Etki / Detay |
| :--- | :--- | :--- |
| **Move (İleri/Geri)** | `4 Mana` | Mesafeyi (Close/Mid/Far) değiştirir. |
| **Ranged Attack** | `12 Mana` + `1 Ammo` | 15-20 arası hasar verir. *(Sadece Mid/Far mesafede)* |
| **Melee Attack** | `20 Mana` | 20-30 arası yüksek hasar verir. *(Sadece Close mesafede)* |
| **Armor Up** | `15 Mana` | 2 tur boyunca alınan hasarı %30 azaltır. |
| **Sleep (Dinlen)** | `0 Mana` | Turu pas geçer, `+40 Mana` yeniler. |

---

## 🛠️ Kurulum ve Test

Projeyi Unity Editör'de açmak veya test etmek için:

1.  `Scenes/MainMenu` sahnesini açın.
2.  Play tuşuna basın.
3.  **"Yapay Zeka Yükle"** butonuna basarak eğitilmiş Q-Learning modeli ile oynayın.
4.  **"Rastgele Başla"** butonu, ajanın beynini devre dışı bırakır ve tamamen rastgele oynamasını sağlar (AI farkını görmek için).

---

**Geliştirici:** Elif Nur Beycan
**Ders:** Bilgisayar Mühendisliği - Oyun Programlama Projesi
