using StardewModdingAPI;

namespace GiveMeItems
{
    /// <summary>Mod configuration. Edit via config.json or the in-game menu (requires Generic Mod Config Menu).</summary>
    public class ModConfig
    {
        /// <summary>Key to open the item menu. Default: G</summary>
        public SButton ActivateKey { get; set; } = SButton.G;

        /// <summary>Default item quantity. Default: 1</summary>
        public int DefaultQuantity { get; set; } = 1;
    }
}