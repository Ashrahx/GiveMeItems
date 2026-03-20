using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
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
            helper.Events.Input.ButtonPressed  += OnButtonPressed;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            Monitor.Log($"GiveMeItems listo. Presiona [{Config.ActivateKey}] para obtener ítems.", LogLevel.Info);
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            // No abrir si ya hay un menú activo (evita que la G se consuma al escribir)
            if (Game1.activeClickableMenu != null) return;

            if (e.Button == Config.ActivateKey)
                Game1.activeClickableMenu = new ItemInputMenu(GiveItem);
        }

        // ── Dar ítem SOLO al jugador local ────────────────────────────────
        //
        // Nunca usamos Game1.player.addItemByMenuIfNecessary porque en
        // multijugador puede sincronizarse con el servidor.
        // En cambio añadimos directamente al inventario local y mostramos
        // el menú de items-collected solo para este cliente.

        private void GiveItem(string input, int quantity)
        {
            if (string.IsNullOrWhiteSpace(input)) return;

            Item? item = null;

            if (int.TryParse(input.Trim(), out int itemId))
            {
                try { item = ItemRegistry.Create(itemId.ToString(), quantity); }
                catch { item = null; }

                if (item == null)
                {
                    ShowLocalError($"No existe ningún objeto con ID {itemId}.");
                    return;
                }
            }
            else
            {
                item = ItemRegistry.ItemTypes
                    .SelectMany(t => t.GetAllIds())
                    .Select(id => ItemRegistry.Create(id))
                    .FirstOrDefault(i => i != null &&
                        i.DisplayName.Contains(input.Trim(), StringComparison.OrdinalIgnoreCase));

                if (item == null)
                {
                    ShowLocalError($"No encontré ningún objeto llamado \"{input.Trim()}\".");
                    return;
                }

                item.Stack = quantity;
            }

            // Añadir al inventario directamente sin pasar por el sistema de notificaciones
            // del juego (que duplicaría el HUD). Insertamos slot por slot en silencio.
            Item? leftover = Game1.player.addItemToInventory(item);

            if (leftover != null)
            {
                // Inventario lleno: notificar sin tirar al suelo
                // (createItemDebris sincroniza con el servidor en multijugador)
                ShowLocalError($"Inventario lleno. No caben más {leftover.DisplayName}.");
            }

            // Mostrar UNA sola notificación local nuestra
            ShowLocalSuccess(item);

            Monitor.Log($"[GiveMeItems] +{quantity}x {item.DisplayName} (solo local)", LogLevel.Info);
        }

        // ── Notificaciones puramente locales ──────────────────────────────

        /// <summary>
        /// Muestra el cuadro de "objeto recibido" estilo Stardew
        /// solo en la pantalla del jugador local. No va a la red.
        /// </summary>
        private static void ShowLocalSuccess(Item item)
        {
            // Eliminar cualquier HUD que haya generado addItemToInventory internamente
            // para evitar duplicados. Buscamos por messageSubject o texto igual.
            Game1.hudMessages.RemoveAll(m =>
                m.messageSubject != null && m.messageSubject.QualifiedItemId == item.QualifiedItemId);

            // Mostrar nuestro único HUD con ícono del ítem
            var msg = new HUDMessage(item.DisplayName, item.Stack)
            {
                messageSubject = item,
                timeLeft       = 3500f,
            };
            Game1.addHUDMessage(msg);
            Game1.playSound("coin");
        }

        /// <summary>
        /// Mensaje de error solo local, estilo HUD de Stardew.
        /// </summary>
        private static void ShowLocalError(string message)
        {
            Game1.addHUDMessage(new HUDMessage(message, HUDMessage.error_type) { timeLeft = 3500f });
            Game1.playSound("cancel");
        }
    }
}