using UnityEngine;
using TMPro;

public class TombolMusik : MonoBehaviour
{
    public TextMeshProUGUI txtIkon;

    void Start()
    {
        PerbaruiIkon();
    }

    public void ToggleMusik()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ToggleMusik();

        PerbaruiIkon();
    }

    void PerbaruiIkon()
    {
        if (txtIkon == null) return;
        bool dibisukan = GameManager.Instance != null && GameManager.Instance.MusikDibisukan;
        // Pakai simbol yang sama seperti yang sudah dipakai di UI lain (✓/✗ di KalkulasiRs),
        // bukan emoji speaker, karena font TMP default proyek ini tidak menjamin dukungan emoji.
        txtIkon.text = dibisukan ? "✗" : "♪"; // ✗ : ♪
    }
}