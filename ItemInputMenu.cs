using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;

namespace GiveMeItems
{
    public class ItemInputMenu : IClickableMenu
    {
        private const int MenuWidth  = 560;
        private const int MenuHeight = 320;
        private const int Padding    = 24;

        private string   InputText   = "";
        private new bool IsActive    = true;
        private int      Quantity    = 1;

        private Item?   PreviewItem  = null;
        private string  PreviewLabel = "";
        private int     PreviewTimer = 0;

        private readonly Action<string, int> OnConfirm;

        private readonly ClickableTextureComponent OkButton;
        private readonly ClickableTextureComponent CancelButton;
        private readonly ClickableTextureComponent MinusButton;
        private readonly ClickableTextureComponent PlusButton;

        public ItemInputMenu(Action<string, int> onConfirm)
            : base(
                (Game1.viewport.Width  - MenuWidth)  / 2,
                (Game1.viewport.Height - MenuHeight) / 2,
                MenuWidth, MenuHeight,
                showUpperRightCloseButton: false)
        {
            OnConfirm = onConfirm;
            Game1.keyboardDispatcher.Subscriber = new TextBoxKeyboardSubscriber(this);

            OkButton = new ClickableTextureComponent(
                new Rectangle(
                    xPositionOnScreen + MenuWidth - Padding - 64 - 4,
                    yPositionOnScreen + MenuHeight - Padding - 64,
                    64, 64),
                Game1.mouseCursors,
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);

            CancelButton = new ClickableTextureComponent(
                new Rectangle(
                    xPositionOnScreen + MenuWidth - Padding - 64 - 4 - 68,
                    yPositionOnScreen + MenuHeight - Padding - 64,
                    64, 64),
                Game1.mouseCursors,
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 47), 1f);

            MinusButton = new ClickableTextureComponent(
                new Rectangle(0, 0, 28, 32), Game1.mouseCursors,
                new Rectangle(177, 345, 7, 8), 4f);

            PlusButton = new ClickableTextureComponent(
                new Rectangle(0, 0, 28, 32), Game1.mouseCursors,
                new Rectangle(184, 345, 7, 8), 4f);
        }

        public override void receiveKeyPress(Keys key)
        {
            if (!IsActive) return;
            switch (key)
            {
                case Keys.Enter:  Confirm(); break;
                case Keys.Escape: Cancel();  break;
                case Keys.Left:   ChangeQuantity(-1);  break;
                case Keys.Right:  ChangeQuantity(+1);  break;
                case Keys.Up:     ChangeQuantity(+10); break;
                case Keys.Down:   ChangeQuantity(-10); break;
                case Keys.Back:
                    if (InputText.Length > 0) { InputText = InputText[..^1]; ResetPreviewTimer(); }
                    break;
            }
        }

        public void ReceiveCharacter(char c)
        {
            if (!char.IsControl(c) && InputText.Length < 40) { InputText += c; ResetPreviewTimer(); }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (OkButton.containsPoint(x, y))     { Game1.playSound("coin");        Confirm(); return; }
            if (CancelButton.containsPoint(x, y)) { Game1.playSound("bigDeSelect"); Cancel();  return; }
            if (MinusButton.containsPoint(x, y))  { ChangeQuantity(-1); return; }
            if (PlusButton.containsPoint(x, y))   { ChangeQuantity(+1); return; }
        }

        public override void receiveScrollWheelAction(int direction)
            => ChangeQuantity(direction > 0 ? 1 : -1);

        public override void performHoverAction(int x, int y)
        {
            OkButton.tryHover(x, y, 0.25f);
            CancelButton.tryHover(x, y, 0.25f);
            MinusButton.tryHover(x, y, 0.2f);
            PlusButton.tryHover(x, y, 0.2f);
        }

        private void ChangeQuantity(int delta)
        {
            Quantity = Math.Clamp(Quantity + delta, 1, 999);
            Game1.playSound("smallSelect");
        }

        private void ResetPreviewTimer() => PreviewTimer = 0;

