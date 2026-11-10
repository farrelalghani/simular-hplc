using UnityEngine;

// Menggambar kurva kromatogram (dua puncak Gaussian) langsung ke Texture2D dari angka
// tR1, tR2, W1, W2 -- menggantikan pemakaian gambar PNG statis, supaya jumlah kombinasi
// sampel x fase x skenario fase gerak tidak lagi dibatasi jumlah aset gambar yang dibuat manual.
// Nama sumbu/angka/keterangan ditempel terpisah sebagai teks TMP oleh KalkulasiRs.cs
// (lihat HitungWaktuMaks di bawah, dipakai bersama supaya posisinya presisi nyambung
// dengan gambar kurva ini).
public static class GeneratorKromatogram
{
    public const int Lebar = 512;
    public const int Tinggi = 256;
    public const int MarginBawah = 28;
    public const int MarginKiri = 8;

    static readonly Color WarnaLatar   = new Color(0.075f, 0.122f, 0.2f, 1f);   // sama seperti kartu UI gelap
    static readonly Color WarnaSumbu   = new Color(1f, 1f, 1f, 0.35f);
    static readonly Color WarnaTick    = new Color(1f, 1f, 1f, 0.55f);
    static readonly Color WarnaPuncak1 = new Color(34f / 255f, 197f / 255f, 94f / 255f, 1f);  // hijau, komponen 1 (tR1)
    static readonly Color WarnaPuncak2 = new Color(239f / 255f, 68f / 255f, 68f / 255f, 1f);  // merah, komponen 2 (tR2)

    // Rentang waktu (menit) yang digambar di sumbu X, dari 0 sampai nilai ini.
    // Dipakai bersama oleh Buat() dan oleh KalkulasiRs.cs saat menempatkan label angka,
    // supaya posisi tick di gambar dan posisi label teks selalu presisi nyambung.
    public static float HitungWaktuMaks(float tR1, float tR2, float W1, float W2)
    {
        return Mathf.Max(tR1, tR2) + Mathf.Max(W1, W2) * 2f + 0.5f;
    }

    public static Texture2D Buat(float tR1, float tR2, float W1, float W2)
    {
        var tex = new Texture2D(Lebar, Tinggi, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;

        var pixels = new Color[Lebar * Tinggi];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = WarnaLatar;

        // sumbu X (waktu retensi), garis horizontal tipis
        for (int x = MarginKiri; x < Lebar; x++)
            pixels[MarginBawah * Lebar + x] = WarnaSumbu;
        // sumbu Y (absorbansi), garis vertikal tipis di kiri
        for (int y = 0; y < Tinggi; y++)
            pixels[y * Lebar + MarginKiri] = WarnaSumbu;

        float waktuMaks = HitungWaktuMaks(tR1, tR2, W1, W2);
        int tinggiGrafik = Tinggi - MarginBawah - 10;
        int lebarGrafik  = Lebar - MarginKiri;

        // tick tiap 1 menit di sumbu X (garis pendek menonjol ke bawah sumbu)
        int menitMaks = Mathf.FloorToInt(waktuMaks);
        for (int menit = 0; menit <= menitMaks; menit++)
        {
            int tx = MarginKiri + Mathf.RoundToInt((menit / waktuMaks) * lebarGrafik);
            if (tx >= Lebar) continue;
            for (int dy = 1; dy <= 5; dy++)
            {
                int ty = MarginBawah - dy;
                if (ty < 0) break;
                pixels[ty * Lebar + tx] = WarnaTick;
            }
        }

        // tick 25/50/75/100% tinggi di sumbu Y (garis pendek menonjol ke kiri sumbu)
        for (int frac = 1; frac <= 4; frac++)
        {
            int ty = MarginBawah + Mathf.RoundToInt((frac / 4f) * tinggiGrafik);
            if (ty >= Tinggi) continue;
            for (int dx = 1; dx <= 4; dx++)
            {
                int tx = MarginKiri - dx;
                if (tx < 0) break;
                pixels[ty * Lebar + tx] = WarnaTick;
            }
        }

        float sigma1 = Mathf.Max(W1 / 4f, 0.03f);
        float sigma2 = Mathf.Max(W2 / 4f, 0.03f);

        for (int x = MarginKiri + 1; x < Lebar; x++)
        {
            float t = ((x - MarginKiri) / (float)lebarGrafik) * waktuMaks;

            float g1 = Mathf.Exp(-Mathf.Pow(t - tR1, 2) / (2f * sigma1 * sigma1));
            float g2 = Mathf.Exp(-Mathf.Pow(t - tR2, 2) / (2f * sigma2 * sigma2));

            int tinggi1 = Mathf.RoundToInt(g1 * tinggiGrafik);
            int tinggi2 = Mathf.RoundToInt(g2 * tinggiGrafik);
            int tinggiMaksKolom = Mathf.Max(tinggi1, tinggi2);

            for (int y = 1; y <= tinggiMaksKolom; y++)
            {
                int py = MarginBawah + y;
                if (py >= Tinggi) break;

                bool dalam1 = y <= tinggi1;
                bool dalam2 = y <= tinggi2;

                Color warna;
                if (dalam1 && dalam2) warna = Color.Lerp(WarnaPuncak1, WarnaPuncak2, 0.5f);
                else if (dalam1) warna = WarnaPuncak1;
                else warna = WarnaPuncak2;

                pixels[py * Lebar + x] = warna;
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }
}