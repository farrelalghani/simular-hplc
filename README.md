# SimulAR HPLC (High Performance Liquid Chromatography)

Aplikasi Augmented Reality (AR) Android untuk simulasi interaktif prosedur laboratorium kimia instrumen High Performance Liquid Chromatography (HPLC).

> **Tugas Akhir / Skripsi**  
> Program Studi Teknik Informatika, Universitas Komputer Indonesia (UNIKOM) Bandung  
> Penulis: **Farrel Muhammad Alghani**

---

## 🎯 Latar Belakang & Fitur
Peralatan laboratorium HPLC membutuhkan biaya operasional dan risiko instrumen fisik yang tinggi. Aplikasi ini memfasilitasi mahasiswa/pelajar untuk:
- **Visualisasi Instrumen 3D Interaktif:** Menampilkan model 3D HPLC berbasis Vuforia marker tracking secara presisi.
- **Mode X-Ray:** Menginspeksi alur sirkulasi eluen/fluida di dalam komponen internal mesin secara transparan.
- **Simulasi Prosedural Aman:** Praktikum mandiri tanpa risiko kerusakan alat nyata.
- **Performa Ringan:** Asset 3D dimodelkan dan dioptimasi di Blender untuk mencapai 60 FPS di platform Android.

## 🛠️ Tech Stack & Requirements
- **Game Engine:** Unity 2022.3.62f3 LTS
- **AR Engine:** Vuforia Engine 11.4.4
- **3D Modeling & Texturing:** Blender 4.x
- **Bahasa Pemrograman:** C# (.NET / Mono)
- **Target Platform:** Android SDK (API 26+)

## 📂 Struktur Project
- `Assets/` : Script C#, Scenes simulasi, 3D Models, Materials, Prefabs, Audio, UI.
- `Packages/` : Dependencies & Vuforia SDK.
- `ProjectSettings/` : Konfigurasi project & build settings Unity.

---
Dikembangkan oleh **Farrel Muhammad Alghani** (S.Kom, UNIKOM Bandung)