        private void UpdatePreview()
        {
            if (string.IsNullOrWhiteSpace(InputText)) { PreviewItem = null; PreviewLabel = ""; return; }
            try
            {
                if (int.TryParse(InputText.Trim(), out int id))
                {
                    var item = ItemRegistry.Create(id.ToString(), 1);
                    PreviewItem  = item;
                    PreviewLabel = item != null ? item.DisplayName : "ID no encontrado";
                    return;
                }
                string query = InputText.Trim();
                Item? exactMatch   = null;
                Item? partialMatch = null;

                foreach (var type in ItemRegistry.ItemTypes)
                    foreach (var rid in type.GetAllIds())
                    {
                        var c = ItemRegistry.Create(rid, 1);
                        if (c == null) continue;
                        if (c.DisplayName.Equals(query, StringComparison.OrdinalIgnoreCase))
                        { exactMatch = c; break; }
                        if (partialMatch == null && c.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase))
                            partialMatch = c;
                    }

                var found = exactMatch ?? partialMatch;
                if (found != null) { PreviewItem = found; PreviewLabel = found.DisplayName; return; }
                PreviewItem = null; PreviewLabel = "No encontrado";
            }
            catch { PreviewItem = null; PreviewLabel = ""; }
        }

        private void Confirm()
        {
            if (string.IsNullOrWhiteSpace(InputText)) return;
            IsActive = false;
            Game1.keyboardDispatcher.Subscriber = null;
            exitThisMenu();
            OnConfirm?.Invoke(InputText, Quantity);
        }

        private void Cancel()
        {
            IsActive = false;
            Game1.keyboardDispatcher.Subscriber = null;
            exitThisMenu();
        }

        public override void update(GameTime time)
        {
            base.update(time);
            if (PreviewTimer < 30) { PreviewTimer++; if (PreviewTimer == 30) UpdatePreview(); }
        }

        public override void draw(SpriteBatch b)
        {
            b.Draw(Game1.fadeToBlackRect,
                new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height),
                Color.Black * 0.55f);

            drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
                xPositionOnScreen, yPositionOnScreen, width, height,
                Color.White, drawShadow: true);

            SpriteText.drawStringWithScrollCenteredAt(b,
                "Dame un objeto",
                xPositionOnScreen + width / 2,
                yPositionOnScreen - 4,
                color: SpriteText.color_Black);

            Utility.drawTextWithShadow(b,
                "Escribe el ID o el nombre del objeto:",
                Game1.smallFont,
                new Vector2(xPositionOnScreen + Padding, yPositionOnScreen + 72),
                Game1.textColor);

            int boxX = xPositionOnScreen + Padding;
            int boxY = yPositionOnScreen + 100;
            int boxW = width - Padding * 2;
            int boxH = 52;

            drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
                boxX, boxY, boxW, boxH,
                Color.Wheat * 0.55f, scale: 1f, drawShadow: false);

            bool   hasInput    = InputText.Length > 0;
            string cursor      = IsActive && (DateTime.Now.Millisecond < 500) ? "|" : " ";
            string displayText = hasInput ? InputText + cursor : "ej: 72  ó  Diamante" + cursor;

            Utility.drawTextWithShadow(b, displayText, Game1.smallFont,
                new Vector2(boxX + 12, boxY + 14),
                hasInput ? Game1.textColor : Color.Gray);

            int previewY = boxY + boxH + 14;

            if (PreviewTimer >= 30 && hasInput)
            {
                if (PreviewItem != null)
                {
                    PreviewItem.drawInMenu(b,
                        new Vector2(xPositionOnScreen + Padding, previewY - 4),
                        0.85f, 1f, 0.9f, StackDrawType.Hide, Color.White, true);

                    Utility.drawTextWithShadow(b, PreviewLabel, Game1.smallFont,
                        new Vector2(xPositionOnScreen + Padding + 56, previewY + 8),
                        Game1.textColor);

                    b.Draw(Game1.mouseCursors,
                        new Vector2(
                            xPositionOnScreen + Padding + 56 + Game1.smallFont.MeasureString(PreviewLabel).X + 10,
                            previewY + 6),
                        new Rectangle(194, 388, 16, 16),
                        Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.9f);
                }
                else
                {
                    b.Draw(Game1.mouseCursors,
                        new Vector2(xPositionOnScreen + Padding, previewY + 4),
                        new Rectangle(322, 498, 12, 12),
                        Color.White, 0f, Vector2.Zero, 2.5f, SpriteEffects.None, 0.9f);

                    Utility.drawTextWithShadow(b, PreviewLabel, Game1.smallFont,
                        new Vector2(xPositionOnScreen + Padding + 36, previewY + 8),
                        Color.Red * 0.9f);
                }
            }
            else if (!hasInput)
            {
                Utility.drawTextWithShadow(b,
                    "Enter para confirmar  •  Esc para cancelar",
                    Game1.smallFont,
                    new Vector2(xPositionOnScreen + Padding, previewY + 8),
                    Game1.textColor * 0.45f);
            }

            int qtyRowY  = yPositionOnScreen + MenuHeight - Padding - 50;
            int qtyRowCY = qtyRowY + 22;

            string qtyLabel     = "Cantidad:";
            Vector2 qtyLabelSz  = Game1.smallFont.MeasureString(qtyLabel);
            int     qtyLabelX   = xPositionOnScreen + Padding;
            int     qtyLabelY   = qtyRowCY - (int)(qtyLabelSz.Y / 2);

            Utility.drawTextWithShadow(b, qtyLabel, Game1.smallFont,
                new Vector2(qtyLabelX, qtyLabelY), Game1.textColor);

            int btnW    = 28;
            int btnH    = 32;
            int numBoxW = 64;
            int numBoxH = 44;
            int minusBtnX = qtyLabelX + (int)qtyLabelSz.X + 16;
            int numBoxX   = minusBtnX + btnW + 6;
            int numBoxY   = qtyRowCY - numBoxH / 2;

            drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60),
                numBoxX, numBoxY, numBoxW, numBoxH,
                Color.Wheat * 0.55f, scale: 1f, drawShadow: false);

            string  qtyStr  = Quantity.ToString();
            Vector2 qtySz   = Game1.smallFont.MeasureString(qtyStr);
            Utility.drawTextWithShadow(b, qtyStr, Game1.smallFont,
                new Vector2(numBoxX + (numBoxW - qtySz.X) / 2f, numBoxY + (numBoxH - qtySz.Y) / 2f),
                Game1.textColor);

            MinusButton.bounds = new Rectangle(
                minusBtnX,
                qtyRowCY - btnH / 2,
                btnW, btnH);
            MinusButton.draw(b);

            PlusButton.bounds = new Rectangle(
                numBoxX + numBoxW + 6,
                qtyRowCY - btnH / 2,
                btnW, btnH);
            PlusButton.draw(b);

            OkButton.draw(b);
            CancelButton.draw(b);

            int mx = Game1.getMouseX(), my = Game1.getMouseY();
            if      (OkButton.containsPoint(mx, my))     drawToolTip(b, "Confirmar (Enter)", "", null);
            else if (CancelButton.containsPoint(mx, my)) drawToolTip(b, "Cancelar (Esc)", "", null);
            else if (MinusButton.containsPoint(mx, my))  drawToolTip(b, "Menos (-1)", "", null);
            else if (PlusButton.containsPoint(mx, my))   drawToolTip(b, "Más (+1)", "", null);

            drawMouse(b);
        }
    }

    public class TextBoxKeyboardSubscriber : IKeyboardSubscriber
    {
        private readonly ItemInputMenu Menu;
        public TextBoxKeyboardSubscriber(ItemInputMenu menu) => Menu = menu;
        public bool Selected { get; set; } = true;
        public void RecieveTextInput(char inputChar) => Menu.ReceiveCharacter(inputChar);
        public void RecieveTextInput(string text)    { foreach (var c in text) Menu.ReceiveCharacter(c); }
        public void RecieveCommandInput(char command) { }
        public void RecieveSpecialInput(Keys key)     { }
    }
}