# GiveMeItems — Mod de Stardew Valley

Presiona **G** en el juego para recibir cualquier ítem que escribas.  
Solo te lo da **a ti** (funciona en multijugador sin afectar a otros jugadores).

---

## 📋 Requisitos

| Requisito | Versión mínima |
|-----------|---------------|
| Stardew Valley | 1.6+ |
| SMAPI | 4.0+ |
| .NET SDK | 6.0 (para compilar) |

---

## 🚀 Instalación rápida (sin compilar)

Si solo quieres usar el mod sin tocar código:

1. Instala [SMAPI](https://smapi.io/) si no lo tienes.
2. Copia la carpeta `GiveMeItems` completa a:
   - **Windows:** `%APPDATA%\StardewValley\Mods\`
   - **macOS:** `~/.config/StardewValley/Mods/`
   - **Linux:** `~/.config/StardewValley/Mods/`
3. Lanza el juego con SMAPI.

---

## 🔨 Compilar desde código fuente

### 1. Editar la ruta del juego

Abre `GiveMeItems.csproj` y cambia `<GamePath>` a donde tengas instalado Stardew Valley:

```xml
<GamePath>C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley</GamePath>
```

### 2. Compilar

```bash
cd GiveMeItems
dotnet build
```

### 3. Copiar a la carpeta de Mods

Copia el contenido de `bin/Debug/net6.0/` a la carpeta Mods de Stardew Valley.

---

## 🎮 Uso en el juego

1. Carga tu partida.
2. Presiona **G** (o la tecla que hayas configurado).
3. Escribe el **ID numérico** o el **nombre** del ítem.
4. Presiona **Enter**.

### Ejemplos de IDs comunes

| ID  | Ítem          |
|-----|---------------|
| 74  | Diamante      |
| 72  | Diamante (alt)|
| 388 | Madera        |
| 390 | Piedra        |
| 466 | Fibra de coco |
| 428 | Tela          |
| 787 | Batería       |
| 709 | Tronco duro   |
| 60  | Esmeralda     |
| 62  | Aguamarina    |
| 64  | Rubí          |
| 66  | Amatista      |
| 68  | Topacio       |
| 70  | Jade          |

Puedes encontrar todos los IDs en: https://stardewvalleywiki.com/Modding:Item_queries

---

## ⚙️ Configuración

Después de lanzar el juego una vez, se creará el archivo `config.json` en la carpeta del mod:

```json
{
  "ActivateKey": "G",
  "DefaultQuantity": 1
}
```

| Campo | Descripción |
|-------|-------------|
| `ActivateKey` | Tecla para abrir el menú |
| `DefaultQuantity` | Cuántos ítems recibes cada vez |

### Teclas válidas
`A-Z`, `F1-F12`, `NumPad0-9`, `Tab`, `Space`, `OemTilde` (~), etc.  
Lista completa: https://stardewvalleywiki.com/Modding:Player_Guide/Key_Bindings

---

## 🌐 Multijugador

- ✅ Solo **tú** recibes el ítem (el jugador host o el jugador local).
- ✅ No envía nada a otros jugadores.
- ✅ No modifica el mundo compartido.

---

## 📁 Estructura del proyecto

```
GiveMeItems/
├── ModEntry.cs          ← Lógica principal del mod
├── ItemInputMenu.cs     ← Menú de texto personalizado
├── ModConfig.cs         ← Configuración (tecla, cantidad)
├── GiveMeItems.csproj   ← Proyecto .NET
├── manifest.json        ← Metadatos del mod para SMAPI
└── README.md            ← Este archivo
```
