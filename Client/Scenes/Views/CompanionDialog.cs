using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;
using Library.SystemModels;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class CompanionDialog : DXWindow
    {
        #region Properties

        public DXItemCell[] EquipmentGrid;
        public DXItemGrid InventoryGrid;

        public MonsterObject CompanionDisplay;
        public Point CompanionDisplayPoint;

        public DXLabel WeightLabel, HungerLabel, NameLabel, LevelLabel, ExperienceLabel, Level3Label, Level5Label, Level7Label, Level10Label, Level11Label, Level13Label, Level15Label;
        public DXComboBox ModeComboBox;

        // feature 拾取过滤 物品显示过滤
        private DXTabControl CompanionTabControl;
        private DXTab CompanionBagTab, PickUpFilterTab, ItemNameFilterTab;
        private DXTextBox PickupFilterText;
        private DXControl WarriorSkillPanel;
        private DXLabel WarriorSkillPanelTitle;
        private DXControl WizardSkillPanel;
        private DXLabel WizardSkillPanelTitle;
        private DXControl TaoistSkillPanel;
        private DXLabel TaoistSkillPanelTitle;
        private DXControl AssassinSkillPanel;
        private DXLabel AssassinSkillPanelTitle;
        public DXCheckBox AutoBladeStormSkillCheckBox;
        public DXCheckBox AutoFlamingSwordSkillCheckBox;
        public DXCheckBox AutoMightSkillCheckBox;
        public DXCheckBox AutoMagicShieldSkillCheckBox;
        public DXCheckBox AutoRenounceSkillCheckBox;
        public DXCheckBox AutoCelestialLightSkillCheckBox;
        public DXCheckBox AutoFourFlowerSkillCheckBox;
        public DXCheckBox AutoRagingWindSkillCheckBox;
        public DXCheckBox AutoEvasionSkillCheckBox;
        // feature end

        public int BagWeight, MaxBagWeight, InventorySize;


        public override WindowType Type => WindowType.CompanionBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisiblity => true;

        #endregion

        public CompanionDialog()
        {
            TitleLabel.Text = "宠物";
            SetClientSize(new Size(352, 590));

            // feature 拾取过滤 物品显示过滤
            HasTitle = true;
            CompanionTabControl = new DXTabControl
            {
                Parent = this,
                Location = ClientArea.Location,
                Size = ClientArea.Size,
            };

            CompanionBagTab = new DXTab
            {
                Parent = CompanionTabControl,
                Border = true,
                TabButton = { Label = { Text = "宠物背包" } },
            };
            PickUpFilterTab = new DXTab
            {
                Parent = CompanionTabControl,
                Border = true,
                TabButton = { Label = { Text = "拾取过滤" } },
            };
            ItemNameFilterTab = new DXTab
            {
                Parent = CompanionTabControl,
                Border = true,
                TabButton = { Label = { Text = "显示过滤" } },
            };

            CompanionTabControl.SelectedTab = CompanionBagTab;

            PickupFilterText = new DXTextBox
            {
                Size = new Size(PickUpFilterTab.Size.Width, 400),
                Location = new Point(0, 400),
                Parent = PickUpFilterTab,
                MaxLength = 999999,
                AllowResize = true,
                CanResizeHeight = true,
                BorderSize = 2,
                BorderColour = Color.FromArgb(198, 166, 99),
                Opacity = 0.35f,
            };

            // feature end

            //CompanionDisplayPoint = new Point(ClientArea.X + 60, ClientArea.Y + 50);
            CompanionDisplayPoint = new Point(60, 120);

            InventoryGrid = new DXItemGrid
            {
                GridSize = new Size(10, 10),
                Parent = CompanionBagTab,
                GridType = GridType.CompanionInventory,
                Location = new Point(0, 200),
            };

            EquipmentGrid = new DXItemCell[Globals.CompanionEquipmentSize];
            DXItemCell cell;
            EquipmentGrid[(int)CompanionSlot.Bag] = cell = new DXItemCell
            {
                Location = new Point(196, 5),
                Parent = CompanionBagTab,
                FixedBorder = true,
                Border = true,
                Slot = (int)CompanionSlot.Bag,
                GridType = GridType.CompanionEquipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 99);

            EquipmentGrid[(int)CompanionSlot.Head] = cell = new DXItemCell
            {
                Location = new Point(236, 5),
                Parent = CompanionBagTab,
                FixedBorder = true,
                Border = true,
                Slot = (int)CompanionSlot.Head,
                GridType = GridType.CompanionEquipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 100);

            EquipmentGrid[(int)CompanionSlot.Back] = cell = new DXItemCell
            {
                Location = new Point(276, 5),
                Parent = CompanionBagTab,
                FixedBorder = true,
                Border = true,
                Slot = (int)CompanionSlot.Back,
                GridType = GridType.CompanionEquipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 101);

            EquipmentGrid[(int)CompanionSlot.Food] = cell = new DXItemCell
            {
                Location = new Point(316, 5),
                Parent = CompanionBagTab,
                FixedBorder = true,
                Border = true,
                Slot = (int)CompanionSlot.Food,
                GridType = GridType.CompanionEquipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 102);

            DXCheckBox PickUpCheckBox = new DXCheckBox
            {
                Parent = CompanionBagTab,
                Label = { Text = "拾取物品:" },
                Visible = false
            };
            //PickUpCheckBox.Location = new Point(ClientArea.Right - PickUpCheckBox.Size.Width +3, ClientArea.Y + 45);
            PickUpCheckBox.Location = new Point(60, 90);

            /*
            new DXLabel
            {
                AutoSize = false,
                Parent = this,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Abilities",
                Location = new Point(ClientArea.X + 196, CompanionDisplayPoint.Y - 20),
                Size = new Size(156, 20),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };
            */

            DXLabel label = new DXLabel
            {
                Parent = CompanionBagTab,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Level 3",
            };
            label.Location = new Point(235 - label.Size.Width, CompanionDisplayPoint.Y - 70);

            Level3Label = new DXLabel
            {
                Parent = CompanionBagTab,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(235, CompanionDisplayPoint.Y - 67),
                Text = "不可用"
            };

            label = new DXLabel
            {
                Parent = CompanionBagTab,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Level 5",
            };
            label.Location = new Point(235 - label.Size.Width, CompanionDisplayPoint.Y - 50);

            Level5Label = new DXLabel
            {
                Parent = CompanionBagTab,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(235, CompanionDisplayPoint.Y - 47),
                Text = "不可用"
            };

            label = new DXLabel
            {
                Parent = CompanionBagTab,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Level 7",
            };
            label.Location = new Point(235 - label.Size.Width, CompanionDisplayPoint.Y - 30);

            Level7Label = new DXLabel
            {
                Parent = CompanionBagTab,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(235, CompanionDisplayPoint.Y - 27),
                Text = "不可用"
            };


            label = new DXLabel
            {
                Parent = CompanionBagTab,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Level 10",
            };
            label.Location = new Point(235 - label.Size.Width, CompanionDisplayPoint.Y - 10);

            Level10Label = new DXLabel
            {
                Parent = CompanionBagTab,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(235, CompanionDisplayPoint.Y - 7),
                Text = "不可用"
            };


            label = new DXLabel
            {
                Parent = CompanionBagTab,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Level 11",
            };
            label.Location = new Point(235 - label.Size.Width, CompanionDisplayPoint.Y + 10);

            Level11Label = new DXLabel
            {
                Parent = CompanionBagTab,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(235, CompanionDisplayPoint.Y + 13),
                Text = "不可用"
            };

            label = new DXLabel
            {
                Parent = CompanionBagTab,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Level 13",
            };
            label.Location = new Point(235 - label.Size.Width, CompanionDisplayPoint.Y + 30);

            Level13Label = new DXLabel
            {
                Parent = CompanionBagTab,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(235, CompanionDisplayPoint.Y + 33),
                Text = "不可用"
            };

            label = new DXLabel
            {
                Parent = CompanionBagTab,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "Level 15",
            };
            label.Location = new Point(235 - label.Size.Width, CompanionDisplayPoint.Y + 50);

            Level15Label = new DXLabel
            {
                Parent = CompanionBagTab,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(235, CompanionDisplayPoint.Y + 53),
                Text = "不可用"
            };

            NameLabel = new DXLabel
            {
                Parent = CompanionBagTab,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(CompanionDisplayPoint.X + 25, CompanionDisplayPoint.Y - 27)
            };

            label = new DXLabel
            {
                Parent = CompanionBagTab,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "名字",
            };
            label.Location = new Point(CompanionDisplayPoint.X + 30 - label.Size.Width, CompanionDisplayPoint.Y - 30);

            LevelLabel = new DXLabel
            {
                Parent = CompanionBagTab,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(CompanionDisplayPoint.X + 25, CompanionDisplayPoint.Y - 7)
            };

            label = new DXLabel
            {
                Parent = CompanionBagTab,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "等级",
            };
            label.Location = new Point(CompanionDisplayPoint.X + 30 - label.Size.Width, CompanionDisplayPoint.Y - 10);

            ExperienceLabel = new DXLabel
            {
                Parent = CompanionBagTab,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(CompanionDisplayPoint.X + 25, CompanionDisplayPoint.Y + 13)
            };

            label = new DXLabel
            {
                Parent = CompanionBagTab,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "经验",
            };
            label.Location = new Point(CompanionDisplayPoint.X + 30 - label.Size.Width, CompanionDisplayPoint.Y + 10);

            HungerLabel = new DXLabel
            {
                Parent = CompanionBagTab,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(CompanionDisplayPoint.X + 25, CompanionDisplayPoint.Y + 33)
            };

            label = new DXLabel
            {
                Parent = CompanionBagTab,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "饥饿度",
            };
            label.Location = new Point(CompanionDisplayPoint.X + 30 - label.Size.Width, CompanionDisplayPoint.Y + 30);

            WeightLabel = new DXLabel
            {
                Parent = CompanionBagTab,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(CompanionDisplayPoint.X + 25, CompanionDisplayPoint.Y + 53)
            };

            label = new DXLabel
            {
                Parent = CompanionBagTab,
                Outline = true,
                Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "重量",
            };
            label.Location = new Point(CompanionDisplayPoint.X + 30 - label.Size.Width, CompanionDisplayPoint.Y + 50);
        }

        #region Methods
        public void CompanionChanged()
        {
            if (GameScene.Game.Companion == null)
            {
                Visible = false;
                return;
            }

            InventoryGrid.ItemGrid = GameScene.Game.Companion.InventoryArray;

            foreach (DXItemCell cell in EquipmentGrid)
                cell.ItemGrid = GameScene.Game.Companion.EquipmentArray;

            CompanionDisplay = new MonsterObject(GameScene.Game.Companion.CompanionInfo);
            NameLabel.Text = GameScene.Game.Companion.Name;

            Refresh();

        }

        public void Draw(DXItemCell cell, int index)
        {
            if (InterfaceLibrary == null) return;

            if (cell.Item != null) return;

            Size s = InterfaceLibrary.GetSize(index);
            int x = (cell.Size.Width - s.Width) / 2 + cell.DisplayArea.X;
            int y = (cell.Size.Height - s.Height) / 2 + cell.DisplayArea.Y;

            InterfaceLibrary.Draw(index, x, y, Color.White, false, 0.2F, ImageType.Image);
        }

        public override void Process()
        {
            base.Process();

            CompanionDisplay?.Process();
        }

        protected override void OnAfterDraw()
        {
            base.OnAfterDraw();


            if (CompanionDisplay == null) return;
            if (CompanionTabControl.SelectedTab != CompanionBagTab) return;

            int x = DisplayArea.X + CompanionDisplayPoint.X;
            int y = DisplayArea.Y + CompanionDisplayPoint.Y;

            if (CompanionDisplay.Image == MonsterImage.Companion_Donkey)
            {
                x += 10;
                y -= 5;
            }


            CompanionDisplay.DrawShadow(x, y);
            CompanionDisplay.DrawBody(x, y);
        }

        public void Refresh()
        {
            LevelLabel.Text = GameScene.Game.Companion.Level.ToString();

            CompanionLevelInfo info = Globals.CompanionLevelInfoList.Binding.First(x => x.Level == GameScene.Game.Companion.Level);

            ExperienceLabel.Text = info.MaxExperience > 0 ? $"{GameScene.Game.Companion.Experience/(decimal) info.MaxExperience:p2}" : "100%";

            HungerLabel.Text = $"{GameScene.Game.Companion.Hunger} of {info.MaxHunger}";

            WeightLabel.Text = $"{BagWeight} / {MaxBagWeight}";

            WeightLabel.ForeColour = BagWeight >= MaxBagWeight ? Color.Red : Color.White;

            Level3Label.Text = GameScene.Game.Companion.Level3 == null ? "不可用" : GameScene.Game.Companion.Level3.GetDisplay(GameScene.Game.Companion.Level3.Values.Keys.First());

            Level5Label.Text = GameScene.Game.Companion.Level5 == null ? "不可用" : GameScene.Game.Companion.Level5.GetDisplay(GameScene.Game.Companion.Level5.Values.Keys.First());
            
            Level7Label.Text = GameScene.Game.Companion.Level7 == null ? "不可用" : GameScene.Game.Companion.Level7.GetDisplay(GameScene.Game.Companion.Level7.Values.Keys.First());

            Level10Label.Text = GameScene.Game.Companion.Level10 == null ? "不可用" : GameScene.Game.Companion.Level10.GetDisplay(GameScene.Game.Companion.Level10.Values.Keys.First());

            Level11Label.Text = GameScene.Game.Companion.Level11 == null ? "不可用" : GameScene.Game.Companion.Level11.GetDisplay(GameScene.Game.Companion.Level11.Values.Keys.First());

            Level13Label.Text = GameScene.Game.Companion.Level13 == null ? "不可用" : GameScene.Game.Companion.Level13.GetDisplay(GameScene.Game.Companion.Level13.Values.Keys.First());

            Level15Label.Text = GameScene.Game.Companion.Level15 == null ? "不可用" : GameScene.Game.Companion.Level15.GetDisplay(GameScene.Game.Companion.Level15.Values.Keys.First());

            for (int i = 0; i < InventoryGrid.Grid.Length; i++)
                InventoryGrid.Grid[i].Enabled = i < InventorySize;
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                CompanionDisplay = null;
                CompanionDisplayPoint = Point.Empty;

                if (CompanionTabControl != null)
                {
                    if (!CompanionTabControl.IsDisposed)
                        CompanionTabControl.Dispose();

                    CompanionTabControl = null;
                }

                if (CompanionBagTab != null)
                {
                    if (!CompanionBagTab.IsDisposed)
                        CompanionBagTab.Dispose();

                    CompanionBagTab = null;
                }

                if (PickUpFilterTab != null)
                {
                    if (!PickUpFilterTab.IsDisposed)
                        PickUpFilterTab.Dispose();

                    PickUpFilterTab = null;
                }

                if (ItemNameFilterTab != null)
                {
                    if (!ItemNameFilterTab.IsDisposed)
                        ItemNameFilterTab.Dispose();

                    ItemNameFilterTab = null;
                }

                if (EquipmentGrid != null)
                {
                    for (int i = 0; i < EquipmentGrid.Length; i++)
                    {
                        if (EquipmentGrid[i] != null)
                        {
                            if (!EquipmentGrid[i].IsDisposed)
                                EquipmentGrid[i].Dispose();

                            EquipmentGrid[i] = null;
                        }
                    }

                    EquipmentGrid = null;
                }

                if (InventoryGrid != null)
                {
                    if (!InventoryGrid.IsDisposed)
                        InventoryGrid.Dispose();

                    InventoryGrid = null;
                }

                if (WeightLabel != null)
                {
                    if (!WeightLabel.IsDisposed)
                        WeightLabel.Dispose();

                    WeightLabel = null;
                }

                if (HungerLabel != null)
                {
                    if (!HungerLabel.IsDisposed)
                        HungerLabel.Dispose();

                    HungerLabel = null;
                }

                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

                if (LevelLabel != null)
                {
                    if (!LevelLabel.IsDisposed)
                        LevelLabel.Dispose();

                    LevelLabel = null;
                }

                if (ExperienceLabel != null)
                {
                    if (!ExperienceLabel.IsDisposed)
                        ExperienceLabel.Dispose();

                    ExperienceLabel = null;
                }

                if (Level3Label != null)
                {
                    if (!Level3Label.IsDisposed)
                        Level3Label.Dispose();

                    Level3Label = null;
                }

                if (Level5Label != null)
                {
                    if (!Level5Label.IsDisposed)
                        Level5Label.Dispose();

                    Level5Label = null;
                }

                if (Level7Label != null)
                {
                    if (!Level7Label.IsDisposed)
                        Level7Label.Dispose();

                    Level7Label = null;
                }

                if (Level10Label != null)
                {
                    if (!Level10Label.IsDisposed)
                        Level10Label.Dispose();

                    Level10Label = null;
                }

                if (ModeComboBox != null)
                {
                    if (!ModeComboBox.IsDisposed)
                        ModeComboBox.Dispose();

                    ModeComboBox = null;
                }

                BagWeight = 0;
                MaxBagWeight = 0;
                InventorySize = 0;
            }

        }

        #endregion
    }
}
