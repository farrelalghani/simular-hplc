using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Menu1 : MonoBehaviour
{
    public TextMeshProUGUI txtDeskripsi;
    public Image bgParasetamol;
    public Image bgKafein;
    public GameObject checkParasetamol;
    public GameObject checkKafein;

    private string sampelTerpilih = "Parasetamol";

    private static readonly Color WarnaAktif = new Color(0.101961f, 0.603922f, 0.682353f, 1f);
    private static readonly Color WarnaNonAktif = new Color(0.075f, 0.122f, 0.2f, 1f);

    private const string DeskripsiParasetamol =
        "Parasetamol bersifat polar dan akan menunjukkan waktu retensi yang lebih singkat pada Fase Terbalik dibandingkan senyawa non-polar.";
    private const string DeskripsiKafein =
        "Kafein bersifat kurang polar dibandingkan Parasetamol, sehingga waktu retensinya relatif lebih panjang pada Fase Terbalik.";

    void Start()
    {
        PilihParasetamol();
    }

    public void PilihParasetamol() { SetSampel("Parasetamol"); }
    public void PilihKafein() { SetSampel("Kafein"); }

    private void SetSampel(string nama)
    {
        sampelTerpilih = nama;

        if (bgParasetamol != null) bgParasetamol.color = nama == "Parasetamol" ? WarnaAktif : WarnaNonAktif;
        if (bgKafein != null) bgKafein.color = nama == "Kafein" ? WarnaAktif : WarnaNonAktif;
        if (checkParasetamol != null) checkParasetamol.SetActive(nama == "Parasetamol");
        if (checkKafein != null) checkKafein.SetActive(nama == "Kafein");
        if (txtDeskripsi != null) txtDeskripsi.text = nama == "Parasetamol" ? DeskripsiParasetamol : DeskripsiKafein;
    }

    public void LanjutKeSimulasi()
    {
        GameManager.Instance.cairanDipilih = sampelTerpilih;
        SceneManager.LoadScene("MenuFaseGerak");
    }

    public void Kembali()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void TombolInformasi()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.menuAsal = "Menu1";
        SceneManager.LoadScene("Informasi");
    }
}