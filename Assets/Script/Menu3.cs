using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Menu3 : MonoBehaviour
{
    [Header("Referensi Objek 3D")]
    public GameObject innerColumn;
    public GameObject mesinDalam;

    [Header("Particle System")]
    public ParticleSystem partikelMerah;   // Non-Polar
    public ParticleSystem partikelHijau;   // Polar
    public ParticleSystem partikelFaseGerakMetanol;
    public ParticleSystem partikelFaseGerakAir;

    [Header("Jalur Partikel di Mesin Dalam")]
    public PartikelIkutiJalur jalurFaseGerakMetanol;
    public PartikelIkutiJalur jalurFaseGerakAir;

    [Header("UI Elements")]
    public TextMeshProUGUI txtPenjelasan;
    public TextMeshProUGUI txtHasilAnalisis;
    public TextMeshProUGUI txtLabelWarna;  // label keterangan warna
    public TextMeshProUGUI txtTimer;
    public GameObject buttonFaseNormal;
    public GameObject buttonFaseTerbalik;
    public Image imgFaseNormal;
    public Image imgFaseTerbalik;

    private const float DurasiSimulasi = 7f;
    private const float DurasiMesinDalam = 9f;

    private static readonly Color WarnaAktif = new Color(0.101961f, 0.603922f, 0.682353f, 1f);
    private static readonly Color WarnaNonAktif = new Color(0.075f, 0.122f, 0.2f, 1f);

    // posisi partikel fase gerak setelah pindah dari mesin_dalam ke kolom analitik
    private static readonly Vector3 PosisiFaseGerakMetanolDiKolom = new Vector3(-0.15f, 1.0f, 1.83f);
    private static readonly Vector3 PosisiFaseGerakAirDiKolom = new Vector3(0.15f, 1.0f, 1.83f);

    void Start()
    {
        // Tahap 1: mesin dalam + aliran fase gerak, kolom & pilihan fase disembunyikan dulu
        if (innerColumn != null) innerColumn.SetActive(false);
        if (buttonFaseNormal != null) buttonFaseNormal.SetActive(false);
        if (buttonFaseTerbalik != null) buttonFaseTerbalik.SetActive(false);

        partikelMerah.gameObject.SetActive(false);
        partikelHijau.gameObject.SetActive(false);

        if (txtTimer != null)
            txtTimer.text = "";

        if (txtPenjelasan != null)
            txtPenjelasan.text = "Mengamati aliran fase gerak di dalam instrumen...";

        if (txtLabelWarna != null)
            txtLabelWarna.text = "";

        if (mesinDalam != null)
            mesinDalam.SetActive(true);

        // Persentase Metanol:Air dipakai sebagai skala kecepatan -- rasio yang lebih dominan
        // tampil mengalir lebih deras, cocok dengan pilihan slider di MenuFaseGerak.
        (float persenMetanol, float persenAir) = RasioFaseGerak();
        MulaiPartikelFaseGerak(partikelFaseGerakMetanol, persenMetanol, persenBasisBaik: 70f);
        MulaiPartikelFaseGerak(partikelFaseGerakAir, persenAir, persenBasisBaik: 30f);

        // Selama di mesin dalam, partikel dipaksa ikut jalur selang
        if (jalurFaseGerakMetanol != null) jalurFaseGerakMetanol.enabled = true;
        if (jalurFaseGerakAir != null) jalurFaseGerakAir.enabled = true;

        StartCoroutine(HitungMundur(DurasiMesinDalam));
        StartCoroutine(TampilkanKolomSetelahMesinDalam());
    }

    // Kecepatan simulasi partikel merah/hijau di kolom, mengikuti skenario slider fase gerak.
    // Skenario "Baik" = nilai asli yang sudah divalidasi guru (0.35/0.08), jangan diubah.
    // "Kurang Optimal" -> kecepatan lebih berdekatan (pemisahan kurang jelas, cocok Rs rendah).
    // "Sangat Baik" -> kecepatan lebih jauh beda (pemisahan lebih jelas, cocok Rs tinggi).
    (float cepat, float lambat) KecepatanUntukSkenario()
    {
        string skenario = GameManager.Instance != null ? GameManager.Instance.skenarioFaseGerak : "Baik";
        switch (skenario)
        {
            case "Kurang Optimal": return (0.20f, 0.14f);
            case "Sangat Baik":    return (0.45f, 0.05f);
            default:               return (0.35f, 0.08f); // Baik
        }
    }

    // Persentase komposisi Metanol:Air sesuai kategori skenario slider di MenuFaseGerak.cs
    // (40:60 / 70:30 / 80:20) -- didefinisikan ulang di sini karena GameManager hanya
    // menyimpan label kategorinya, bukan angka persennya.
    (float persenMetanol, float persenAir) RasioFaseGerak()
    {
        string skenario = GameManager.Instance != null ? GameManager.Instance.skenarioFaseGerak : "Baik";
        switch (skenario)
        {
            case "Kurang Optimal": return (40f, 60f);
            case "Sangat Baik":    return (80f, 20f);
            default:               return (70f, 30f); // Baik
        }
    }

    void MulaiPartikelFaseGerak(ParticleSystem partikel, float persenKomposisi, float persenBasisBaik)
    {
        if (partikel == null) return;

        // dipercepat dari default template (dulu dipakai untuk elusi 7 detik yang sengaja lambat).
        // startSpeed diskalakan relatif ke persentase skenario "Baik" (basis), supaya tampilan
        // default (skenario Baik) tidak berubah dari sebelumnya, dan skenario lain jadi lebih
        // deras/lambat sesuai proporsi Metanol:Air yang dipilih di slider.
        var main = partikel.main;
        main.simulationSpeed = 1f;
        main.startSpeed = 1.5f * (persenKomposisi / persenBasisBaik);

        partikel.gameObject.SetActive(true);
        partikel.Play();
    }

    IEnumerator TampilkanKolomSetelahMesinDalam()
    {
        yield return new WaitForSeconds(DurasiMesinDalam);

        // Tahap 2: mesin dalam disembunyikan, kolom analitik + pilihan fase muncul
        if (mesinDalam != null) mesinDalam.SetActive(false);
        if (innerColumn != null) innerColumn.SetActive(true);

        // Sudah keluar dari mesin dalam, lepas dari jalur selang, kembali ke simulasi partikel biasa
        if (jalurFaseGerakMetanol != null) jalurFaseGerakMetanol.enabled = false;
        if (jalurFaseGerakAir != null) jalurFaseGerakAir.enabled = false;

        // Fase gerak lanjut mengalir, sekarang di posisi kolom
        if (partikelFaseGerakMetanol != null)
        {
            partikelFaseGerakMetanol.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            partikelFaseGerakMetanol.transform.localPosition = PosisiFaseGerakMetanolDiKolom;
            partikelFaseGerakMetanol.Play();
        }
        if (partikelFaseGerakAir != null)
        {
            partikelFaseGerakAir.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            partikelFaseGerakAir.transform.localPosition = PosisiFaseGerakAirDiKolom;
            partikelFaseGerakAir.Play();
        }

        if (buttonFaseNormal != null) buttonFaseNormal.SetActive(true);
        if (buttonFaseTerbalik != null) buttonFaseTerbalik.SetActive(true);

        if (txtPenjelasan != null)
            txtPenjelasan.text = "Pilih mode fase pemisahan untuk memulai simulasi.";

        if (txtLabelWarna != null)
            txtLabelWarna.text =
                "<color=#EF4444>●</color> Non-Polar\n" +
                "<color=#22C55E>●</color> Polar";
    }

    public void PilihFaseNormal()
    {
        GameManager.Instance.faseDipilih = "Normal";

        if (txtPenjelasan != null)
            txtPenjelasan.text =
                "Fase Normal: senyawa non-polar keluar lebih cepat dari kolom fase diam polar.";

        if (txtHasilAnalisis != null)
            txtHasilAnalisis.text =
                "Non-Polar keluar lebih cepat, Polar tertahan lebih lama";

        // DIPERLAMBAT sesuai masukan guru (nilai skenario "Baik" di bawah ini persis nilai itu)
        (float cepat, float lambat) = KecepatanUntukSkenario();

        var mainMerah = partikelMerah.main;
        mainMerah.simulationSpeed = cepat;  // non-polar, cepat

        var mainHijau = partikelHijau.main;
        mainHijau.simulationSpeed = lambat;  // polar, lambat

        partikelMerah.gameObject.SetActive(true);
        partikelHijau.gameObject.SetActive(true);
        partikelMerah.Play();
        partikelHijau.Play();

        if (imgFaseNormal != null) imgFaseNormal.color = WarnaAktif;
        if (imgFaseTerbalik != null) imgFaseTerbalik.color = WarnaNonAktif;

        buttonFaseNormal.GetComponent<Button>().interactable = false;
        buttonFaseTerbalik.GetComponent<Button>().interactable = false;

        StartCoroutine(HitungMundur(DurasiSimulasi));
        StartCoroutine(PindahKeMenu4(DurasiSimulasi));
    }

    public void PilihFaseTerbalik()
    {
        GameManager.Instance.faseDipilih = "Terbalik";

        if (txtPenjelasan != null)
            txtPenjelasan.text =
                "Fase Terbalik: senyawa polar keluar lebih cepat dari kolom fase diam non-polar (C18).";

        if (txtHasilAnalisis != null)
            txtHasilAnalisis.text =
                "Polar keluar lebih cepat, Non-Polar tertahan lebih lama";

        // DIPERLAMBAT sesuai masukan guru (nilai skenario "Baik" di bawah ini persis nilai itu)
        (float cepat, float lambat) = KecepatanUntukSkenario();

        var mainHijau = partikelHijau.main;
        mainHijau.simulationSpeed = cepat;  // polar, cepat

        var mainMerah = partikelMerah.main;
        mainMerah.simulationSpeed = lambat;  // non-polar, lambat

        partikelMerah.gameObject.SetActive(true);
        partikelHijau.gameObject.SetActive(true);
        partikelMerah.Play();
        partikelHijau.Play();

        if (imgFaseTerbalik != null) imgFaseTerbalik.color = WarnaAktif;
        if (imgFaseNormal != null) imgFaseNormal.color = WarnaNonAktif;

        buttonFaseNormal.GetComponent<Button>().interactable = false;
        buttonFaseTerbalik.GetComponent<Button>().interactable = false;

        StartCoroutine(HitungMundur(DurasiSimulasi));
        StartCoroutine(PindahKeMenu4(DurasiSimulasi));
    }

    IEnumerator HitungMundur(float durasi)
    {
        int sisa = Mathf.CeilToInt(durasi);
        while (sisa > 0)
        {
            if (txtTimer != null)
                txtTimer.text = sisa.ToString();
            yield return new WaitForSeconds(1f);
            sisa--;
        }
        if (txtTimer != null)
            txtTimer.text = "0";
    }

    IEnumerator PindahKeMenu4(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Menu4");
    }

    public void Kembali()
    {
        SceneManager.LoadScene("Menu2");
    }

    public void TombolInformasi()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.menuAsal = "Menu3";
        SceneManager.LoadScene("Informasi");
    }
}
