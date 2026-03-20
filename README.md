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

  <h1>✨ GiveMeItems</h1>
  <h3><em>Stardew Valley Item Spawner Mod</em></h3>

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

## 📋 Tabla de Contenidos

1. [✨ ¿Qué es GiveMeItems?](#-qué-es-givemeitems)
2. [🎮 ¿Cómo funciona?](#-cómo-funciona)
3. [🔋 Características](#-características)
4. [⚙️ Tech Stack](#️-tech-stack)
5. [🤸 Guía de Instalación](#-guía-de-instalación)
   - [Instalación Simple (sin código)](#instalación-simple-sin-código)
   - [Compilar desde el código fuente](#compilar-desde-el-código-fuente)
6. [🔗 IDs de Objetos y Recursos](#-ids-de-objetos-y-recursos)
7. [🤝 Contribuir](#-contribuir)

---

## ✨ ¿Qué es GiveMeItems?

**GiveMeItems** es un mod para Stardew Valley enfocado en una sola cosa y hacerla bien: darte el objeto que necesitas, cuando lo necesitas.

A diferencia de los menús de trampas masivos, este mod tiene una interfaz minimalista que respeta el estilo visual original del juego (**Vanilla UI**) y opera de forma **estrictamente local** en sesiones multijugador — tus compañeros no se ven afectados.

> 💡 **Ideal para**: constructores de granjas, creadores de contenido, jugadores que quieren testear builds, o simplemente quien quiera pasar de buscar materiales.

---

## 🎮 ¿Cómo funciona?

```
1. Presiona  G  mientras juegas
         ↓
2. Escribe el nombre o ID del objeto
   Ej: "Diamond"  o  "74"
         ↓
3. Presiona  Enter
         ↓
4. ¡El objeto aparece en tu inventario!
```

---

## 🔋 Características

| Característica | Descripción |
|---|---|
| ⚡ **Obtención Instantánea** | Presiona `G`, escribe, Enter. Sin ventanas extra, sin pasos innecesarios. |
| 🔍 **Búsqueda Inteligente** | Busca por ID numérico exacto (`74`) o por nombre parcial (`Diamond`, `Wood`). |
| 👥 **Seguro para Multijugador** | Los objetos se entregan solo a ti mediante `addItemToInventory` local. No afecta el mundo compartido. |
| 🎨 **Vanilla UI** | La interfaz imita fielmente las ventanas, fuentes y cursores originales de Stardew Valley. |
| ⚙️ **Configurable** | Al ejecutar el mod por primera vez se genera un `config.json`. Desde ahí puedes cambiar: |
| | → `ActivateKey` — la tecla para abrir el menú (por defecto: `G`) |
| | → `DefaultQuantity` — la cantidad de objetos entregados por defecto |

---

## ⚙️ Tech Stack

- **[C#](https://learn.microsoft.com/en-us/dotnet/csharp/)** — lenguaje principal del mod.
- **[.NET 6.0](https://dotnet.microsoft.com/)** — framework requerido para mods de Stardew Valley 1.6.
- **[SMAPI](https://smapi.io/)** — cargador de mods que permite interceptar eventos, botones y manipular el inventario del juego.
- **[MonoGame](https://monogame.net/)** — framework de renderizado de Stardew Valley, usado para dibujar la UI personalizada con `SpriteBatch` y texturas nativas.

---

## 🤸 Guía de Instalación

### Instalación Simple (sin código)

Si solo quieres **usar** el mod, no necesitas compilar nada:

1. Asegúrate de tener [Stardew Valley 1.6+](https://store.steampowered.com/app/413150/Stardew_Valley/) y [SMAPI 4.0+](https://smapi.io/) instalados.
2. Descarga el mod desde **[Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/43902)**.
3. Extrae la carpeta `GiveMeItems` dentro de tu directorio de mods:
   - **Windows**: `%APPDATA%\StardewValley\Mods\`
   - **Linux / macOS**: `~/.config/StardewValley/Mods/`
4. Inicia el juego a través de **SMAPI** y presiona `G` en el juego. ¡Listo!

---

### Compilar desde el código fuente

Si quieres modificar o explorar el código, sigue estos pasos:

**Prerrequisitos**

- [Stardew Valley 1.6+](https://store.steampowered.com/app/413150/Stardew_Valley/)
- [SMAPI 4.0+](https://smapi.io/)
- [.NET SDK 6.0](https://dotnet.microsoft.com/download/dotnet/6.0)

**1. Clonar el repositorio**

```bash
git clone https://github.com/Ashrahx/GiveMeItems.git
cd GiveMeItems
```

**2. Configurar la ruta del juego**

Abre `GiveMeItems.csproj` y ajusta `<GamePath>` para que apunte a tu instalación de Stardew Valley:

```xml
<!-- Windows (Steam, ruta típica) -->
<GamePath>C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley</GamePath>

<!-- Linux (Steam) -->
<!-- <GamePath>~/.steam/steam/steamapps/common/Stardew Valley</GamePath> -->
```

**3. Compilar**

```bash
dotnet build
```

**4. Instalar**

Copia el contenido generado en `bin/Debug/net6.0/` a tu carpeta de Mods de Stardew Valley e inicia el juego con SMAPI.

---

## 🔗 IDs de Objetos y Recursos

¿No sabes el nombre o ID del objeto que buscas? Consulta la wiki oficial de modding de Stardew Valley:

- 📖 **[Wiki de Modding — Item Queries](https://stardewvalleywiki.com/Modding:Item_queries)** — Lista completa y actualizada de IDs de objetos.
- 📖 **[Stardew Valley Wiki — Items](https://stardewvalleywiki.com/Items)** — Referencia general de todos los objetos del juego.

---

## 🤝 Contribuir

¿Encontraste un bug o tienes una idea para mejorar el mod?

- ⭐ **Dale una estrella** al repositorio si te fue útil.
- 🐛 **Abre un Issue** si encuentras algún error o comportamiento inesperado.
- 🔧 **Abre un Pull Request** si quieres contribuir directamente con código.
- 💬 **Deja un comentario** en la página de [Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/43902) con feedback o sugerencias.

Todo tipo de contribución es bienvenida. ¡Gracias por usar GiveMeItems! 🌾
