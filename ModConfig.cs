using StardewModdingAPI;

namespace GiveMeItems
{
    /// <summary>Configuración del mod. Se edita en config.json dentro de la carpeta del mod.</summary>
    public class ModConfig
    {
        /// <summary>Tecla para abrir el menú de ítems. Por defecto: G</summary>
        public SButton ActivateKey { get; set; } = SButton.G;

        /// <summary>Cantidad de ítems que recibirás cada vez. Por defecto: 1</summary>
        public int DefaultQuantity { get; set; } = 1;
    }
}
