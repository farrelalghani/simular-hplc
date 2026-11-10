using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Informasi : MonoBehaviour
{
    public TextMeshProUGUI txtKonten;
    public TextMeshProUGUI txtKonteks;
    public TextMeshProUGUI txtJudulKonten;

    string kontenMenu1 =
        "<b>Sampel Uji HPLC</b>\n\n" +
        "<b>Parasetamol</b>\n" +
        "Senyawa yang bersifat lebih polar. " +
        "Pada Fase Terbalik, Parasetamol akan keluar " +
        "lebih cepat dari kolom karena kurang " +
        "berinteraksi dengan fase diam non-polar (C18).\n\n" +
        "<b>Kafein</b>\n" +
        "Senyawa yang bersifat kurang polar. " +
        "Pada Fase Terbalik, Kafein akan tertahan " +
        "lebih lama di dalam kolom karena berinteraksi " +
        "lebih kuat dengan fase diam non-polar (C18).\n\n" +
        "<b>Prinsip Dasar</b>\n" +
        "Pemilihan sampel ini sesuai dengan praktik " +
        "analisis HPLC di laboratorium Kimia Analisis " +
        "SMKN 13 Bandung.";

    string kontenMenu2 =
        "<b>Instrumen HPLC Shimadzu LC-20AT</b>\n\n" +
        "<b>1. CBM-20A (System Controller)</b>\n" +
        "Modul pengontrol utama sistem. Mengatur dan memonitor " +
        "seluruh komponen instrumen secara terintegrasi.\n\n" +
        "<b>2. DGU-20A (Degasser)</b>\n" +
        "Menghilangkan gelembung udara dari fase gerak sebelum " +
        "masuk ke pompa agar aliran stabil dan konsisten.\n\n" +
        "<b>3. SPD-20A (UV-Vis Detector)</b>\n" +
        "Mendeteksi komponen yang keluar dari kolom berdasarkan " +
        "absorbansi sinar UV-Vis dan menghasilkan sinyal kromatogram.\n\n" +
        "<b>4. LC-20AT (Pump)</b>\n" +
        "Mendorong fase gerak dengan tekanan tinggi secara konstan " +
        "melalui seluruh sistem.\n\n" +
        "<b>Injeksi Sampel</b>\n" +
        "Sampel diinjeksikan ke dalam aliran fase gerak melalui " +
        "injektor, kemudian terbawa masuk ke kolom untuk proses pemisahan.";

    string kontenMenu3 =
        "<b>Mode Fase Pemisahan HPLC</b>\n\n" +
        "<b>Prinsip: Like Dissolves Like</b>\n" +
        "Senyawa polar berikatan lebih kuat dengan fase polar, " +
        "senyawa non-polar berikatan lebih kuat dengan " +
        "fase non-polar. Perbedaan kekuatan interaksi " +
        "inilah yang menghasilkan waktu retensi berbeda.\n\n" +
        "<b>Fase Normal</b>\n" +
        "- Fase Diam  : POLAR (Silika Berpori)\n" +
        "- Fase Gerak : NON-POLAR (Heksan/Heptana)\n" +
        "- Polar tertahan lebih lama (tR besar)\n" +
        "- Non-Polar keluar lebih cepat (tR kecil)\n\n" +
        "<b>Fase Terbalik</b>\n" +
        "- Fase Diam  : NON-POLAR (C18)\n" +
        "- Fase Gerak : POLAR (Metanol + Air)\n" +
        "- Non-Polar tertahan lebih lama (tR besar)\n" +
        "- Polar keluar lebih cepat (tR kecil)\n" +
        "- Paling umum digunakan karena fleksibel\n\n" +
        "<b>Keterangan Warna:</b>\n" +
        "Merah = Non-Polar | Hijau = Polar";

    string kontenMenu4 =
        "<b>Kromatogram & Perhitungan Resolusi</b>\n\n" +
        "<b>Apa itu Kromatogram?</b>\n" +
        "Grafik yang menampilkan hubungan waktu retensi (sumbu X) " +
        "dan absorbansi detektor (sumbu Y). Setiap puncak mewakili " +
        "satu komponen dalam sampel.\n\n" +
        "<b>Waktu Retensi (tR)</b>\n" +
        "Waktu yang dibutuhkan komponen untuk keluar dari kolom " +
        "dan terdeteksi oleh detektor.\n\n" +
        "<b>Rumus Resolusi (Rs)</b>\n" +
        "Rs = 2(tR\u2082 \u2013 tR\u2081) / (W\u2081 + W\u2082)\n\n" +
        "<b>Interpretasi Nilai Rs</b>\n" +
        "- Rs < 1,0  : Buruk\n" +
        "- Rs \u2265 1,0  : Kurang Optimal\n" +
        "- Rs \u2265 1,5  : Baik (standar minimum)\n" +
        "- Rs \u2265 2,0  : Sangat Baik";

    void Start()
    {
        if (txtKonten == null)
        {
            Debug.LogError("TxtKonten belum diassign di Inspector!");
            return;
        }

        string menuAsal = "Menu1";
        if (GameManager.Instance != null)
            menuAsal = GameManager.Instance.menuAsal;

        switch (menuAsal)
        {
            case "Menu1": txtKonten.text = kontenMenu1; break;
            case "Menu2": txtKonten.text = kontenMenu2; break;
            case "Menu3": txtKonten.text = kontenMenu3; break;
            case "Menu4": txtKonten.text = kontenMenu4; break;
            default:      txtKonten.text = kontenMenu1; break;
        }

        if (txtKonteks != null)
            txtKonteks.text = "Konteks: " + LabelKonteks(menuAsal);

        if (txtJudulKonten != null)
            txtJudulKonten.text = JudulKonten(menuAsal);
    }

    string LabelKonteks(string menuAsal)
    {
        switch (menuAsal)
        {
            case "Menu1": return "Menu 1 — Pemilihan Sampel";
            case "Menu2": return "Menu 2 — Injeksi";
            case "Menu3": return "Menu 3 — Mode X-Ray Kolom";
            case "Menu4": return "Menu 4 — Hasil Analisis";
            default:      return "Menu 1 — Pemilihan Sampel";
        }
    }

    string JudulKonten(string menuAsal)
    {
        switch (menuAsal)
        {
            case "Menu1": return "Sampel Uji HPLC";
            case "Menu2": return "Prosedur Injeksi Sampel";
            case "Menu3": return "Mode Fase Pemisahan";
            case "Menu4": return "Kromatogram & Resolusi";
            default:      return "Sampel Uji HPLC";
        }
    }

    public void Kembali()
    {
        string menuAsal = "Menu1";
        if (GameManager.Instance != null)
            menuAsal = GameManager.Instance.menuAsal;

        SceneManager.LoadScene(menuAsal);
    }
}