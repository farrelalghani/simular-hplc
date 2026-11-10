using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Kuis : MonoBehaviour
{
    [System.Serializable]
    public class SoalKuis
    {
        public string pertanyaan;
        public string[] opsi;
        public int jawabanBenar;
        public string penjelasan;
    }

    [Header("Header & Progress")]
    public TextMeshProUGUI txtCounter;
    public Image imgProgressFill;

    [Header("Panel Soal")]
    public TextMeshProUGUI txtNomorSoal;
    public TextMeshProUGUI txtPertanyaan;
    public Button[] tombolJawaban;
    public Image[] bgJawaban;
    public TextMeshProUGUI[] txtJawaban;

    [Header("Panel Feedback")]
    public GameObject panelFeedback;
    public TextMeshProUGUI txtFeedbackTitle;
    public TextMeshProUGUI txtPenjelasan;
    public GameObject buttonSelanjutnya;
    public TextMeshProUGUI txtLabelSelanjutnya;

    private static readonly string[] Huruf = { "A", "B", "C", "D" };
    private static readonly Color WarnaDefault = new Color(0.09f, 0.14f, 0.22f, 1f);
    private static readonly Color WarnaBenar = new Color(0.09f, 0.45f, 0.28f, 1f);
    private static readonly Color WarnaSalah = new Color(0.5f, 0.14f, 0.14f, 1f);
    private static readonly Color WarnaTeksBenar = new Color(0.13f, 0.77f, 0.37f, 1f);
    private static readonly Color WarnaTeksSalah = new Color(0.94f, 0.27f, 0.27f, 1f);

    private readonly SoalKuis[] daftarSoal = new SoalKuis[]
    {
        new SoalKuis
        {
            pertanyaan = "Apa kepanjangan dari HPLC?",
            opsi = new[] {
                "High Performance Liquid Chromatography",
                "High Pressure Liquid Compound",
                "Heavy Phase Liquid Chemistry",
                "High Performance Light Chromatography"
            },
            jawabanBenar = 0,
            penjelasan = "HPLC adalah singkatan dari High Performance Liquid Chromatography, teknik kromatografi cair kinerja tinggi untuk memisahkan komponen dalam suatu campuran."
        },
        new SoalKuis
        {
            pertanyaan = "Apa fungsi utama pompa LC-20AT pada sistem HPLC?",
            opsi = new[] {
                "Mendeteksi komponen sampel",
                "Mendorong fase gerak dengan tekanan tinggi secara konstan",
                "Menghilangkan gelembung udara dari fase gerak",
                "Menampung sampel sebelum injeksi"
            },
            jawabanBenar = 1,
            penjelasan = "LC-20AT (Pump) bertugas mendorong fase gerak dengan tekanan tinggi secara konstan melalui seluruh sistem HPLC."
        },
        new SoalKuis
        {
            pertanyaan = "Pada Fase Terbalik (Reversed Phase), jenis senyawa manakah yang keluar lebih dahulu dari kolom analitik?",
            opsi = new[] {
                "Senyawa non-polar",
                "Senyawa polar",
                "Kedua senyawa bersamaan",
                "Tidak dapat ditentukan"
            },
            jawabanBenar = 1,
            penjelasan = "Pada Fase Terbalik, fase diam bersifat non-polar (C18) sedangkan fase gerak bersifat polar. Senyawa polar memiliki afinitas rendah terhadap fase diam sehingga bergerak lebih cepat dan keluar lebih dahulu dari kolom."
        },
        new SoalKuis
        {
            pertanyaan = "Apa fungsi Degasser (DGU-20A) pada instrumen HPLC?",
            opsi = new[] {
                "Mendeteksi sinyal UV-Vis",
                "Mengontrol seluruh sistem instrumen",
                "Menghilangkan gelembung udara dari fase gerak",
                "Memisahkan komponen sampel di dalam kolom"
            },
            jawabanBenar = 2,
            penjelasan = "DGU-20A (Degasser) menghilangkan gelembung udara dari fase gerak sebelum masuk ke pompa agar aliran tetap stabil dan konsisten."
        },
        new SoalKuis
        {
            pertanyaan = "Pada Fase Normal, fase diam yang digunakan bersifat...",
            opsi = new[] { "Non-polar", "Polar", "Netral, tidak berpengaruh", "Bergantung pada sampel" },
            jawabanBenar = 1,
            penjelasan = "Fase Normal menggunakan fase diam POLAR (silika berpori) dengan fase gerak NON-POLAR (heksan/heptana), sehingga senyawa polar tertahan lebih lama di kolom."
        },
        new SoalKuis
        {
            pertanyaan = "Apa yang dimaksud dengan waktu retensi (tR)?",
            opsi = new[] {
                "Waktu total keseluruhan proses analisis",
                "Waktu yang dibutuhkan komponen untuk keluar dari kolom dan terdeteksi detektor",
                "Waktu persiapan sampel sebelum injeksi",
                "Waktu pergantian fase gerak"
            },
            jawabanBenar = 1,
            penjelasan = "Waktu retensi (tR) adalah waktu yang dibutuhkan suatu komponen untuk keluar dari kolom dan terdeteksi oleh detektor."
        },
        new SoalKuis
        {
            pertanyaan = "Manakah rumus resolusi (Rs) yang benar dalam kromatografi?",
            opsi = new[] {
                "Rs = (tR2 - tR1) / (W1 + W2)",
                "Rs = 2(tR2 - tR1) / (W1 + W2)",
                "Rs = (tR1 + tR2) / 2(W1 - W2)",
                "Rs = (W1 + W2) / 2(tR2 - tR1)"
            },
            jawabanBenar = 1,
            penjelasan = "Rumus resolusi yang benar adalah Rs = 2(tR2 - tR1) / (W1 + W2), yaitu dua kali selisih waktu retensi dibagi jumlah lebar dasar kedua puncak."
        },
        new SoalKuis
        {
            pertanyaan = "Nilai Rs berapa yang dianggap \"Baik\" sebagai standar minimum pemisahan?",
            opsi = new[] { "Rs ≥ 0,5", "Rs ≥ 1,0", "Rs ≥ 1,5", "Rs ≥ 3,0" },
            jawabanBenar = 2,
            penjelasan = "Rs ≥ 1,5 dianggap Baik dan menjadi standar minimum pemisahan yang dapat diterima. Rs ≥ 2,0 dikategorikan Sangat Baik."
        },
        new SoalKuis
        {
            pertanyaan = "Detektor SPD-20A pada HPLC Shimadzu LC-20AT bekerja berdasarkan prinsip...",
            opsi = new[] { "Fluoresensi", "Indeks bias", "Absorbansi sinar UV-Vis", "Konduktivitas listrik" },
            jawabanBenar = 2,
            penjelasan = "SPD-20A adalah detektor UV-Vis yang mendeteksi komponen berdasarkan absorbansi sinar UV-Vis dan menghasilkan sinyal kromatogram."
        },
        new SoalKuis
        {
            pertanyaan = "Mengapa Fase Terbalik (Reversed Phase) paling umum digunakan dalam analisis HPLC?",
            opsi = new[] {
                "Karena biayanya paling murah",
                "Karena hanya bisa memisahkan senyawa polar",
                "Karena lebih fleksibel dan cocok untuk berbagai jenis senyawa",
                "Karena tidak memerlukan fase gerak"
            },
            jawabanBenar = 2,
            penjelasan = "Fase Terbalik paling umum digunakan karena lebih fleksibel dan dapat diaplikasikan untuk memisahkan berbagai jenis senyawa, mulai dari yang polar hingga non-polar."
        },
    };

    private int soalKe = 0;
    private int skor = 0;
    private bool sudahJawab = false;

    void Start()
    {
        soalKe = 0;
        skor = 0;
        TampilkanSoal();
    }

    void TampilkanSoal()
    {
        sudahJawab = false;
        SoalKuis soal = daftarSoal[soalKe];

        if (txtNomorSoal != null) txtNomorSoal.text = $"Soal {soalKe + 1}";
        if (txtPertanyaan != null) txtPertanyaan.text = soal.pertanyaan;
        if (txtCounter != null) txtCounter.text = $"{soalKe + 1} / {daftarSoal.Length}";
        if (imgProgressFill != null) imgProgressFill.fillAmount = (float)soalKe / daftarSoal.Length;

        for (int i = 0; i < tombolJawaban.Length; i++)
        {
            txtJawaban[i].text = $"<b>{Huruf[i]}</b>   {soal.opsi[i]}";
            bgJawaban[i].color = WarnaDefault;
            tombolJawaban[i].interactable = true;
            tombolJawaban[i].gameObject.SetActive(true);
        }

        if (panelFeedback != null) panelFeedback.SetActive(false);
        if (buttonSelanjutnya != null) buttonSelanjutnya.SetActive(true);
    }

    public void PilihJawaban(int index)
    {
        if (sudahJawab) return;
        sudahJawab = true;

        SoalKuis soal = daftarSoal[soalKe];
        bool benar = index == soal.jawabanBenar;
        if (benar) skor++;

        bgJawaban[index].color = benar ? WarnaBenar : WarnaSalah;
        if (!benar) bgJawaban[soal.jawabanBenar].color = WarnaBenar;

        foreach (var tombol in tombolJawaban)
            tombol.interactable = false;

        if (txtFeedbackTitle != null)
        {
            txtFeedbackTitle.text = benar ? "✓ Benar!" : "✗ Salah";
            txtFeedbackTitle.color = benar ? WarnaTeksBenar : WarnaTeksSalah;
        }

        if (txtPenjelasan != null)
            txtPenjelasan.text = $"<b>Penjelasan:</b>\n{soal.penjelasan}";

        if (panelFeedback != null) panelFeedback.SetActive(true);

        if (txtLabelSelanjutnya != null)
            txtLabelSelanjutnya.text = (soalKe == daftarSoal.Length - 1) ? "Lihat Hasil" : "Soal Selanjutnya  →";
    }

    public void SoalSelanjutnya()
    {
        if (!sudahJawab) return;

        soalKe++;
        if (soalKe >= daftarSoal.Length)
        {
            TampilkanHasilAkhir();
            return;
        }
        TampilkanSoal();
    }

    void TampilkanHasilAkhir()
    {
        if (panelFeedback != null) panelFeedback.SetActive(false);
        foreach (var tombol in tombolJawaban)
            tombol.gameObject.SetActive(false);
        if (buttonSelanjutnya != null) buttonSelanjutnya.SetActive(false);

        if (txtNomorSoal != null) txtNomorSoal.text = "Selesai";
        if (imgProgressFill != null) imgProgressFill.fillAmount = 1f;
        if (txtCounter != null) txtCounter.text = $"{daftarSoal.Length} / {daftarSoal.Length}";

        if (txtPertanyaan != null)
            txtPertanyaan.text = $"<b>Kuis Selesai!</b>\n\nSkor kamu: {skor} / {daftarSoal.Length}\n\n{KomentarSkor()}";
    }

    string KomentarSkor()
    {
        float persen = (float)skor / daftarSoal.Length;
        if (persen >= 0.8f) return "Pemahaman kamu tentang HPLC sudah sangat baik!";
        if (persen >= 0.5f) return "Cukup baik, pelajari kembali materi yang masih keliru.";
        return "Yuk pelajari kembali Materi HPLC sebelum mencoba lagi.";
    }

    public void Kembali()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
