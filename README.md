<div align="center">
  <br />
    <a href="https://github.com/tu-usuario/GiveMeItems" target="_blank">
      <img src="https://via.placeholder.com/800x400.png?text=GiveMeItems+Banner" alt="Project Banner">
    </a>
  <br />

  <div>
    <img alt="Static Badge" src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white">
        <img src="https://img.shields.io/badge/.NET_6.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
        <img src="https://img.shields.io/badge/SMAPI-4.0+-brightgreen?style=for-the-badge" alt="SMAPI" />
    <img alt="Static Badge" src="https://img.shields.io/badge/Stardew_Valley-1.6+-ff69b4?style=for-the-badge">
  </div>

  <h3 align="center">GiveMeItems - Stardew Valley Item Spawner</h3>

   <div align="center">
     ¡Obtén cualquier objeto al instante en Stardew Valley sin afectar a otros jugadores en multijugador!
    </div>
</div>

## 📋 <a name="table">Table of Contents</a>

1. ✨ [Introduction](#introduction)
2. ⚙️ [Tech Stack](#tech-stack)
3. 🔋 [Features](#features)
4. 🤸 [Quick Start](#quick-start)
5. 🔗 [Item IDs & Resources](#links)
6. 🚀 [More](#more)

## 🚨 Mod Showcase

Este repositorio contiene el código fuente de **GiveMeItems**, un mod diseñado para ser ligero, rápido y seguro para partidas cooperativas. 

Si solo quieres usar el mod sin tocar código, puedes descargarlo directamente desde nuestra página oficial en Nexus Mods o seguir las instrucciones de instalación más abajo. ¡Ideal para constructores y creadores de contenido!

<a href="https://nexusmods.com/stardewvalley/mods/TULINK" target="_blank"><img src="https://via.placeholder.com/800x100.png?text=Descargar+en+Nexus+Mods" /></a>

## <a name="introduction">✨ Introduction</a>

**GiveMeItems** te permite presionar la tecla **G** en el juego para abrir un menú personalizado, escribir el nombre o el ID de cualquier objeto y recibirlo directamente en tu inventario. 

A diferencia de los menús de trampas masivos, este mod está enfocado en una sola tarea, cuenta con una interfaz que respeta el estilo visual (Vanilla) del juego y funciona de manera estrictamente local en multijugador (solo te da el objeto a ti, sin enviar comandos ni afectar el mundo compartido).

## <a name="tech-stack">⚙️ Tech Stack</a>

- **[C#](https://learn.microsoft.com/en-us/dotnet/csharp/)** es el lenguaje de programación principal utilizado para desarrollar la lógica del mod.
- **[.NET 6.0](https://dotnet.microsoft.com/)** es el framework requerido para compilar y ejecutar mods en la versión 1.6 de Stardew Valley.
- **[SMAPI (Stardew Modding API)](https://smapi.io/)** es el cargador de mods subyacente que permite interceptar eventos del juego, botones y manipular el inventario.
- **[MonoGame](https://monogame.net/)** es el framework de renderizado de Stardew Valley, utilizado aquí para dibujar nuestra interfaz gráfica personalizada (UI) con `SpriteBatch` y texturas nativas.

## <a name="features">🔋 Features</a>

👉 **Obtención Instantánea**: Presiona `G` (configurable) para abrir el menú de texto, escribe lo que necesitas y presiona Enter.

👉 **Búsqueda Inteligente**: Soporta la entrada del ID numérico exacto (ej. `74`) o búsqueda por nombre parcial (ej. `Diamante`, `Madera`).

👉 **Seguro para Multijugador**: Los objetos se entregan mediante métodos locales (`addItemToInventory`). Si no eres el host, no afecta a tus compañeros ni rompe la economía global a menos que tú lo decidas.

👉 **Menú Integrado (Vanilla UI)**: Interfaz gráfica construida desde cero que simula perfectamente las ventanas, fuentes y cursores originales de Stardew Valley.

👉 **Altamente Configurable**: Una vez ejecutado el mod por primera vez, se generará un archivo `config.json` donde podrás cambiar la tecla de activación (`ActivateKey`) y la cantidad de objetos por defecto (`DefaultQuantity`).

## <a name="quick-start">🤸 Quick Start</a>

Sigue estos pasos para compilar e instalar el proyecto localmente en tu máquina.

**Prerequisites**

Asegúrate de tener instalado lo siguiente:
- [Stardew Valley 1.6+](https://store.steampowered.com/app/413150/Stardew_Valley/)
- [SMAPI 4.0+](https://smapi.io/)
- [.NET SDK 6.0](https://dotnet.microsoft.com/download/dotnet/6.0) (Solo si vas a compilar el código fuente)

**Cloning the Repository**

```bash
git clone [https://github.com/tu-usuario/GiveMeItems.git](https://github.com/tu-usuario/GiveMeItems.git)
cd GiveMeItems
````

**Configuration**

Abre el archivo `GiveMeItems.csproj` y asegúrate de que la ruta `<GamePath>` coincida con la instalación de tu juego:

```xml
<GamePath>C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley</GamePath>
```

**Building the Project**

Compila el proyecto usando la CLI de .NET:

```bash
dotnet build
```

**Installation**
Copia el contenido generado en `bin/Debug/net6.0/` a tu carpeta de **Mods** de Stardew Valley (ej. `%APPDATA%\StardewValley\Mods\` o la ruta de Steam). ¡Inicia el juego a través de SMAPI y presiona `G`\!

## \<a name="links"\>🔗 Item IDs & Resources\</a\>

Puedes encontrar todos los IDs numéricos actualizados para generar tus objetos en la wiki oficial de modding.

\<a href="https://stardewvalleywiki.com/Modding:Item\_queries" target="\_blank"\>
\<img src="https://www.google.com/search?q=https://via.placeholder.com/800x100.png%3Ftext%3DVer%2BLista%2Bde%2BIDs%2Ben%2Bla%2BWiki" alt="Wiki Link Banner"\>
\</a\>

## \<a name="more"\>🚀 More\</a\>

**¿Te ha sido útil este mod?**

Si disfrutaste usar este mod o explorar su código fuente, considera darle una ⭐ al repositorio y revisar la página de Nexus Mods. Si encuentras algún error o tienes sugerencias, ¡siéntete libre de abrir un *Issue* o un *Pull Request*\!

