# YMH219-FlappyBird

Kısa Açıklama: Windows Forms üzerinde zamanlayıcı (`Timer`), GDI+ çizimleri ve klavye/fare olaylarının kullanıldığı Flappy Bird benzeri masaüstü oyunu.

## Kullanılan Teknolojiler

- C# / .NET Windows Forms
- System.Drawing (GDI+ 2D Grafik Çizimi)

## Kurulum ve Çalıştırma Adımları

1. Bu depoyu yerel bilgisayarınıza klonlayın:
   `git clone https://github.com/250542008-sanat/YMH219-FlappyBird.git`
2. Proje klasörüne girin ve `Flappy Bird.csproj` dosyasını Visual Studio ile açın.
3. Üst menüden **Başlat (Start)** butonuna basarak veya `F5` tuşu ile oyunu derleyip çalıştırın.
4. `Space`, `Yukarı Ok` tuşuna basarak veya fare sol tuşuna tıklayarak kuşu zıplatın ve boruların arasından geçin.

## Uygulamanın Temel Özellikleri

- **Oyun Döngüsü ve Fizik:** `System.Windows.Forms.Timer` (~50 FPS) ile yerçekimi ve zıplama mekaniği.
- **Özel GDI+ Çizimi:** Harici resim dosyasına ihtiyaç duymadan `Graphics` ve `DoubleBuffered` ile titreşimsiz kuş, boru, bulut ve zemin çizimi.
- **Hareketli Engeller ve Çarpışma Kontrolü:** Rastgele yüksekliklerde sürekli üretilen borular ve `RectangleF.IntersectsWith` ile boru/zemin çarpışma tespiti.
- **Skor Takibi ve Yeniden Başlatma:** Anlık skor ile oturum içi en yüksek skor (High Score) takibi; oyun bittiğinde `Space`, `R` veya fare tıklamasıyla anında yeniden başlatma.

## Ekran Görüntüleri

<img width="1470" height="956" alt="Ekran Resmi 2026-11-25 15 18 30" src="https://github.com/user-attachments/assets/f080c49a-cfd3-46b9-8e75-a528213c2149" />
<img width="1470" height="956" alt="Ekran Resmi 2026-11-25 15 18 27" src="https://github.com/user-attachments/assets/704e0257-823f-4995-8ae3-6af540c6a061" />
<img width="1470" height="956" alt="Ekran Resmi 2026-11-25 15 18 05" src="https://github.com/user-attachments/assets/a6061399-be45-47df-91b7-83269a481902" />


## Bilinen Sınırlamalar / Geliştirme Notları

- Oyun penceresi sabit boyuttadır (`460x620`), tam ekran veya yeniden boyutlandırma desteklenmemektedir.
- En yüksek skor (High Score) yalnızca uygulama açık olduğu sürece bellekte tutulur; program kapatıldığında sıfırlanır.
- İlerleyen sürümlerde ses efektleri, skor arttıkça hızlanan zorluk seviyesi ve kalıcı skor kaydı eklenebilir.

## Geliştirici

Ad-Soyad: Mustafa Şahingöz  
Öğrenci Numarası: 250542008
