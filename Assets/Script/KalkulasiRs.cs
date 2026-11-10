using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class KalkulasiRs : MonoBehaviour
{
    [Header("Gambar Kromatogram UI")]
    public RawImage imgKromatogram;
    public GameObject graphCard;   // kartu background di belakang imgKromatogram, disembunyikan otomatis kalau mode monitor 3D aktif

    [Header("Font untuk Label Grafik (angka/nama sumbu)")]
    public TMP_FontAsset fontLabelGrafik;

    [Header("Layar Monitor 3D (opsional -- isi monitorScreenRenderer utk aktifkan mode ini)")]
    public Renderer monitorScreenRenderer;   // mesh Monitor_Screen pada PCMonitor_Menu4
    public float skalaLebarLayar3D = 1f;
    public float skalaTinggiLayar3D = 1f;
    // Posisi world-space pasti untuk quad LayarKromatogram3D (ditemukan lewat tuning manual di
    // Play Mode). Dipakai langsung, tidak lagi dihitung otomatis dari bounds.center Monitor_Screen.
    public Vector3 posisiLayar3D = new Vector3(-0.013f, 0.014f, 0.16f);

    [Header("UI Teks Perhitungan")]
    public TextMeshProUGUI txtKondisi;
    public TextMeshProUGUI txtLangkah1;
    public TextMeshProUGUI txtLangkah2;
    public TextMeshProUGUI txtLangkah3;
    public TextMeshProUGUI txtLangkah4;
    public TextMeshProUGUI txtKesimpulan;
    public TextMeshProUGUI txtKualitatif;
    public TextMeshProUGUI txtInfoDetektor;

    [Header("Panel Background")]
    public GameObject panelHasil;

    // Wrapper ber-RectMask2D yang dibungkuskan ke imgKromatogram secara runtime, dipakai supaya
    // grafik bisa dianimasikan "tumbuh"/meluncur naik dari sumbu bawah alih-alih muncul statis
    // langsung penuh seperti RawImage biasa.
    RectTransform wrapperGrafik;

    // Format: { tR1, tR2, W1, W2 }. Kunci: "Sampel|Fase|SkenarioFaseGerak".
    // Nilai untuk skenario "Baik" adalah nilai asli yang sudah divalidasi Guru Kimia Analisis
    // (lihat Tabel 3.29 Deterministic Look-Up Table di skripsi) -- jangan diubah. Nilai untuk
    // "Kurang Optimal"/"Sangat Baik" diturunkan dari situ: rasio fase gerak yang kurang optimal
    // membuat tR1-tR2 lebih berdekatan dan puncak lebih lebar (Rs turun), rasio yang sangat baik
    // membuat tR1-tR2 lebih terpisah dan puncak lebih tajam (Rs naik), tR1 (waktu retensi awal)
    // tidak diubah supaya logika pencocokan tR standar di analisis kualitatif tetap konsisten.
    static readonly Dictionary<string, float[]> DataKromatogram = new Dictionary<string, float[]>
    {
        { "Parasetamol|Normal|Kurang Optimal",   new float[] { 4.1f, 4.8f, 1.0f, 1.1f } },
        { "Parasetamol|Normal|Baik",             new float[] { 4.1f, 5.2f, 0.8f, 0.9f } },
        { "Parasetamol|Normal|Sangat Baik",      new float[] { 4.1f, 5.6f, 0.7f, 0.8f } },

        { "Parasetamol|Terbalik|Kurang Optimal", new float[] { 2.8f, 3.6f, 0.8f, 1.0f } },
        { "Parasetamol|Terbalik|Baik",           new float[] { 2.8f, 4.1f, 0.7f, 0.8f } },
        { "Parasetamol|Terbalik|Sangat Baik",    new float[] { 2.8f, 4.6f, 0.6f, 0.7f } },

        { "Kafein|Normal|Kurang Optimal",        new float[] { 3.5f, 4.1f, 1.0f, 1.1f } },
        { "Kafein|Normal|Baik",                  new float[] { 3.5f, 4.4f, 0.8f, 0.9f } },
        { "Kafein|Normal|Sangat Baik",            new float[] { 3.5f, 4.8f, 0.7f, 0.8f } },

        { "Kafein|Terbalik|Kurang Optimal",      new float[] { 3.2f, 4.4f, 1.0f, 1.1f } },
        { "Kafein|Terbalik|Baik",                new float[] { 3.2f, 5.0f, 0.8f, 0.9f } },
        { "Kafein|Terbalik|Sangat Baik",         new float[] { 3.2f, 5.7f, 0.7f, 0.8f } },
    };

    void Start()
    {
        SembunyikanSemuaTeks();

        if (panelHasil != null)
            panelHasil.SetActive(false);

        // Kalau mode monitor 3D aktif (monitorScreenRenderer diisi), kartu grafik di layar HP
        // disembunyikan total supaya tidak menutupi pandangan ke monitor 3D di baliknya.
        bool pakaiMonitor3D = monitorScreenRenderer != null;

        if (graphCard != null)
            graphCard.SetActive(!pakaiMonitor3D);

        // Sembunyikan kromatogram dulu sebelum muncul animasi
        if (imgKromatogram != null)
            imgKromatogram.gameObject.SetActive(false);

        string cairan   = "Parasetamol";
        string fase     = "Terbalik";
        string skenario = "Baik";

        if (GameManager.Instance != null)
        {
            cairan   = GameManager.Instance.cairanDipilih;
            fase     = GameManager.Instance.faseDipilih;
            skenario = GameManager.Instance.skenarioFaseGerak;
        }

        float[] data = AmbilData(cairan, fase, skenario);
        float rs     = (2 * (data[1] - data[0])) / (data[2] + data[3]);

        // Grafik digambar langsung dari angka tR1/tR2/W1/W2 (bukan gambar statis), supaya
        // otomatis menyesuaikan tiap kombinasi sampel x fase x skenario fase gerak.
        Texture tekstur = GeneratorKromatogram.Buat(data[0], data[1], data[2], data[3]);
        if (imgKromatogram != null && !pakaiMonitor3D)
        {
            imgKromatogram.texture = tekstur;
            BuatLabelGrafik(imgKromatogram.rectTransform, data[0], data[1], data[2], data[3], rs);
            SiapkanWrapperAnimasiGrafik();
        }

        // Tempelkan versi grafik yang SUDAH lengkap dengan judul/label (bukan tekstur polos)
        // ke layar monitor 3D kalau mode ini aktif
        if (pakaiMonitor3D)
        {
            Texture teksturMonitor = BuatTeksturGrafikBerlabel(data[0], data[1], data[2], data[3], rs, cairan, fase);
            BuatLayarKromatogram3D(teksturMonitor);
        }

        // Tampilkan info detektor sesuai sampel
        if (txtInfoDetektor != null)
        {
            float lambda = cairan == "Parasetamol" ? 243f : 270f;
            txtInfoDetektor.text =
                $"🔬 Detektor SPD-20A (UV-Vis)  |  " +
                $"Sampel: {cairan}  |  λ = {lambda} nm";
        }

        string labelKondisi = $"Sampel: {cairan}  |  Fase {fase}  |  Fase Gerak: {skenario}";

        StartCoroutine(TampilkanHasil(data, labelKondisi, cairan));
    }

    float[] AmbilData(string sampel, string fase, string skenario)
    {
        string kunci = $"{sampel}|{fase}|{skenario}";
        if (DataKromatogram.TryGetValue(kunci, out float[] data))
            return data;

        // Fallback aman kalau skenario tidak dikenali (seharusnya tidak pernah terjadi)
        return DataKromatogram[$"{sampel}|{fase}|Baik"];
    }

    // Nama sumbu, angka skala waktu, label tR1/tR2, dan anotasi Rs -- ditempel sebagai TMP text
    // terpisah di atas RawImage (bukan digambar ke piksel tekstur), supaya hurufnya selalu
    // tajam/terbaca memakai font asli aplikasi. Posisinya dihitung memakai rumus yang sama
    // persis dengan GeneratorKromatogram supaya presisi nyambung dengan kurva di baliknya.
    void BuatLabelGrafik(RectTransform area, float tR1, float tR2, float W1, float W2, float rs, int layer = 0,
                          float skala = 1f)
    {
        if (area == null || fontLabelGrafik == null) return;

        // Bersihkan label lama kalau Start() sempat terpanggil ulang
        for (int i = area.childCount - 1; i >= 0; i--)
            Destroy(area.GetChild(i).gameObject);

        float waktuMaks = GeneratorKromatogram.HitungWaktuMaks(tR1, tR2, W1, W2);
        float fracKiri  = (float)GeneratorKromatogram.MarginKiri / GeneratorKromatogram.Lebar;
        float fracBawah = (float)GeneratorKromatogram.MarginBawah / GeneratorKromatogram.Tinggi;

        // Judul sumbu X
        BuatTeksLabel(area, "Waktu Retensi (menit)", new Vector2(0.5f, 0f), new Vector2(0, -18 * skala), 13 * skala,
                      TextAlignmentOptions.Center, layer);

        // Judul sumbu Y (diputar tegak)
        var judulY = BuatTeksLabel(area, "Absorbansi", new Vector2(0f, 0.5f), new Vector2(-22 * skala, 0), 13 * skala,
                      TextAlignmentOptions.Center, layer);
        if (judulY != null) judulY.rectTransform.localEulerAngles = new Vector3(0, 0, 90);

        // Angka skala tiap 1 menit di sumbu X
        int menitMaks = Mathf.FloorToInt(waktuMaks);
        for (int menit = 0; menit <= menitMaks; menit++)
        {
            float fracX = fracKiri + (menit / waktuMaks) * (1f - fracKiri);
            BuatTeksLabel(area, menit.ToString(), new Vector2(fracX, fracBawah), new Vector2(0, -11 * skala), 10 * skala,
                          TextAlignmentOptions.Center, layer);
        }

        // Label tR1 & tR2 di atas grafik, tepat di posisi X puncak masing-masing
        float fracTR1 = fracKiri + (tR1 / waktuMaks) * (1f - fracKiri);
        float fracTR2 = fracKiri + (tR2 / waktuMaks) * (1f - fracKiri);
        BuatTeksLabel(area, $"tR₁={tR1:F1}", new Vector2(fracTR1, 1f), new Vector2(0, 9 * skala), 10 * skala,
                      TextAlignmentOptions.Center, layer);
        BuatTeksLabel(area, $"tR₂={tR2:F1}", new Vector2(fracTR2, 1f), new Vector2(0, 9 * skala), 10 * skala,
                      TextAlignmentOptions.Center, layer);

        // Anotasi Rs di pojok kanan atas grafik
        BuatTeksLabel(area, $"Rs = {rs:F2}", new Vector2(1f, 1f), new Vector2(-6 * skala, 9 * skala), 12 * skala,
                      TextAlignmentOptions.Right, layer);
    }

    TextMeshProUGUI BuatTeksLabel(RectTransform induk, string teks, Vector2 anchor, Vector2 offsetPosisi,
                                   float ukuranFont, TextAlignmentOptions perataan, int layer = 0)
    {
        var go = new GameObject("Label_" + teks);
        go.layer = layer;
        go.transform.SetParent(induk, false);

        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = offsetPosisi;
        rt.sizeDelta = new Vector2(280, 40);

        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.font = fontLabelGrafik;
        tmp.text = teks;
        tmp.fontSize = ukuranFont;
        tmp.alignment = perataan;
        tmp.color = new Color(1f, 1f, 1f, 0.9f);
        tmp.raycastTarget = false;
        tmp.enableAutoSizing = false;

        return tmp;
    }

    // Bungkus imgKromatogram dengan sebuah wrapper ber-RectMask2D pada posisi & ukuran yang persis
    // sama, lalu jadikan imgKromatogram anak dari wrapper itu (mengisi penuh 100% wrapper).
    // Wrapper inilah yang jadi "jendela" tetap, sementara imgKromatogram sendiri nanti digeser
    // naik-turun oleh AnimasikanGrafikTumbuh() -- hasilnya terlihat seperti grafik meluncur naik
    // dari sumbu bawah, tanpa gambar aslinya ikut memelar/menyusut (beda dari sekadar resize).
    void SiapkanWrapperAnimasiGrafik()
    {
        if (imgKromatogram == null || wrapperGrafik != null) return;

        var wrapperGO = new GameObject("WrapperAnimasiGrafik");
        var wrapperRT = wrapperGO.AddComponent<RectTransform>();
        wrapperGO.AddComponent<RectMask2D>();

        Transform parentAsli = imgKromatogram.transform.parent;
        wrapperRT.SetParent(parentAsli, false);
        wrapperRT.anchorMin = imgKromatogram.rectTransform.anchorMin;
        wrapperRT.anchorMax = imgKromatogram.rectTransform.anchorMax;
        wrapperRT.offsetMin = imgKromatogram.rectTransform.offsetMin;
        wrapperRT.offsetMax = imgKromatogram.rectTransform.offsetMax;
        wrapperRT.SetSiblingIndex(imgKromatogram.transform.GetSiblingIndex());

        imgKromatogram.rectTransform.SetParent(wrapperRT, false);
        imgKromatogram.rectTransform.anchorMin = Vector2.zero;
        imgKromatogram.rectTransform.anchorMax = Vector2.one;
        imgKromatogram.rectTransform.offsetMin = Vector2.zero;
        imgKromatogram.rectTransform.offsetMax = Vector2.zero;

        wrapperGrafik = wrapperRT;
    }

    IEnumerator AnimasikanGrafikTumbuh(float durasi)
    {
        if (wrapperGrafik == null || imgKromatogram == null) yield break;

        wrapperGrafik.gameObject.SetActive(true);
        imgKromatogram.gameObject.SetActive(true);

        float tinggiPixel = wrapperGrafik.rect.height;
        var rtGrafik = imgKromatogram.rectTransform;
        Vector2 posisiAwal  = new Vector2(0, -tinggiPixel);
        Vector2 posisiAkhir = Vector2.zero;

        rtGrafik.anchoredPosition = posisiAwal;

        float t = 0f;
        while (t < durasi)
        {
            t += Time.deltaTime;
            float frac = 1f - Mathf.Pow(1f - Mathf.Clamp01(t / durasi), 3f); // ease-out, mendarat halus
            rtGrafik.anchoredPosition = Vector2.Lerp(posisiAwal, posisiAkhir, frac);
            yield return null;
        }

        rtGrafik.anchoredPosition = posisiAkhir;
    }

    string KategoriRs(float rs)
    {
        if (rs >= 2.0f) return "Sangat Baik";
        if (rs >= 1.5f) return "Baik";
        if (rs >= 1.0f) return "Kurang Optimal";
        return "Buruk";
    }

    // Layar monitor 3D tidak bisa menampilkan teks TMP terpisah di atasnya (objek 3D dunia,
    // beda dari RawImage 2D di HP), jadi untuk versi monitor, grafik + semua labelnya
    // (judul, sumbu, tR1/tR2, Rs) benar-benar DIRENDER ke sebuah Texture (RenderTexture) lewat
    // kamera & Canvas "siluman" yang tidak pernah tampil ke layar utama -- supaya hasilnya
    // pakai font TMP asli (tajam, terjamin benar), bukan digambar manual piksel per piksel.
    Texture BuatTeksturGrafikBerlabel(float tR1, float tR2, float W1, float W2, float rs,
                                       string cairan, string fase)
    {
        const int lebarRT = 1024;
        const int tinggiRT = 512;
        const int layerOffscreen = 8; // "GrafikOffscreen3D", lihat TagManager.asset

        var rt = new RenderTexture(lebarRT, tinggiRT, 16);

        var goKamera = new GameObject("KameraGrafikOffscreen");
        goKamera.layer = layerOffscreen;
        var kamera = goKamera.AddComponent<Camera>();
        kamera.clearFlags = CameraClearFlags.SolidColor;
        kamera.backgroundColor = new Color(0.075f, 0.122f, 0.2f, 1f);
        kamera.orthographic = true;
        kamera.cullingMask = 1 << layerOffscreen;
        kamera.targetTexture = rt;
        kamera.enabled = false; // dirender manual sekali saja lewat kamera.Render(), tidak usah tiap frame

        var goCanvas = new GameObject("CanvasGrafikOffscreen");
        goCanvas.layer = layerOffscreen;
        var canvas = goCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = kamera;
        canvas.planeDistance = 1f;
        var scaler = goCanvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(lebarRT, tinggiRT);

        // Kurva kromatogram, menutupi bagian bawah canvas (atasnya disisakan buat judul)
        var goGrafik = new GameObject("RawImageOffscreen");
        goGrafik.layer = layerOffscreen;
        goGrafik.transform.SetParent(goCanvas.transform, false);
        var rtGrafik = goGrafik.AddComponent<RectTransform>();
        rtGrafik.anchorMin = new Vector2(0f, 0f);
        rtGrafik.anchorMax = new Vector2(1f, 0.85f);
        rtGrafik.offsetMin = Vector2.zero;
        rtGrafik.offsetMax = Vector2.zero;
        var rawGrafik = goGrafik.AddComponent<RawImage>();
        rawGrafik.texture = GeneratorKromatogram.Buat(tR1, tR2, W1, W2);
        rawGrafik.raycastTarget = false;

        // Label sumbu/tR/Rs -- pakai ulang persis method yang sama dengan versi HP, tapi dengan
        // skala font lebih besar karena layar monitor fisiknya kecil di dalam frame kamera AR,
        // jadi tulisan kecil seperti di HP bakal kebaca terlalu mini di situ.
        const float skalaFontMonitor = 2.4f;
        BuatLabelGrafik(rtGrafik, tR1, tR2, W1, W2, rs, layerOffscreen, skalaFontMonitor);

        // Judul kondisi + Rs di bagian atas yang disisakan tadi
        var judul = BuatTeksLabel(canvas.GetComponent<RectTransform>(),
            $"{cairan} + Fase {fase}  |  Rs = {rs:F2} → {KategoriRs(rs)}",
            new Vector2(0.5f, 1f), new Vector2(0, -30), 34, TextAlignmentOptions.Center, layerOffscreen);
        if (judul != null)
        {
            judul.fontStyle = FontStyles.Bold;
            judul.rectTransform.sizeDelta = new Vector2(lebarRT - 40, 60);
        }

        Canvas.ForceUpdateCanvases();
        kamera.Render();

        return rt;
    }

    // Tempelkan tekstur kromatogram yang sama ke layar monitor 3D di Menu4. Mesh Monitor_Screen
    // dari FBX pihak ketiga UV-nya tidak diketahui/tidak rapi (langsung pakai bikin kepotong/zoom),
    // jadi dibuat quad baru dengan UV yang pasti benar (0,0)-(1,1). Ukuran (lebar/tinggi) tetap
    // dihitung otomatis dari bounding box Monitor_Screen (bisa diskala lewat skalaLebar/TinggiLayar3D),
    // tapi posisinya pakai posisiLayar3D (nilai pasti hasil tuning manual di Play Mode), bukan
    // dihitung dari bounds.center lagi.
    void BuatLayarKromatogram3D(Texture tekstur)
    {
        if (monitorScreenRenderer == null || tekstur == null) return;

        Bounds bounds = monitorScreenRenderer.bounds;
        Vector3 ukuran = bounds.size;

        Vector3 sumbuLebar, sumbuTinggi;
        float lebar, tinggi;

        if (ukuran.x <= ukuran.y && ukuran.x <= ukuran.z)
        {
            sumbuLebar = Vector3.up; sumbuTinggi = Vector3.forward;
            lebar = ukuran.y; tinggi = ukuran.z;
        }
        else if (ukuran.y <= ukuran.x && ukuran.y <= ukuran.z)
        {
            sumbuLebar = Vector3.right; sumbuTinggi = Vector3.forward;
            lebar = ukuran.x; tinggi = ukuran.z;
        }
        else
        {
            sumbuLebar = Vector3.right; sumbuTinggi = Vector3.up;
            lebar = ukuran.x; tinggi = ukuran.y;
        }

        lebar  *= skalaLebarLayar3D;
        tinggi *= skalaTinggiLayar3D;

        Vector3 setengahLebar  = sumbuLebar  * (lebar  / 2f);
        Vector3 setengahTinggi = sumbuTinggi * (tinggi / 2f);

        Mesh mesh = new Mesh();
        mesh.vertices = new[]
        {
            -setengahLebar - setengahTinggi,
             setengahLebar - setengahTinggi,
            -setengahLebar + setengahTinggi,
             setengahLebar + setengahTinggi,
        };
        mesh.uv = new[]
        {
            new Vector2(0, 0), new Vector2(1, 0),
            new Vector2(0, 1), new Vector2(1, 1),
        };
        // Dua set segitiga winding berlawanan supaya quad tetap terlihat dari kedua sisi,
        // tanpa perlu tahu arah normal mesh aslinya.
        mesh.triangles = new[] { 0, 2, 1, 1, 2, 3, 0, 1, 2, 1, 3, 2 };
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        var objekLayar = new GameObject("LayarKromatogram3D");
        // Ditempelkan sebagai anak ARCamera (bukan posisi dunia lepas) supaya ikut bergerak
        // menempel kamera persis seperti PCMonitor_Menu4 sendiri -- kalau tidak, begitu HP
        // digerakkan quad ini diam di tempat sementara monitornya ikut geser, jadi kelihatan
        // seperti "grafiknya hilang" padahal cuma lepas dari posisi layar monitornya.
        Transform arCamera = Camera.main != null ? Camera.main.transform : null;
        if (arCamera != null)
        {
            objekLayar.transform.SetParent(arCamera, false);
            objekLayar.transform.localPosition = posisiLayar3D;
            objekLayar.transform.localRotation = Quaternion.identity;
        }
        else
        {
            objekLayar.transform.position = posisiLayar3D;
            objekLayar.transform.rotation = Quaternion.identity;
        }

        var meshFilter = objekLayar.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        var meshRenderer = objekLayar.AddComponent<MeshRenderer>();
        var materialLayar = new Material(Shader.Find("Unlit/Texture"));
        materialLayar.mainTexture = tekstur;
        meshRenderer.material = materialLayar;

        // Sembunyikan mesh layar asli supaya tidak dobel/ketimpa dengan quad baru ini
        monitorScreenRenderer.enabled = false;
    }

    IEnumerator TampilkanHasil(float[] data, string labelKondisi,
                                string sampel)
    {
        float tR1       = data[0];
        float tR2       = data[1];
        float W1        = data[2];
        float W2        = data[3];
        float selisihTR = tR2 - tR1;
        float jumlahW   = W1 + W2;
        float rs        = (2 * selisihTR) / jumlahW;

        // Tampilkan gambar kromatogram lebih dulu (kecuali mode monitor 3D aktif, kartu ini
        // sengaja disembunyikan total di Start() supaya tidak menutupi monitor 3D). Animasi
        // "tumbuh" meluncur naik dari sumbu bawah, bukan langsung muncul statis penuh.
        yield return new WaitForSeconds(0.3f);
        if (imgKromatogram != null && monitorScreenRenderer == null)
            yield return StartCoroutine(AnimasikanGrafikTumbuh(1.1f));

        // Tampilkan panel perhitungan
        yield return new WaitForSeconds(0.4f);
        if (panelHasil != null)
            panelHasil.SetActive(true);

        // Langkah 1 — Kondisi
        yield return new WaitForSeconds(0.3f);
        txtKondisi.gameObject.SetActive(true);
        txtKondisi.text = $"Kondisi: {labelKondisi}";

        // Langkah 2 — tR1
        yield return new WaitForSeconds(0.8f);
        txtLangkah1.gameObject.SetActive(true);
        txtLangkah1.text = $"tR₁ = {tR1:F1} menit  (Komponen 1)";

        // Langkah 3 — tR2
        yield return new WaitForSeconds(0.8f);
        txtLangkah2.gameObject.SetActive(true);
        txtLangkah2.text = $"tR₂ = {tR2:F1} menit  (Komponen 2)";

        // Langkah 4 — Substitusi rumus
        yield return new WaitForSeconds(0.8f);
        txtLangkah3.gameObject.SetActive(true);
        txtLangkah3.text =
            $"Rs = 2({tR2:F1} – {tR1:F1}) / ({W1:F1} + {W2:F1})" +
            $"\n     = 2 × {selisihTR:F1} / {jumlahW:F1}";

        // Langkah 5 — Hasil Rs
        yield return new WaitForSeconds(1.0f);
        txtLangkah4.gameObject.SetActive(true);
        txtLangkah4.text = $"Rs = {rs:F2}";

        // Langkah 6 — Kesimpulan
        yield return new WaitForSeconds(0.8f);
        txtKesimpulan.gameObject.SetActive(true);

        string status;
        Color warna;

        if (rs >= 2.0f)
        {
            status = "✓ SANGAT BAIK  (Rs ≥ 2,0)";
            warna  = new Color(0.1f, 0.7f, 0.1f);
        }
        else if (rs >= 1.5f)
        {
            status = "✓ BAIK  (Rs ≥ 1,5)";
            warna  = new Color(0.1f, 0.7f, 0.1f);
        }
        else if (rs >= 1.0f)
        {
            status = "⚠ KURANG OPTIMAL  (Rs ≥ 1,0)";
            warna  = new Color(1.0f, 0.6f, 0.0f);
        }
        else
        {
            status = "✗ BURUK  (Rs < 1,0)";
            warna  = new Color(0.9f, 0.1f, 0.1f);
        }

        txtKesimpulan.text  = status;
        txtKesimpulan.color = warna;

        // Langkah 7 — Analisis Kualitatif
        yield return new WaitForSeconds(1.0f);
        if (txtKualitatif != null)
        {
            txtKualitatif.gameObject.SetActive(true);

            float tRStandar = (sampel == "Parasetamol") ? 2.8f : 3.2f;
            bool cocok = Mathf.Abs(tR1 - tRStandar) < 0.2f;

            txtKualitatif.text =
                "━━━ ANALISIS KUALITATIF ━━━\n" +
                $"Standar {sampel,-12}: tR = {tRStandar:F1} menit\n" +
                $"Sampel uji         : tR = {tR1:F1} menit\n" +
                (cocok
                    ? $"✓ tR cocok → POSITIF mengandung {sampel}"
                    : $"✗ tR tidak cocok → NEGATIF / bukan {sampel}");

            txtKualitatif.color = cocok
                ? new Color(0.1f, 0.7f, 0.1f)
                : new Color(0.9f, 0.1f, 0.1f);
        }
    }

    void SembunyikanSemuaTeks()
    {
        txtKondisi.gameObject.SetActive(false);
        txtLangkah1.gameObject.SetActive(false);
        txtLangkah2.gameObject.SetActive(false);
        txtLangkah3.gameObject.SetActive(false);
        txtLangkah4.gameObject.SetActive(false);
        txtKesimpulan.gameObject.SetActive(false);
        if (txtKualitatif != null)
            txtKualitatif.gameObject.SetActive(false);
    }

    public void Kembali()
    {
        SceneManager.LoadScene("Menu3");
    }

    public void KembaliKeMenuUtama()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void TombolInformasi()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.menuAsal = "Menu4";
        SceneManager.LoadScene("Informasi");
    }
}