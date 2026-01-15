# Gladiator Duel AI - Q-Learning Project

Bu proje, Unity oyun motoru kullanılarak geliştirilmiş ve düşman karakteri için **Reinforcement Learning (Pekiştirmeli Öğrenme)** yöntemlerinden **Q-Learning** algoritması entegre edilmiş bir gladyatör dövüş oyunudur.

## 🎮 Oynanabilir Oyun Linki (Live Demo)
Oyunun WebGL versiyonunu tarayıcı üzerinden oynamak için aşağıdaki bağlantıya tıklayın:
👉 **[Oymak İçin Tıkla: Gladiator Duel (Itch.io)](https://elifnurbeycan.itch.io/gladiator-duel)**

---

## 🧠 Yapay Zeka Entegrasyonu (Q-Learning)

Oyunun ikinci aşamasında, kural tabanlı (If-Else) yapı yerine, kendi kendine öğrenen ve tecrübe kazanan bir **Q-Learning Ajanı** geliştirilmiştir. Ajan, çevreden aldığı ödül ve cezalara göre aksiyon almayı öğrenmiştir.

### 🎯 Ödül ve Ceza Sistemi (Reward & Penalty System)
Ajanın doğru davranışları pekiştirmesi ve hatalı davranışlardan kaçınması için aşağıdaki puanlama sistemi kullanılarak eğitim gerçekleştirilmiştir:

* **Rakibe Hasar Verme (Başarılı Saldırı):** `+15 Puan` (Saldırganlığı teşvik etmek için)
* **Oyunu Kazanma (Rakibi Öldürme):** `+100 Puan` (Ana hedef)
* **Hasar Alma (Darbe Yeme):** `-20 Puan` (Savunmayı ve kaçınmayı öğrenmesi için)
* **Boşa Saldırı (Iska Geçme):** `-2 Puan` (Rastgele saldırı spamlamasını engellemek için)
* **Oyunu Kaybetme:** `-100 Puan` (Hayatta kalmayı önceliklendirmesi için)
* **Rakibe Yaklaşma:** `+0.5 Puan` (Pasif kalmayıp oyuna dahil olması için)

### ⚙️ Hiperparametreler (Hyperparameters)
Eğitimin daha stabil olması ve ajanın optimum stratejiyi bulabilmesi için Q-Learning değerleri aşağıdaki gibi güncellenmiştir:

* **Learning Rate (Alpha):** `0.1` (Yeni bilgilerin eski bilgilerin üzerine ne kadar yazılacağını belirler)
* **Discount Factor (Gamma):** `0.9` (Gelecekteki ödüllerin şimdiki kararlar üzerindeki etkisi)
* **Exploration Rate (Epsilon):** `0.1` (Ajanın rastgele keşif yapma ihtimali. Eğitim ilerledikçe bu oran düşürülmüştür.)

### 📂 Model Dosyası
Eğitilen yapay zeka verileri (Q-Table), oyun içerisinde `Resources/BlueBrain.json` dosyasında tutulmaktadır. WebGL sürümünde oyun başladığında bu hafıza otomatik olarak yüklenir ve ajan "Akıllı Mod"da başlar.

---

## 🕹️ Nasıl Oynanır?

* **Hareket:** A ve D Tuşları (Veya Yön Tuşları)
* **Saldırı:** Space (Boşluk) Tuşu
* **Amaç:** Yapay zeka kontrollü rakibin canını sıfıra indirerek arenadan galip ayrılmak.

---

**Geliştirici:** Elif Nur Beycan
**Ders:** Bilgisayar Mühendisliği Oyun Programlama Projesi
