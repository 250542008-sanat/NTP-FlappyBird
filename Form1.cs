using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace  Flappy_Bird
{
    public partial class Form1 : Form
    {
        // Oyun döngüsü ve rastgelelik
        private System.Windows.Forms.Timer oyunZamanlayici = null!;
        private readonly Random rastgele = new Random();

        // Kuþ fizik deðiþkenleri
        private float kusX = 110;
        private float kusY = 250;
        private float dikeyHiz = 0;
        private const int KusBoyut = 34;
        private const float YerCekimi = 0.65f;
        private const float ZiplamaGucu = -9.5f;

        // Boru (Engel) yapýsý
        private class BoruCifti
        {
            public float X;
            public int UstBoruYukseklik;
            public bool PuanVerildi;
        }

        private readonly List<BoruCifti> borular = new List<BoruCifti>();
        private const int BoruGenislik = 65;
        private const int BoruBosluk = 155;
        private const float BoruHizi = 3.8f;
        private const int ZeminYukseklik = 75;

        // Oyun durumu ve skor
        private int skor = 0;
        private int enYuksekSkor = 0;
        private bool oyunBasladi = false;
        private bool oyunBitti = false;

        public Form1()
        {
            InitializeComponent();
            ArayuzuKur();
        }

        private void ArayuzuKur()
        {
            this.Text = "YMH219 - Flappy Bird";
            this.ClientSize = new Size(460, 620);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.DoubleBuffered = true; // Titremeyi (flicker) önler
            this.KeyPreview = true;

            this.KeyDown += Form1_KeyDown;
            this.MouseDown += Form1_MouseDown;

            oyunZamanlayici = new System.Windows.Forms.Timer();
            oyunZamanlayici.Interval = 20; // ~50 FPS
            oyunZamanlayici.Tick += OyunDongusu;

            OyunuSifirla();
        }

        private void OyunuSifirla()
        {
            kusY = (this.ClientSize.Height - ZeminYukseklik) / 2f;
            dikeyHiz = 0;
            skor = 0;
            oyunBasladi = false;
            oyunBitti = false;

            borular.Clear();
            // Ýlk 3 boru çiftini baþlangýçta sýraya diz
            for (int i = 0; i < 3; i++)
            {
                borular.Add(new BoruCifti
                {
                    X = this.ClientSize.Width + 120 + (i * 230),
                    UstBoruYukseklik = rastgele.Next(70, this.ClientSize.Height - ZeminYukseklik - BoruBosluk - 70),
                    PuanVerildi = false
                });
            }

            oyunZamanlayici.Stop();
            this.Invalidate();
        }

        private void Zipla()
        {
            if (oyunBitti)
            {
                OyunuSifirla();
                return;
            }

            if (!oyunBasladi)
            {
                oyunBasladi = true;
                oyunZamanlayici.Start();
            }

            dikeyHiz = ZiplamaGucu;
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Up)
            {
                e.SuppressKeyPress = true;
                Zipla();
            }
            else if (e.KeyCode == Keys.R && oyunBitti)
            {
                OyunuSifirla();
            }
        }

        private void Form1_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Zipla();
            }
        }

        private void OyunDongusu(object? sender, EventArgs e)
        {
            // Kuþ fiziði
            dikeyHiz += YerCekimi;
            kusY += dikeyHiz;

            int oyunAlaniAlt = this.ClientSize.Height - ZeminYukseklik;

            // Tavan veya zemin çarpýþmasý
            if (kusY < 0)
            {
                kusY = 0;
                dikeyHiz = 0;
            }
            if (kusY + KusBoyut >= oyunAlaniAlt)
            {
                kusY = oyunAlaniAlt - KusBoyut;
                OyunuBitir();
                return;
            }

            RectangleF kusKutusu = new RectangleF(kusX + 3, kusY + 3, KusBoyut - 6, KusBoyut - 6);

            // Borularý hareket ettir ve çarpýþma kontrolü yap
            for (int i = 0; i < borular.Count; i++)
            {
                BoruCifti boru = borular[i];
                boru.X -= BoruHizi;

                // Ekrandan çýkan boruyu saða yeni yükseklikle taþý
                if (boru.X + BoruGenislik < 0)
                {
                    float enSagX = 0;
                    foreach (var b in borular)
                        if (b.X > enSagX) enSagX = b.X;

                    boru.X = enSagX + 230;
                    boru.UstBoruYukseklik = rastgele.Next(70, oyunAlaniAlt - BoruBosluk - 70);
                    boru.PuanVerildi = false;
                }

                // Skor kontrolü
                if (!boru.PuanVerildi && boru.X + BoruGenislik < kusX)
                {
                    boru.PuanVerildi = true;
                    skor++;
                    if (skor > enYuksekSkor) enYuksekSkor = skor;
                }

                // Üst ve alt boru çarpýþma alanlarý
                RectangleF ustBoruAlani = new RectangleF(boru.X, 0, BoruGenislik, boru.UstBoruYukseklik);
                float altBoruY = boru.UstBoruYukseklik + BoruBosluk;
                RectangleF altBoruAlani = new RectangleF(boru.X, altBoruY, BoruGenislik, oyunAlaniAlt - altBoruY);

                if (kusKutusu.IntersectsWith(ustBoruAlani) || kusKutusu.IntersectsWith(altBoruAlani))
                {
                    OyunuBitir();
                    return;
                }
            }

            this.Invalidate();
        }

        private void OyunuBitir()
        {
            oyunBitti = true;
            oyunZamanlayici.Stop();
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int w = this.ClientSize.Width;
            int h = this.ClientSize.Height;
            int oyunAlaniAlt = h - ZeminYukseklik;

            // 1. Gökyüzü Arka Planý
            using (LinearGradientBrush gokyuzu = new LinearGradientBrush(
                new Rectangle(0, 0, w, oyunAlaniAlt),
                Color.FromArgb(112, 197, 206),
                Color.FromArgb(180, 232, 238),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(gokyuzu, 0, 0, w, oyunAlaniAlt);
            }

            // Dekoratif bulutlar
            using (SolidBrush bulutFirca = new SolidBrush(Color.FromArgb(180, 255, 255, 255)))
            {
                g.FillEllipse(bulutFirca, 40, 80, 90, 40);
                g.FillEllipse(bulutFirca, 75, 65, 70, 45);
                g.FillEllipse(bulutFirca, 280, 120, 110, 45);
                g.FillEllipse(bulutFirca, 320, 105, 80, 45);
            }

            // 2. Borular
            using (SolidBrush boruGovde = new SolidBrush(Color.FromArgb(115, 191, 46)))
            using (SolidBrush boruAcik = new SolidBrush(Color.FromArgb(156, 229, 89)))
            using (Pen boruKenar = new Pen(Color.FromArgb(45, 75, 20), 2.5f))
            {
                foreach (var boru in borular)
                {
                    // Üst boru
                    g.FillRectangle(boruGovde, boru.X, 0, BoruGenislik, boru.UstBoruYukseklik);
                    g.FillRectangle(boruAcik, boru.X + 6, 0, 10, boru.UstBoruYukseklik);
                    g.DrawRectangle(boruKenar, boru.X, -2, BoruGenislik, boru.UstBoruYukseklik + 2);

                    // Üst boru baþlýðý
                    g.FillRectangle(boruGovde, boru.X - 4, boru.UstBoruYukseklik - 24, BoruGenislik + 8, 24);
                    g.DrawRectangle(boruKenar, boru.X - 4, boru.UstBoruYukseklik - 24, BoruGenislik + 8, 24);

                    // Alt boru
                    float altY = boru.UstBoruYukseklik + BoruBosluk;
                    float altYukseklik = oyunAlaniAlt - altY;
                    g.FillRectangle(boruGovde, boru.X, altY, BoruGenislik, altYukseklik);
                    g.FillRectangle(boruAcik, boru.X + 6, altY, 10, altYukseklik);
                    g.DrawRectangle(boruKenar, boru.X, altY, BoruGenislik, altYukseklik);

                    // Alt boru baþlýðý
                    g.FillRectangle(boruGovde, boru.X - 4, altY, BoruGenislik + 8, 24);
                    g.DrawRectangle(boruKenar, boru.X - 4, altY, BoruGenislik + 8, 24);
                }
            }

            // 3. Zemin
            using (SolidBrush cimFirca = new SolidBrush(Color.FromArgb(115, 191, 46)))
            using (SolidBrush toprakFirca = new SolidBrush(Color.FromArgb(222, 216, 149)))
            using (Pen zeminCizgi = new Pen(Color.FromArgb(84, 56, 71), 3))
            {
                g.FillRectangle(toprakFirca, 0, oyunAlaniAlt, w, ZeminYukseklik);
                g.FillRectangle(cimFirca, 0, oyunAlaniAlt, w, 16);
                g.DrawLine(zeminCizgi, 0, oyunAlaniAlt, w, oyunAlaniAlt);
            }

            // 4. Kuþ Çizimi
            using (SolidBrush kusGovde = new SolidBrush(Color.FromArgb(255, 204, 0)))
            using (SolidBrush kusKanat = new SolidBrush(Color.FromArgb(255, 240, 150)))
            using (SolidBrush gagaFirca = new SolidBrush(Color.FromArgb(242, 92, 42)))
            using (Pen siyahKenar = new Pen(Color.FromArgb(60, 40, 20), 2))
            {
                // Gövde
                g.FillEllipse(kusGovde, kusX, kusY, KusBoyut, KusBoyut);
                g.DrawEllipse(siyahKenar, kusX, kusY, KusBoyut, KusBoyut);

                // Kanat
                g.FillEllipse(kusKanat, kusX + 4, kusY + 14, 16, 11);
                g.DrawEllipse(siyahKenar, kusX + 4, kusY + 14, 16, 11);

                // Göz
                g.FillEllipse(Brushes.White, kusX + 19, kusY + 6, 10, 10);
                g.DrawEllipse(siyahKenar, kusX + 19, kusY + 6, 10, 10);
                g.FillEllipse(Brushes.Black, kusX + 24, kusY + 9, 4, 4);

                // Gaga
                PointF[] gaga =
                {
                    new PointF(kusX + KusBoyut - 2, kusY + 14),
                    new PointF(kusX + KusBoyut + 10, kusY + 19),
                    new PointF(kusX + KusBoyut - 2, kusY + 24)
                };
                g.FillPolygon(gagaFirca, gaga);
                g.DrawPolygon(siyahKenar, gaga);
            }

            // 5. Skor Yazýsý (Üst Orta)
            using (Font skorFont = new Font("Segoe UI", 28, FontStyle.Bold))
            using (StringFormat ortala = new StringFormat() { Alignment = StringAlignment.Center })
            {
                g.DrawString(skor.ToString(), skorFont, Brushes.Black, w / 2f + 2, 22, ortala);
                g.DrawString(skor.ToString(), skorFont, Brushes.White, w / 2f, 20, ortala);
            }

            // 6. Baþlangýç veya Oyun Bitti Bilgi Ekraný
            if (!oyunBasladi && !oyunBitti)
            {
                BilgiKutusuCiz(g, w, h, "FLAPPY BIRD", "Baþlamak için SPACE veya Týkla");
            }
            else if (oyunBitti)
            {
                BilgiKutusuCiz(g, w, h, "OYUN BÝTTÝ!", $"Skor: {skor}   |   En Yüksek: {enYuksekSkor}\nTekrar oynamak için SPACE veya Týkla");
            }
        }

        private void BilgiKutusuCiz(Graphics g, int w, int h, string baslik, string altMetin)
        {
            Rectangle kutu = new Rectangle(45, h / 2 - 90, w - 90, 130);
            using (SolidBrush arka = new SolidBrush(Color.FromArgb(200, 30, 30, 30)))
            using (Pen cerceve = new Pen(Color.White, 2))
            using (Font baslikFont = new Font("Segoe UI", 18, FontStyle.Bold))
            using (Font altFont = new Font("Segoe UI", 10.5f, FontStyle.Regular))
            using (StringFormat ortala = new StringFormat() { Alignment = StringAlignment.Center })
            {
                g.FillRectangle(arka, kutu);
                g.DrawRectangle(cerceve, kutu);
                g.DrawString(baslik, baslikFont, Brushes.Gold, w / 2f, kutu.Y + 18, ortala);
                g.DrawString(altMetin, altFont, Brushes.White, w / 2f, kutu.Y + 62, ortala);
            }
        }
    }
}
