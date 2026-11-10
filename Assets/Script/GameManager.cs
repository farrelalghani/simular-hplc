using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Satu-satunya instance GameManager di seluruh app
    public static GameManager Instance;

    // Menyimpan pilihan user yang dibawa lintas scene
    public string cairanDipilih = "Parasetamol";     // default Cairan A
    public string faseDipilih   = "Normal"; // default Fase Normal
    public string menuAsal      = "Menu1";
    public string skenarioFaseGerak = "Baik"; // default skenario Baik (posisi tengah slider)

    // Musik latar, terus diputar tanpa putus dari MainMenu sampai Menu4
    public AudioClip musikLatar;
    private AudioSource audioSource;

    void Awake()
    {
        // Kalau belum ada GameManager, jadikan ini Instance-nya
        if (Instance == null)
        {
            Instance = this;
            // Jangan hancurkan saat pindah scene
            DontDestroyOnLoad(gameObject);

            // Setup musik latar, cuma sekali karena GameObject ini yang bertahan
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.volume = 0.5f;
            audioSource.clip = musikLatar;
            audioSource.Play();
        }
        else
        {
            // Kalau sudah ada, hancurkan yang duplikat
            Destroy(gameObject);
        }
    }

    public bool MusikDibisukan => audioSource != null && audioSource.mute;

    public void ToggleMusik()
    {
        if (audioSource == null) return;

        if (audioSource.mute)
            audioSource.mute = false;
        else
            audioSource.mute = true;
    }

    public void SetVolumeMusik(float volume)
    {
        if (audioSource == null) return;

        audioSource.volume = volume;
    }
}