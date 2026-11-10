using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class PartikelIkutiJalur : MonoBehaviour
{
    [Tooltip("Titik-titik jalur yang dilewati partikel, berurutan dari ujung awal ke ujung akhir selang (mis. Selang_03 -> Selang_04 -> Selang_06 -> ...).")]
    public Transform[] waypoints;

    private ParticleSystem ps;
    private ParticleSystem.Particle[] partikel;
    private Vector3[] posisiWaypoint;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();

        // Supaya posisi partikel bisa disamakan langsung dengan posisi world waypoint,
        // tanpa perlu konversi ruang lokal/world tiap frame.
        var main = ps.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        HitungPosisiWaypoint();
    }

    // Pakai titik tengah bounding box mesh (bukan Transform.position) karena objek
    // hasil pisah material di Blender (Selang_XX) semuanya mewarisi pivot yang sama
    // dari mesh gabungan asalnya -- Transform.position-nya identik walau bentuk
    // visualnya beda-beda, jadi tidak bisa dipakai langsung sebagai titik jalur.
    void HitungPosisiWaypoint()
    {
        if (waypoints == null) return;

        posisiWaypoint = new Vector3[waypoints.Length];
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;

            Renderer renderer = waypoints[i].GetComponent<Renderer>();
            posisiWaypoint[i] = renderer != null ? renderer.bounds.center : waypoints[i].position;
        }
    }

    void LateUpdate()
    {
        if (!enabled || ps == null) return;
        if (posisiWaypoint == null || posisiWaypoint.Length < 2) return;

        int maxPartikel = ps.main.maxParticles;
        if (partikel == null || partikel.Length < maxPartikel)
            partikel = new ParticleSystem.Particle[maxPartikel];

        int jumlah = ps.GetParticles(partikel);
        if (jumlah == 0) return;

        float lifetimeAwal = Mathf.Max(ps.main.startLifetime.constant, 0.0001f);

        for (int i = 0; i < jumlah; i++)
        {
            float umurNormal = 1f - Mathf.Clamp01(partikel[i].remainingLifetime / lifetimeAwal);
            partikel[i].position = PosisiDiJalur(umurNormal);
        }

        ps.SetParticles(partikel, jumlah);
    }

    Vector3 PosisiDiJalur(float t)
    {
        int jumlahSegmen = posisiWaypoint.Length - 1;
        float posisiSkala = Mathf.Clamp01(t) * jumlahSegmen;
        int segmen = Mathf.Clamp(Mathf.FloorToInt(posisiSkala), 0, jumlahSegmen - 1);
        float localT = posisiSkala - segmen;

        Vector3 p0 = posisiWaypoint[Mathf.Max(segmen - 1, 0)];
        Vector3 p1 = posisiWaypoint[segmen];
        Vector3 p2 = posisiWaypoint[Mathf.Min(segmen + 1, posisiWaypoint.Length - 1)];
        Vector3 p3 = posisiWaypoint[Mathf.Min(segmen + 2, posisiWaypoint.Length - 1)];

        return CatmullRomCentripetal(p0, p1, p2, p3, localT);
    }

    // Centripetal Catmull-Rom (bukan versi "uniform" seperti sebelumnya). Versi uniform gampang
    // overshoot/melebar keluar garis lurus antar titik kalau selang berbelok tajam atau jarak
    // antar Selang_XX tidak rata -- itu penyebab partikel kelihatan meliuk keluar dari mesh selang.
    // Parameterisasi berbasis jarak (alpha 0.5) ini menahan kurva supaya tetap dekat garis lurus
    // antar titik, sehingga tidak overshoot di tikungan.
    Vector3 CatmullRomCentripetal(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        const float alpha = 0.5f;
        const float epsilon = 0.0001f;

        float t0 = 0f;
        float t1 = t0 + Mathf.Pow(Vector3.Distance(p0, p1), alpha) + epsilon;
        float t2 = t1 + Mathf.Pow(Vector3.Distance(p1, p2), alpha) + epsilon;
        float t3 = t2 + Mathf.Pow(Vector3.Distance(p2, p3), alpha) + epsilon;

        float tt = Mathf.Lerp(t1, t2, t);

        Vector3 A1 = (t1 - tt) / (t1 - t0) * p0 + (tt - t0) / (t1 - t0) * p1;
        Vector3 A2 = (t2 - tt) / (t2 - t1) * p1 + (tt - t1) / (t2 - t1) * p2;
        Vector3 A3 = (t3 - tt) / (t3 - t2) * p2 + (tt - t2) / (t3 - t2) * p3;

        Vector3 B1 = (t2 - tt) / (t2 - t0) * A1 + (tt - t0) / (t2 - t0) * A2;
        Vector3 B2 = (t3 - tt) / (t3 - t1) * A2 + (tt - t1) / (t3 - t1) * A3;

        return (t2 - tt) / (t2 - t1) * B1 + (tt - t1) / (t2 - t1) * B2;
    }
}