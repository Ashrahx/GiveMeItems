<div align="center">
  <br />
  <a href="https://github.com/Ashrahx/GiveMeItems" target="_blank">
    <img src="https://i.imgur.com/539S0fI.png" alt="GiveMeItems Banner" width="800" />
  </a>
  <br /><br />

  <div>
    <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" alt="C#" />
    <img src="https://img.shields.io/badge/.NET_6.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 6.0" />
    <img src="https://img.shields.io/badge/SMAPI-4.0+-brightgreen?style=for-the-badge" alt="SMAPI" />
    <img src="https://img.shields.io/badge/Stardew_Valley-1.6+-ff69b4?style=for-the-badge" alt="Stardew Valley" />
    <img src="https://img.shields.io/badge/Nexus_Mods-Available-orange?style=for-the-badge" alt="Nexus Mods" />
  </div>

  <br />

  <h1>GiveMeItems</h1>
  <h3><em>Stardew Valley Item Mod</em></h3>

  <p>
    Obtén cualquier objeto al instante en Stardew Valley — sin comandos de consola, sin menús complejos, sin afectar a tus compañeros de multijugador.
  </p>

  <br />

  <a href="https://www.nexusmods.com/stardewvalley/mods/43902" target="_blank">
    <img src="https://img.shields.io/badge/⬇️_Descargar_en_Nexus_Mods-DA8A00?style=for-the-badge&logoColor=white" alt="Descargar en Nexus Mods" height="40" />
  </a>

<br /><br />

</div>

---

## Contenido

- [¿Qué es?](#qué-es)
- [Cómo usar](#cómo-usar)
- [Características](#características)
- [Instalación](#instalación)
- [Compilar desde fuentes](#compilar-desde-fuentes)

---

## ¿Qué es?

Un mod simple para Stardew Valley que te deja obtener cualquier objeto al instante. Presiona una tecla, escribe lo que quieres, y aparece en tu inventario.

La interfaz respeta el estilo visual del juego, y en multijugador solo te afecta a ti — tus compañeros no notan nada.

---

## Cómo usar

1. Presiona `G` en el juego
2. Escribe el nombre o ID del objeto (ej. "Diamond" o "74")
3. Selecciona cantidad y calidad
4. Presiona Enter

---

## Características

- Búsqueda rápida por nombre o ID
- Interfaz estilo Stardew Valley
- Seguro para multijugador (solo te afecta a ti)
- Configurable con `config.json`

---

## Instalación

1. Descarga desde [Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/43902)
2. Extrae la carpeta en tu directorio de mods:
   - Windows: `%APPDATA%\StardewValley\Mods\`
   - Linux/macOS: `~/.config/StardewValley/Mods/`
3. Abre el juego con SMAPI
4. Presiona `G` y listo

Necesitas [Stardew Valley 1.6+](https://store.steampowered.com/app/413150/Stardew_Valley/) y [SMAPI 4.0+](https://smapi.io/).

---

## Compilar desde fuentes

Si quieres modificar el código:

```bash
git clone https://github.com/Ashrahx/GiveMeItems.git
cd GiveMeItems
```

Ajusta `<GamePath>` en `GiveMeItems.csproj` con la ruta a tu Stardew Valley, luego:

```bash
dotnet build
```

Copia los archivos compilados de `bin/Debug/net6.0/` a tu carpeta de mods. Necesitas [.NET SDK 6.0](https://dotnet.microsoft.com/download/dotnet/6.0).
