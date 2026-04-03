using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace GiveMeItems
{
    public class ItemInputMenu : IClickableMenu
    {
        private const int MW = 720;
        private const int MH = 540;
        private const int PAD = 32;

        // Grid Izquierdo
        private const int GCOLS = 3;
        private const int GROWS = 3;
        private const int CELL  = 112; 
        private const int ICON  = 64;

        // Paneles Derechos
        private const int RPX = PAD + (GCOLS * CELL) + 32;
        private const int RPW = MW - RPX - PAD;

        // ── Calidades ─────────────────────────────────────────────────────────
        private const int Q_NORMAL  = 0;
        private const int Q_SILVER  = 1;
        private const int Q_GOLD    = 2;
        private const int Q_IRIDIUM = 4;

        private string SearchText      = "";
        private string QuantityText    = "1";
        private string HoverText       = "";
        private bool   EditingQuantity = false;
        private bool   MenuActive      = true;
        private int    SelectedQuality = Q_NORMAL;

        private List<Item> Results     = new();
        private int        SelectedIdx = 0;
        private int        SearchTimer = 0;

        private readonly Action<string, int, int> OnConfirm;
        private readonly ITranslationHelper T;

        private readonly ClickableTextureComponent BtnOk;
        private readonly ClickableTextureComponent BtnCancel;
        private readonly ClickableTextureComponent BtnMinus;
        private readonly ClickableTextureComponent BtnPlus;

        private readonly Rectangle[] StarRects = new Rectangle[4];
        private Rectangle SearchBoxRect;
        private Rectangle QuantityBoxRect;

        public ItemInputMenu(Action<string, int, int> onConfirm, int defaultQuantity, ITranslationHelper translation)
            : base(
                (Game1.viewport.Width  - MW) / 2,
                (Game1.viewport.Height - MH) / 2,
                MW, MH, showUpperRightCloseButton: false)
        {
            OnConfirm    = onConfirm;
            T            = translation;
            QuantityText = Math.Clamp(defaultQuantity, 1, 999).ToString();

            Game1.keyboardDispatcher.Subscriber = new TextBoxKeyboardSubscriber(this);

            int btnY = yPositionOnScreen + MH - PAD - 64;
            int okX  = xPositionOnScreen + MW - PAD - 64;

            BtnOk = new ClickableTextureComponent(
                new Rectangle(okX, btnY, 64, 64),
                Game1.mouseCursors,
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);

            BtnCancel = new ClickableTextureComponent(
                new Rectangle(okX - 72, btnY, 64, 64),
                Game1.mouseCursors,
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f);

            // Botones - / +
            BtnMinus = new ClickableTextureComponent(
                new Rectangle(0, 0, 48, 48), Game1.mouseCursors,
                new Rectangle(177, 345, 7, 8), 5f);

            BtnPlus = new ClickableTextureComponent(
                new Rectangle(0, 0, 48, 48), Game1.mouseCursors,
                new Rectangle(184, 345, 7, 8), 5f);
        }

        public override void receiveKeyPress(Keys key)
        {
            if (!MenuActive) return;
            switch (key)
            {
                case Keys.Enter:  Confirm(); break;
                case Keys.Escape: Cancel();  break;
                case Keys.Tab:
                    if (EditingQuantity && string.IsNullOrWhiteSpace(QuantityText))
                        QuantityText = "1";
                    EditingQuantity = !EditingQuantity;
                    Game1.playSound("smallSelect");
                    break;
                case Keys.Back:
                    if (EditingQuantity)
                    {
                        if (QuantityText.Length > 0)
                            QuantityText = QuantityText[..^1];
                    }
                    else if (SearchText.Length > 0)
                    {
                        SearchText = SearchText[..^1];
                        ResetSearch();
                    }
                    break;
                case Keys.Left:  MoveSelection(-1); break;
                case Keys.Right: MoveSelection(+1); break;
                case Keys.Up:    MoveSelection(-GCOLS); break;
                case Keys.Down:  MoveSelection(+GCOLS); break;
            }
        }

        public void ReceiveCharacter(char c)
        {
            if (char.IsControl(c) || !MenuActive) return;

            if (EditingQuantity)
            {
                if (!char.IsDigit(c) || QuantityText.Length >= 3)
                    return;

                string next = (QuantityText == "0" ? "" : QuantityText) + c;
                if (int.TryParse(next, out int qty))
                    QuantityText = Math.Clamp(qty, 0, 999).ToString();
                return;
            }

            if (SearchText.Length < 40)
            {
                SearchText += c;
                ResetSearch();
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (BtnOk.containsPoint(x, y))     { Game1.playSound("coin");        Confirm(); return; }
            if (BtnCancel.containsPoint(x, y)) { Game1.playSound("bigDeSelect"); Cancel();  return; }
            if (BtnMinus.containsPoint(x, y))  { ChangeQty(-1); return; }
            if (BtnPlus.containsPoint(x, y))   { ChangeQty(+1); return; }

            if (SearchBoxRect.Contains(x, y))
            {
                if (EditingQuantity && string.IsNullOrWhiteSpace(QuantityText))
                    QuantityText = "1";
                EditingQuantity = false;
                Game1.playSound("smallSelect");
                return;
            }

            if (QuantityBoxRect.Contains(x, y))
            {
                if (!EditingQuantity)
                    QuantityText = "";
                EditingQuantity = true;
                Game1.playSound("smallSelect");
                return;
            }

            if (EditingQuantity && string.IsNullOrWhiteSpace(QuantityText))
                QuantityText = "1";
            EditingQuantity = false;

            int[] qVals = { Q_NORMAL, Q_SILVER, Q_GOLD, Q_IRIDIUM };
            for (int qi = 0; qi < StarRects.Length; qi++)
            {
                if (StarRects[qi].Contains(x, y))
                {
                    SetQuality(qVals[qi]);
                    return;
                }
            }

            for (int i = 0; i < Results.Count && i < GCOLS * GROWS; i++)
            {
                if (GetCellRect(i).Contains(x, y))
                {
                    SelectedIdx = i;
                    Game1.playSound("smallSelect");
                    return;
                }
            }
        }

        public override void receiveScrollWheelAction(int dir) => ChangeQty(dir > 0 ? 1 : -1);

        public override void performHoverAction(int x, int y)
        {
            HoverText = "";

            BtnOk.tryHover(x, y, 0.25f);
            BtnCancel.tryHover(x, y, 0.25f);
            BtnMinus.tryHover(x, y, 0.2f);
            BtnPlus.tryHover(x, y, 0.2f);

            if (BtnOk.containsPoint(x, y)) { HoverText = T.Get("menu.tooltip.confirm"); return; }
            if (BtnCancel.containsPoint(x, y)) { HoverText = T.Get("menu.tooltip.cancel"); return; }
            if (BtnMinus.containsPoint(x, y)) { HoverText = T.Get("menu.tooltip.minus"); return; }
            if (BtnPlus.containsPoint(x, y)) { HoverText = T.Get("menu.tooltip.plus"); return; }

            string[] qualityKeys = { "menu.quality.normal", "menu.quality.silver", "menu.quality.gold", "menu.quality.iridium" };
            for (int qi = 0; qi < StarRects.Length; qi++)
            {
                if (StarRects[qi].Contains(x, y))
                {
                    HoverText = T.Get(qualityKeys[qi]);
                    return;
                }
            }

            for (int i = 0; i < Results.Count && i < GCOLS * GROWS; i++)
            {
                if (GetCellRect(i).Contains(x, y))
                {
                    HoverText = Results[i].DisplayName;
                    return;
                }
            }
        }

        private Rectangle GetCellRect(int i)
        {
            int col = i % GCOLS;
            int row = i / GCOLS;
            return new Rectangle(
                xPositionOnScreen + PAD + (col * CELL),
                yPositionOnScreen + 152 + (row * CELL),
                CELL, CELL);
        }

        private void MoveSelection(int delta)
        {
            if (Results.Count == 0) return;
            SelectedIdx = Math.Clamp(SelectedIdx + delta, 0, Math.Min(Results.Count, GCOLS * GROWS) - 1);
            Game1.playSound("smallSelect");
        }

        private int GetQty() => int.TryParse(QuantityText, out int v) ? Math.Clamp(v, 1, 999) : 1;
        private void ChangeQty(int delta) { QuantityText = Math.Clamp(GetQty() + delta, 1, 999).ToString(); Game1.playSound("smallSelect"); }
        private void SetQuality(int q) { SelectedQuality = q; Game1.playSound("smallSelect"); }
        private void ResetSearch() { SearchTimer = 0; SelectedIdx = 0; }
        private Item? GetSelectedItem() => Results.Count > 0 && SelectedIdx < Results.Count ? Results[SelectedIdx] : null;

        private static string FitText(string text, SpriteFont font, float maxWidth)
        {
            if (string.IsNullOrEmpty(text) || font.MeasureString(text).X <= maxWidth)
                return text;

            string trimmed = text;
            while (trimmed.Length > 1 && font.MeasureString(trimmed + "...").X > maxWidth)
                trimmed = trimmed[..^1];

            return trimmed + "...";
        }

        private static void DrawWrappedText(SpriteBatch b, string text, Vector2 position, float width, Color color, float scale = 1f)
        {
            string wrapped = Game1.parseText(text, Game1.smallFont, (int)(width / scale));
            Utility.drawTextWithShadow(b, wrapped, Game1.smallFont, position, color, scale);
        }

        private void DrawPanel(SpriteBatch b, Rectangle bounds, Color tint, bool shadow = false)
        {
            drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
                bounds.X, bounds.Y, bounds.Width, bounds.Height, tint, 1f, shadow);
        }

        private void RunSearch()
        {
            Results.Clear();
            if (string.IsNullOrWhiteSpace(SearchText)) return;

            string query = SearchText.Trim();
            var exactMatches = new List<Item>();
            var partialMatches = new List<Item>();
            var seenIds = new HashSet<string>();

            try
            {
                if (int.TryParse(query, out int id))
                {
                    var item = ItemRegistry.Create(id.ToString(), 1);
                    if (item != null) Results.Add(item);
                    return;
                }

                foreach (var type in ItemRegistry.ItemTypes)
                {
                    foreach (var rid in type.GetAllIds())
                    {
                        var c = ItemRegistry.Create(rid, 1);
                        if (c == null) continue;

                        if (!seenIds.Add(c.QualifiedItemId)) continue;

                        if (c.DisplayName.Equals(query, StringComparison.OrdinalIgnoreCase))
                            exactMatches.Add(c);
                        else if (c.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase))
                            partialMatches.Add(c);
                    }
                }

                Results.AddRange(exactMatches);
                Results.AddRange(partialMatches);
            }
            catch { }
        }

        private void Confirm()
        {
            if (!MenuActive) return;
            Item? sel = (Results.Count > 0 && SelectedIdx < Results.Count) ? Results[SelectedIdx] : null;
            string input = sel?.QualifiedItemId ?? SearchText;
            if (string.IsNullOrWhiteSpace(input)) return;

            MenuActive = false;
            Game1.keyboardDispatcher.Subscriber = null;
            exitThisMenu();
            OnConfirm?.Invoke(input, GetQty(), SelectedQuality);
        }

        private void Cancel()
        {
            MenuActive = false;
            Game1.keyboardDispatcher.Subscriber = null;
            exitThisMenu();
        }

        public override void update(GameTime time)
        {
            base.update(time);
            if (SearchTimer < 20) { SearchTimer++; if (SearchTimer == 20) RunSearch(); }
        }

        public override void draw(SpriteBatch b)
        {
            int mouseX = Game1.getMouseX();
            int mouseY = Game1.getMouseY();
            Item? selectedItem = GetSelectedItem();

            b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), Color.Black * 0.72f);

            drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
                xPositionOnScreen, yPositionOnScreen, MW, MH, Color.White, drawShadow: true);

            SpriteText.drawStringWithScrollCenteredAt(b, T.Get("menu.title"),
                xPositionOnScreen + MW / 2, yPositionOnScreen - 4);

            int leftX = xPositionOnScreen + PAD;
            int leftW = GCOLS * CELL;
            int searchLabelY = yPositionOnScreen + 72;

            Utility.drawTextWithShadow(b, T.Get("menu.field.label.short"), Game1.dialogueFont,
                new Vector2(leftX + 2, searchLabelY), new Color(82, 56, 32), 0.55f);

            SearchBoxRect = new Rectangle(leftX, searchLabelY + 28, leftW, 48);
            DrawPanel(b, SearchBoxRect, new Color(253, 248, 231));

            string disp = SearchText;
            while (Game1.smallFont.MeasureString(disp + "|").X > SearchBoxRect.Width - 24 && disp.Length > 0)
                disp = disp[1..];

            string cursor = (MenuActive && !EditingQuantity && DateTime.Now.Millisecond < 500) ? "|" : " ";
            string searchDisplay = SearchText.Length > 0 ? disp + cursor : T.Get("menu.field.placeholder") + cursor;
            Color searchColor = SearchText.Length > 0 ? Game1.textColor : Color.Gray;
            Utility.drawTextWithShadow(b, searchDisplay, Game1.smallFont,
                new Vector2(SearchBoxRect.X + 12, SearchBoxRect.Y + 12), searchColor);

            string resultsText = Results.Count > 0
                ? T.Get("menu.results.count", new { count = Results.Count })
                : T.Get("menu.results.none");
            Utility.drawTextWithShadow(b, resultsText, Game1.smallFont,
                new Vector2(leftX + 2, SearchBoxRect.Bottom + 6), new Color(90, 76, 54), 0.75f);

            Rectangle gridPanel = new(leftX - 8, yPositionOnScreen + 144, leftW + 16, (GROWS * CELL) + 16);
            DrawPanel(b, gridPanel, new Color(242, 235, 214));

            for (int i = 0; i < GCOLS * GROWS; i++)
            {
                Rectangle cell = GetCellRect(i);
                bool hasItem = i < Results.Count;
                bool isSel = hasItem && i == SelectedIdx;
                bool hovered = cell.Contains(mouseX, mouseY);

                Color cellTint = isSel
                    ? new Color(214, 188, 255)
                    : hovered ? new Color(248, 242, 226) : new Color(255, 252, 244);

                DrawPanel(b, cell, cellTint);

                if (!hasItem) continue;

                Item item = Results[i];
                item.drawInMenu(b, new Vector2(cell.X + (CELL - ICON) / 2f, cell.Y + 22), 1f, 1f, 0.9f, StackDrawType.Hide, Color.White, false);
            }

            if (Results.Count == 0 && SearchTimer >= 20 && !string.IsNullOrWhiteSpace(SearchText))
            {
                string notFound = T.Get("menu.preview.not_found");
                Vector2 msgSize = Game1.smallFont.MeasureString(notFound) * 0.75f;
                Utility.drawTextWithShadow(b, notFound, Game1.smallFont,
                    new Vector2(gridPanel.X + (gridPanel.Width - msgSize.X) / 2f, gridPanel.Center.Y - msgSize.Y / 2f), Color.DimGray, 0.75f);
            }

            int rightX = xPositionOnScreen + RPX;
            int rightY = yPositionOnScreen + 96;

            Rectangle previewBounds = new(rightX, rightY, RPW, 110);
            
            Utility.drawTextWithShadow(b, T.Get("menu.preview.title"), Game1.dialogueFont,
                new Vector2(previewBounds.X, previewBounds.Y - 26), new Color(82, 56, 32), 0.55f);

            DrawPanel(b, previewBounds, new Color(243, 236, 216), false); 

            if (selectedItem != null)
            {
                selectedItem.drawInMenu(b, new Vector2(previewBounds.X + 16, previewBounds.Y + 24), 1f, 1f, 0.9f, StackDrawType.Hide, Color.White, false);

                string itemName = FitText(selectedItem.DisplayName, Game1.smallFont, previewBounds.Width - 90);
                Utility.drawTextWithShadow(b, itemName, Game1.smallFont,
                    new Vector2(previewBounds.X + 88, previewBounds.Y + 30), Game1.textColor, 0.9f);

                string idLine = T.Get("menu.preview.id", new { id = selectedItem.QualifiedItemId });
                Utility.drawTextWithShadow(b, idLine, Game1.smallFont,
                    new Vector2(previewBounds.X + 88, previewBounds.Y + 60), Color.DimGray, 0.65f);
            }
            else
            {
                DrawWrappedText(b, T.Get("menu.preview.empty"), new Vector2(previewBounds.X + 14, previewBounds.Y + 40), previewBounds.Width - 28, Color.Gray, 0.68f);
            }

            Rectangle qtyBounds = new(rightX, previewBounds.Bottom + 18, RPW, 84);
            DrawPanel(b, qtyBounds, new Color(243, 236, 216));

            string qtyLbl = T.Get("menu.quantity.label");
            Utility.drawTextWithShadow(b, qtyLbl, Game1.smallFont,
                new Vector2(qtyBounds.X + (qtyBounds.Width - Game1.smallFont.MeasureString(qtyLbl).X) / 2f, qtyBounds.Y + 8), new Color(90, 76, 54), 0.85f);

            int btnSz = 40, numW = 96, numH = 44;
            int rowW = btnSz + 10 + numW + 10 + btnSz;
            int rowX = qtyBounds.X + (qtyBounds.Width - rowW) / 2;
            int rowY = qtyBounds.Y + 30;

            BtnMinus.bounds = new Rectangle(rowX, rowY, btnSz, btnSz);
            DrawBrownButton(b, BtnMinus.bounds, "-", BtnMinus.containsPoint(mouseX, mouseY));

            int numX = rowX + btnSz + 10;
            QuantityBoxRect = new Rectangle(numX, rowY, numW, numH);
            DrawPanel(b, QuantityBoxRect, new Color(255, 250, 238));

            string qtyCursor = (EditingQuantity && DateTime.Now.Millisecond < 500) ? "|" : "";
            string qtyDisplay = string.IsNullOrEmpty(QuantityText)
                ? (EditingQuantity ? qtyCursor : "1")
                : QuantityText + qtyCursor;
            Vector2 qtySize = Game1.smallFont.MeasureString(qtyDisplay);
            Utility.drawTextWithShadow(b, qtyDisplay, Game1.smallFont,
                new Vector2(QuantityBoxRect.X + (QuantityBoxRect.Width - qtySize.X) / 2f, QuantityBoxRect.Y + (QuantityBoxRect.Height - qtySize.Y) / 2f - 1f), Game1.textColor);

            BtnPlus.bounds = new Rectangle(numX + numW + 10, rowY, btnSz, btnSz);
            DrawBrownButton(b, BtnPlus.bounds, "+", BtnPlus.containsPoint(mouseX, mouseY));

            Rectangle qualityBounds = new(rightX, qtyBounds.Bottom + 18, RPW, 84);
            DrawPanel(b, qualityBounds, new Color(243, 236, 216));

            string rarLbl = T.Get("menu.quality.label");
            Utility.drawTextWithShadow(b, rarLbl, Game1.smallFont,
                new Vector2(qualityBounds.X + (qualityBounds.Width - Game1.smallFont.MeasureString(rarLbl).X) / 2f, qualityBounds.Y + 8), new Color(90, 76, 54), 0.85f);

            // FIX: Array de calidades actualizado usando los spritesheet rects correctos de cada estrella nativa
            var qualities = new (int val, string key, Rectangle src, Color color)[] {
                (Q_NORMAL, "menu.quality.normal", new Rectangle(338, 400, 8, 8), new Color(100, 100, 100)),
                (Q_SILVER, "menu.quality.silver", new Rectangle(338, 400, 8, 8), Color.White),
                (Q_GOLD, "menu.quality.gold", new Rectangle(346, 400, 8, 8), Color.White),
                (Q_IRIDIUM, "menu.quality.iridium", new Rectangle(346, 392, 8, 8), Color.White)
            };

            int starSize = 44;
            int starGap = 12;
            int starsWidth = (qualities.Length * starSize) + ((qualities.Length - 1) * starGap);
            int starX = qualityBounds.X + (qualityBounds.Width - starsWidth) / 2;
            int starY = qualityBounds.Y + 30;

            for (int qi = 0; qi < qualities.Length; qi++)
            {
                var q = qualities[qi];
                Rectangle starRect = new(starX + qi * (starSize + starGap), starY, starSize, starSize);
                StarRects[qi] = starRect;

                bool sel = SelectedQuality == q.val;
                bool hovered = starRect.Contains(mouseX, mouseY);
                float scale = sel ? 5f : hovered ? 4.6f : 4.2f;
                Color starColor = sel ? q.color : q.color * (hovered ? 0.9f : 0.7f);
                Vector2 pos = new(
                    starRect.X + (starRect.Width - q.src.Width * scale) / 2f,
                    starRect.Y + (starRect.Height - q.src.Height * scale) / 2f);

                b.Draw(Game1.mouseCursors, pos, q.src, starColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.9f);
            }

            BtnOk.draw(b);
            BtnCancel.draw(b);

            if (!string.IsNullOrWhiteSpace(HoverText))
                drawHoverText(b, HoverText, Game1.smallFont);

            drawMouse(b);
        }

        private void DrawBrownButton(SpriteBatch b, Rectangle bounds, string text, bool hovered = false)
        {
            Color fill = hovered ? new Color(171, 120, 74) : new Color(137, 92, 52);
            DrawPanel(b, bounds, fill);

            Vector2 size = Game1.dialogueFont.MeasureString(text);
            
            float xOffset = text == "+" ? 1f : 0f;
            float yOffset = text == "+" ? 1f : -2f; 
            
            Utility.drawTextWithShadow(b, text, Game1.dialogueFont,
                new Vector2(bounds.X + (bounds.Width - size.X) / 2f + xOffset, bounds.Y + (bounds.Height - size.Y) / 2f + yOffset), new Color(255, 244, 210));
        }

        private void DrawItemName(SpriteBatch b, string name, Rectangle cell, bool isSel)
        {
            const float scale = 0.72f;
            string displayName = FitText(name, Game1.smallFont, (cell.Width - 12) / scale);
            Vector2 size = Game1.smallFont.MeasureString(displayName) * scale;
            float yPos = cell.Bottom - size.Y - 8;
            Color textColor = isSel ? new Color(56, 94, 163) : Game1.textColor;

            Utility.drawTextWithShadow(b, displayName, Game1.smallFont,
                new Vector2(cell.X + (cell.Width - size.X) / 2f, yPos), textColor, scale);
        }
    }

    public class TextBoxKeyboardSubscriber : IKeyboardSubscriber
    {
        private readonly ItemInputMenu Menu;
        public TextBoxKeyboardSubscriber(ItemInputMenu menu) => Menu = menu;
        public bool Selected { get; set; } = true;
        public void RecieveTextInput(char c) => Menu.ReceiveCharacter(c);
        public void RecieveTextInput(string s) { foreach (var c in s) Menu.ReceiveCharacter(c); }
        public void RecieveCommandInput(char c) { }
        public void RecieveSpecialInput(Keys k) { }
    }
}