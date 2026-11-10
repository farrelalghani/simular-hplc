using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MateriHPLC : MonoBehaviour
{
    [System.Serializable]
    public class Topik
    {
        public string judul;
        public string subjudul;
        [TextArea] public string deskripsi;
    }

    [Header("Konten")]
    public TextMeshProUGUI txtJudul;
    public TextMeshProUGUI txtSubjudul;
    public TextMeshProUGUI txtDeskripsi;
    public TextMeshProUGUI txtProgress;
    public TextMeshProUGUI txtTombolLanjut;

    [Header("Sidebar")]
    public Image bgTopik1;
    public Image bgTopik2;
    public Image bgTopik3;
    public Image bgTopik4;
    public Image bgTopik5;

    private Image[] bgTopik;
    private int topikSaatIni = 0;

    private static readonly Color WarnaAktif = new Color(0.101961f, 0.603922f, 0.682353f, 1f);
    private static readonly Color WarnaNonAktif = new Color(0.075f, 0.122f, 0.2f, 1f);

    private readonly Topik[] daftarTopik = new Topik[]
    {
        new Topik
        {
            judul = "Komponen Instrumen HPLC",
            subjudul = "12 komponen utama",
            deskripsi = "Instrumen HPLC terdiri dari 12 komponen utama yang bekerja secara berurutan:\n\n" +
                "1. Reservoir - wadah fase gerak (Metanol + Air)\n" +
                "2. Degasser - menghilangkan gas terlarut\n" +
                "3. Katup Gradien - mengatur komposisi pelarut\n" +
                "4. Mixer - mencampur fase gerak\n" +
                "5. Pompa - mendorong fase gerak bertekanan tinggi\n" +
                "6. Injektor - memasukkan sampel ke sistem\n" +
                "7. Sample Loop - menampung volume sampel secara presisi\n" +
                "8. Pra-kolom - melindungi kolom analitik dari kontaminan\n" +
                "9. Kolom Analitik - memisahkan komponen-komponen sampel\n" +
                "10. Detektor - mendeteksi komponen yang telah terpisah\n" +
                "11. Data System - mengolah data menjadi kromatogram\n" +
                "12. Wadah Limbah - menampung buangan fase gerak"
        },
        new Topik
        {
            judul = "Fase Normal",
            subjudul = "Fase diam polar, fase gerak non-polar",
            deskripsi = "Pada kromatografi Fase Normal, fase diam bersifat polar (contoh: silika) sedangkan fase gerak bersifat non-polar atau kurang polar.\n\n" +
                "Senyawa non-polar memiliki afinitas rendah terhadap fase diam yang polar, sehingga bergerak lebih cepat dan keluar lebih dahulu dari kolom.\n\n" +
                "Senyawa polar akan tertahan lebih lama karena berinteraksi kuat dengan fase diam."
        },
        new Topik
        {
            judul = "Fase Terbalik (Reversed Phase)",
            subjudul = "Fase diam non-polar (C18), fase gerak polar",
            deskripsi = "Pada Fase Terbalik, fase diam bersifat non-polar (umumnya C18) sedangkan fase gerak bersifat polar (Metanol + Air).\n\n" +
                "Senyawa polar memiliki afinitas rendah terhadap fase diam sehingga bergerak lebih cepat dan keluar lebih dahulu dari kolom.\n\n" +
                "Senyawa non-polar tertahan lebih lama karena berinteraksi kuat dengan fase diam C18.\n\n" +
                "Fase Terbalik adalah metode HPLC yang paling umum digunakan di laboratorium."
        },
        new Topik
        {
            judul = "Prosedur Injeksi Sampel",
            subjudul = "Micro-Syringe & Rheodyne Valve",
            deskripsi = "Injeksi sampel dilakukan melalui katup Rheodyne dua posisi.\n\n" +
                "Pada posisi LOAD, analis mengisi sample loop dengan volume sampel yang presisi.\n\n" +
                "Ketika katup diputar ke posisi INJECT, fase gerak mendorong sampel dari sample loop menuju kolom analitik untuk dipisahkan.\n\n" +
                "- Volume sample loop: 10-25 mikroliter\n" +
                "- Menjamin reprodusibilitas volume injeksi\n" +
                "- Dua tahap: LOAD kemudian INJECT"
        },
        new Topik
        {
            judul = "Membaca Kromatogram",
            subjudul = "Waktu retensi, lebar puncak, dan resolusi (Rs)",
            deskripsi = "Kromatogram menampilkan puncak (peak) untuk tiap komponen yang terpisah.\n\n" +
                "- Waktu Retensi (tR): waktu yang dibutuhkan komponen untuk keluar dari kolom\n" +
                "- Lebar Puncak (W): lebar dasar puncak, menggambarkan efisiensi pemisahan\n" +
                "- Luas Puncak: berbanding lurus dengan konsentrasi komponen\n\n" +
                "Resolusi (Rs) mengukur seberapa baik dua puncak terpisah:\n" +
                "Rs = 2(tR2 - tR1) / (W1 + W2)\n\n" +
                "Semakin besar nilai Rs, semakin baik pemisahannya:\n" +
                "Rs >= 2.0 Sangat Baik | Rs >= 1.5 Baik | Rs >= 1.0 Kurang Optimal | Rs < 1.0 Buruk"
        },
    };

    void Start()
    {
        bgTopik = new Image[] { bgTopik1, bgTopik2, bgTopik3, bgTopik4, bgTopik5 };
        SetTopik(0);
    }

    public void PilihTopik1() { SetTopik(0); }
    public void PilihTopik2() { SetTopik(1); }
    public void PilihTopik3() { SetTopik(2); }
    public void PilihTopik4() { SetTopik(3); }
    public void PilihTopik5() { SetTopik(4); }

    public void SetTopik(int index)
    {
        topikSaatIni = index;
        Topik t = daftarTopik[index];

        if (txtJudul != null) txtJudul.text = t.judul;
        if (txtSubjudul != null) txtSubjudul.text = t.subjudul;
        if (txtDeskripsi != null) txtDeskripsi.text = t.deskripsi;
        if (txtProgress != null) txtProgress.text = (index + 1) + " / " + daftarTopik.Length;
        if (txtTombolLanjut != null)
            txtTombolLanjut.text = (index == daftarTopik.Length - 1) ? "Selesai" : "Materi Selanjutnya →";

        for (int i = 0; i < bgTopik.Length; i++)
        {
            if (bgTopik[i] != null)
                bgTopik[i].color = (i == index) ? WarnaAktif : WarnaNonAktif;
        }
    }

    public void MateriSelanjutnya()
    {
        if (topikSaatIni < daftarTopik.Length - 1)
            SetTopik(topikSaatIni + 1);
        else
            Kembali();
    }

    public void Kembali()
    {
        SceneManager.LoadScene("MainMenu");
    }
}