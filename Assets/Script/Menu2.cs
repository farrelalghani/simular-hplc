using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu2 : MonoBehaviour
{
    [Header("Referensi Objek 3D")]
    public GameObject objekInjeksi;   // syringe (dulu botol sampel)
    public Transform targetInjector;
    public GameObject botolSampel;    // botol sampel, animasi masuk ke mesin bersamaan dengan syringe
    public Transform targetBotol;

    [Header("UI")]
    public GameObject buttonSuntikkan;
    public GameObject textPetunjuk;
    public GameObject scanFrame;
    public TextMeshProUGUI txtInfoCairan;

    [Header("Pengaturan Animasi")]
    public float kecepatanAnimasi = 1.5f;
    public float delaySebelumPindah = 0.8f;

void Start()
{
    if (GameManager.Instance != null)
    {
        string sampel = GameManager.Instance.cairanDipilih;

        if (sampel == "Parasetamol")
            txtInfoCairan.text = "Sampel: Parasetamol";
        else if (sampel == "Kafein")
            txtInfoCairan.text = "Sampel: Kafein";
        else
            txtInfoCairan.text = "Sampel: -";
    }
}

    public void SuntikkanSampel()
    {
        buttonSuntikkan.SetActive(false);
        StartCoroutine(AnimasiInjeksi());
    }

    IEnumerator AnimasiInjeksi()
    {
        Vector3 posisiAwalSyringe = objekInjeksi.transform.position;
        Vector3 posisiTargetSyringe = targetInjector != null
            ? targetInjector.position
            : posisiAwalSyringe + new Vector3(0.15f, 0f, 0f);

        Vector3 posisiAwalBotol = botolSampel != null ? botolSampel.transform.position : Vector3.zero;
        Vector3 posisiTargetBotol = (botolSampel != null && targetBotol != null)
            ? targetBotol.position
            : posisiAwalBotol + new Vector3(0f, -0.15f, 0f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * kecepatanAnimasi;

            objekInjeksi.transform.position =
                Vector3.Lerp(posisiAwalSyringe, posisiTargetSyringe, t);

            if (botolSampel != null)
                botolSampel.transform.position =
                    Vector3.Lerp(posisiAwalBotol, posisiTargetBotol, t);

            yield return null;
        }

        yield return new WaitForSeconds(0.3f);
        objekInjeksi.SetActive(false);
        if (botolSampel != null)
            botolSampel.SetActive(false);
        yield return new WaitForSeconds(delaySebelumPindah);
        SceneManager.LoadScene("Menu3");
    }

    public void PadaMarkerDitemukan()
    {
        if (textPetunjuk != null) textPetunjuk.SetActive(false);
        if (scanFrame != null) scanFrame.SetActive(false);
    }

    public void PadaMarkerHilang()
    {
        if (textPetunjuk != null) textPetunjuk.SetActive(true);
        if (scanFrame != null) scanFrame.SetActive(true);
    }

    public void Kembali()
    {
        SceneManager.LoadScene("Menu1");
    }

    public void Informasi()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.menuAsal = "Menu2";
        SceneManager.LoadScene("Informasi");
    }
}