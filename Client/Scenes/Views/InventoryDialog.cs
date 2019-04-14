using System.Drawing;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class InventoryDialog : DXWindow
    {
        #region Properties

        public DXItemGrid Grid;

        public DXLabel GoldLabel, WeightLabel;
        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            if (!IsVisible)
                Grid.ClearLinks();

            base.OnIsVisibleChanged(oValue, nValue);
        }

        public override WindowType Type => WindowType.InventoryBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisiblity => true;
        private DXButton RemoteSellButton;
        private DXButton RemoteFragmentButton;
        public void OpenFragmentDialog()
        {
            GameScene.Game.NPCItemFragmentBox.Visible = true;
            GameScene.Game.NPCItemFragmentBox.Location = new Point(Size.Width - GameScene.Game.NPCItemFragmentBox.Size.Width, Size.Height);
        }
        public void OpenSellGoodsDialog()
        {
            GameScene.Game.NPCSellBox.Visible = true;
            GameScene.Game.NPCSellBox.Location = new Point(0, Size.Height);
        }
        #endregion

        public InventoryDialog()
        {
            TitleLabel.Text = "背包";
            
            Grid = new DXItemGrid
            {
                GridSize = new Size(7, 7),
                Parent = this,
                ItemGrid = GameScene.Game.Inventory,
                GridType = GridType.Inventory
            };

            SetClientSize(new Size(Grid.Size.Width, Grid.Size.Height+ 85));
            Grid.Location = ClientArea.Location;


            GoldLabel = new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(99, 83, 50),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left + 80, ClientArea.Bottom - 81),
                Text = "0",
                Size = new Size(ClientArea.Width - 81, 20),
                Sound = SoundIndex.GoldPickUp
            };
            GoldLabel.MouseClick += GoldLabel_MouseClick;

            new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(99, 83, 50),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left + 1, ClientArea.Bottom - 81),
                Text = "金币",
                Size = new Size(78, 20),
                IsControl = false,
            };


            WeightLabel = new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(99, 83, 50),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left + 80, ClientArea.Bottom - 60),
                Text = "0",
                Size = new Size(ClientArea.Width - 81, 20),
                Sound = SoundIndex.GoldPickUp
            };

            new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(99, 83, 50),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(ClientArea.Left + 1, ClientArea.Bottom - 60),
                Text = "重量",
                Size = new Size(78, 20),
                IsControl = false,
            };

            RemoteSellButton = new DXButton
            {
                Location = new Point(ClientArea.Left + 1, ClientArea.Bottom - 35),
                Size = new Size(78, DefaultHeight),
                Parent = this,
                Label = { Text = "远程出售" },
                Enabled = true,
            };
            RemoteSellButton.MouseClick += (o, e) => OpenSellGoodsDialog();

            RemoteFragmentButton = new DXButton
            {
                Location = new Point(ClientArea.Left + 80, ClientArea.Bottom - 35),
                Size = new Size(78, DefaultHeight),
                Parent = this,
                Label = { Text = "远程拆解" },
                Enabled = true,
            };
            RemoteFragmentButton.MouseClick += (o, e) => OpenFragmentDialog();
        }

        #region Methods
        private void GoldLabel_MouseClick(object sender, MouseEventArgs e)
        {
            if (GameScene.Game.SelectedCell == null)
                GameScene.Game.GoldPickedUp = !GameScene.Game.GoldPickedUp && MapObject.User.Gold > 0;
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Grid != null)
                {
                    if (!Grid.IsDisposed)
                        Grid.Dispose();

                    Grid = null;
                }

                if (GoldLabel != null)
                {
                    if (!GoldLabel.IsDisposed)
                        GoldLabel.Dispose();

                    GoldLabel = null;
                }

                if (WeightLabel != null)
                {
                    if (!WeightLabel.IsDisposed)
                        WeightLabel.Dispose();

                    WeightLabel = null;
                }
            }

        }

        #endregion
    }

}