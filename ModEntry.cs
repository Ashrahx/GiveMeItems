using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Linq;

namespace GiveMeItems
{
    public class ModEntry : Mod
    {
        private ModConfig Config = null!;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            Monitor.Log(Helper.Translation.Get("log.ready", new { key = Config.ActivateKey }), LogLevel.Info);

            var gmcm = Helper.ModRegistry.GetApi<IGMCMApi>("spacechase0.GenericModConfigMenu");
            if (gmcm is null) return;

            gmcm.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );
            gmcm.AddKeybind(
                mod: ModManifest,
                getValue: () => Config.ActivateKey,
                setValue: v => Config.ActivateKey = v,
                name: () => Helper.Translation.Get("config.key.name"),
                tooltip: () => Helper.Translation.Get("config.key.tooltip")
            );
            gmcm.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.DefaultQuantity,
                setValue: v => Config.DefaultQuantity = v,
                name: () => Helper.Translation.Get("config.quantity.name"),
                tooltip: () => Helper.Translation.Get("config.quantity.tooltip"),
                min: 1,
                max: 999
            );
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            if (Game1.activeClickableMenu != null) return;
            if (e.Button == Config.ActivateKey)
                Game1.activeClickableMenu = new ItemInputMenu(GiveItem, Config.DefaultQuantity, Helper.Translation);
        }

        private void GiveItem(string input, int quantity, int quality)
        {
            if (string.IsNullOrWhiteSpace(input)) return;

            Item? item = null;
            string trimmed = input.Trim();

            if (trimmed.StartsWith("("))
            {
                try { item = ItemRegistry.Create(trimmed, quantity); }
                catch { item = null; }

                if (item == null)
                {
                    ShowLocalError(Helper.Translation.Get("error.create_failed"));
                    return;
                }
            }
            else if (int.TryParse(trimmed, out int itemId))
            {
                try { item = ItemRegistry.Create(itemId.ToString(), quantity); }
                catch { item = null; }

                if (item == null)
                {
                    ShowLocalError(Helper.Translation.Get("error.id_not_found", new { id = itemId }));
                    return;
                }
            }
            else
            {
                Item? exactMatch = null;
                Item? partialMatch = null;

                foreach (var type in ItemRegistry.ItemTypes)
                {
                    foreach (var rid in type.GetAllIds())
                    {
                        var candidate = ItemRegistry.Create(rid, 1);
                        if (candidate == null) continue;
                        if (candidate.DisplayName.Equals(trimmed, StringComparison.OrdinalIgnoreCase))
                        { exactMatch = candidate; break; }
                        if (partialMatch == null && candidate.DisplayName.Contains(trimmed, StringComparison.OrdinalIgnoreCase))
                            partialMatch = candidate;
                    }
                    if (exactMatch != null) break;
                }

                item = exactMatch ?? partialMatch;

                if (item == null)
                {
                    ShowLocalError(Helper.Translation.Get("error.name_not_found", new { name = trimmed }));
                    return;
                }

                item = ItemRegistry.Create(item.ItemId, quantity);
                if (item == null)
                {
                    ShowLocalError(Helper.Translation.Get("error.create_failed"));
                    return;
                }
            }


            item.Stack = quantity;

            if (item is StardewValley.Object obj)
                obj.Quality = quality;

            if (item is Tool tool)
            {
                bool isNonRemovable = tool is Pickaxe or Axe or WateringCan || 
                                      tool.Name.Contains("Hoe", StringComparison.OrdinalIgnoreCase);

                if (isNonRemovable)
                {
                    for (int i = 0; i < Game1.player.Items.Count; i++)
                    {
                        if (Game1.player.Items[i] is Tool existingTool && 
                            existingTool.GetType() == tool.GetType())
                        {
                            Game1.player.Items[i] = item;
                            ShowLocalSuccess(item);
                            Monitor.Log(Helper.Translation.Get("log.gave_item", new { quantity, item = item.DisplayName, quality }), LogLevel.Info);
                            return;
                        }
                    }

                    ShowLocalError(Helper.Translation.Get("error.tool_not_found", new { tool = tool.Name }));
                    return;
                }
            }

            Item? leftover = Game1.player.addItemToInventory(item);

            if (leftover != null)
                ShowLocalError(Helper.Translation.Get("error.inventory_full", new { item = leftover.DisplayName }));
            else
                ShowLocalSuccess(item);

            Monitor.Log(Helper.Translation.Get("log.gave_item", new { quantity, item = item.DisplayName, quality }), LogLevel.Info);
        }

        private static void ShowLocalSuccess(Item item)
        {
            Game1.hudMessages.RemoveAll(m => m.messageSubject?.QualifiedItemId == item.QualifiedItemId);
            Game1.addHUDMessage(new HUDMessage(item.DisplayName, item.Stack) { messageSubject = item, timeLeft = 3500f });
            Game1.playSound("coin");
        }

        private static void ShowLocalError(string message)
        {
            Game1.addHUDMessage(new HUDMessage(message, HUDMessage.error_type) { timeLeft = 3500f });
            Game1.playSound("cancel");
        }
    }

    public interface IGMCMApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
        void AddKeybind(IManifest mod, Func<SButton> getValue, Action<SButton> setValue, Func<string> name, Func<string>? tooltip = null, string? fieldId = null);
        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string>? tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string>? formatValue = null, string? fieldId = null);
    }
}