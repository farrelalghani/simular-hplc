using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuFaseGerak : MonoBehaviour
{
    [Header("UI")]
    public Slider sliderFaseGerak;
    public TextMeshProUGUI txtKomposisi;
    public TextMeshProUGUI txtKategori;
    public Image bgKategori;

    private static readonly Color WarnaKurangOptimal = new Color(245f / 255f, 158f / 255f, 11f / 255f, 1f);
    private static readonly Color WarnaBaik = new Color(0.101961f, 0.603922f, 0.682353f, 1f);
    private static readonly Color WarnaSangatBaik = new Color(34f / 255f, 197f / 255f, 94f / 255f, 1f);

    void Start()
    {
        if (sliderFaseGerak != null)
            sliderFaseGerak.value = 1;

        OnSliderValueChanged();
    }

    public void OnSliderValueChanged()
    {
        if (sliderFaseGerak == null) return;

        int posisi = Mathf.RoundToInt(sliderFaseGerak.value);

        string komposisi;
        string kategori;
        Color warna;

        switch (posisi)
        {
            case 0:
                komposisi = "Metanol 40% : Air 60%";
                kategori = "Kurang Optimal";
                warna = WarnaKurangOptimal;
                break;
            case 2:
                komposisi = "Metanol 80% : Air 20%";
                kategori = "Sangat Baik";
                warna = WarnaSangatBaik;
                break;
            default:
                komposisi = "Metanol 70% : Air 30%";
                kategori = "Baik";
                warna = WarnaBaik;
                break;
        }

        if (txtKomposisi != null) txtKomposisi.text = komposisi;
        if (txtKategori != null) txtKategori.text = kategori;
        if (bgKategori != null) bgKategori.color = warna;
    }

    public void LanjutKeMenu2()
    {
        if (sliderFaseGerak != null && GameManager.Instance != null)
        {
            int posisi = Mathf.RoundToInt(sliderFaseGerak.value);
            string skenario = posisi == 0 ? "Kurang Optimal" : (posisi == 2 ? "Sangat Baik" : "Baik");
            GameManager.Instance.skenarioFaseGerak = skenario;
        }

        SceneManager.LoadScene("Menu2");
    }

    public void Kembali()
    {
        SceneManager.LoadScene("Menu1");
    }
}