using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
        private DXLabel PickUpFilterItemTypelabel;
        public DXTextBox PickUpFilterItemNameBox;
        public DXComboBox PickUpFilterItemTypeBox;
        public DXButton PickUpFilterSearchButton;
        public DXVScrollBar PickupFilterSearchScrollBar;
        public List<ItemInfo> PickUpFilterSearchResults;
        public PickUpFilterRow[] PickUpFilterRow;


        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            e.Handled = true;

            if (PickUpFilterSearchButton.Enabled)
                Search();
        }

        public void RefreshList()
        {
            if (PickUpFilterSearchResults == null) return;

            PickupFilterSearchScrollBar.MaxValue = PickUpFilterSearchResults.Count;

            for (int i = 0; i < PickUpFilterRow.Length; i++)
            {
                if (i + PickupFilterSearchScrollBar.Value >= PickUpFilterSearchResults.Count)
                {
                    PickUpFilterRow[i].ItemInfo = null;
                    PickUpFilterRow[i].Visible = false;
                    continue;
                }

                PickUpFilterRow[i].ItemInfo = PickUpFilterSearchResults[i + PickupFilterSearchScrollBar.Value];
                PickUpFilterRow[i].AutoPickUpCheckBox.Checked = GameScene.Game.AutoPickUpItemList.Contains(PickUpFilterSearchResults[i + PickupFilterSearchScrollBar.Value].ItemName);
                PickUpFilterRow[i].DisplayItemNameCheckBox.Checked = GameScene.Game.DisplayNameItemList.Contains(PickUpFilterSearchResults[i + PickupFilterSearchScrollBar.Value].ItemName);
                PickUpFilterRow[i].HighLightItemNameCheckBox.Checked = GameScene.Game.HightLightItemList.Contains(PickUpFilterSearchResults[i + PickupFilterSearchScrollBar.Value].ItemName);
            }

        }

        private void SearchScrollBar_ValueChanged(object sender, EventArgs e)
        {
            RefreshList();
        }

        public void Search()
        {
            PickUpFilterSearchResults = new List<ItemInfo>();
            PickupFilterSearchScrollBar.MaxValue = 0;
            foreach (var row in PickUpFilterRow)
                row.Visible = true;
            ItemType filter = (ItemType?)PickUpFilterItemTypeBox.SelectedItem ?? 0;
            bool useFilter = PickUpFilterItemTypeBox.SelectedItem != null;
            foreach (ItemInfo info in Globals.ItemInfoList.Binding)
            {
                if (useFilter && info.ItemType != filter) continue;
                if (!string.IsNullOrEmpty(PickUpFilterItemNameBox.TextBox.Text) && info.ItemName.IndexOf(PickUpFilterItemNameBox.TextBox.Text, StringComparison.OrdinalIgnoreCase) < 0) continue;
                PickUpFilterSearchResults.Add(info);
            }

            RefreshList();
        }

        // feature end

        public int BagWeight, MaxBagWeight, InventorySize;


        public override WindowType Type => WindowType.CompanionBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisiblity => true;

        #endregion

        public CompanionDialog()
        {
            TitleLabel.Text = "宠物";
            SetClientSize(new Size(355, 590));

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

            DXControl filterPanel = new DXControl
            {
                Parent = PickUpFilterTab,
                Size = new Size(PickUpFilterTab.Size.Width, 26),
                Location = new Point(0, 0),
                Border = true,
                BorderColour = Color.FromArgb(198, 166, 99)
            };

            DXLabel PickUpFilterItemNameLabel = new DXLabel
            {
                Parent = filterPanel,
                Location = new Point(5, 5),
                Text = "名字:",
            };

            PickUpFilterItemNameBox = new DXTextBox
            {
                Parent = filterPanel,
                Size = new Size(90, 20),
                Location = new Point(PickUpFilterItemNameLabel.Location.X + PickUpFilterItemNameLabel.Size.Width + 5, PickUpFilterItemNameLabel.Location.Y),
            };
            PickUpFilterItemNameBox.TextBox.KeyPress += TextBox_KeyPress;



            PickUpFilterItemTypelabel = new DXLabel
            {
                Parent = filterPanel,
                Location = new Point(PickUpFilterItemNameBox.Location.X + PickUpFilterItemNameBox.Size.Width + 10, 5),
                Text = "物品:",
            };



            PickUpFilterItemTypeBox = new DXComboBox
            {
                Parent = filterPanel,
                Location = new Point(PickUpFilterItemTypelabel.Location.X + PickUpFilterItemTypelabel.Size.Width + 5, PickUpFilterItemTypelabel.Location.Y),
                Size = new Size(72, DXComboBox.DefaultNormalHeight),
                DropDownHeight = 198
            };


            new DXListBoxItem
            {
                Parent = PickUpFilterItemTypeBox.ListBox,
                Label = { Text = "所有" },
                Item = null
            };

            Type itemType = typeof(ItemType);

            for (ItemType i = ItemType.Nothing; i <= ItemType.ItemPart; i++)
            {
                MemberInfo[] infos = itemType.GetMember(i.ToString());

                DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();

                new DXListBoxItem
                {
                    Parent = PickUpFilterItemTypeBox.ListBox,
                    Label = { Text = description?.Description ?? i.ToString() },
                    Item = i
                };
            }

            PickUpFilterItemTypeBox.ListBox.SelectItem(null);

            PickUpFilterSearchButton = new DXButton
            {
                Size = new Size(80, SmallButtonHeight),
                Location = new Point(PickUpFilterItemTypeBox.Location.X + PickUpFilterItemTypeBox.Size.Width + 15, PickUpFilterItemTypelabel.Location.Y - 1),
                Parent = filterPanel,
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "搜索" }
            };
            PickUpFilterSearchButton.MouseClick += (o, e) => Search();

            PickUpFilterRow = new PickUpFilterRow[9];

            PickupFilterSearchScrollBar = new DXVScrollBar
            {
                Parent = PickUpFilterTab,
                Location = new Point(PickUpFilterTab.Size.Width - 14 , filterPanel.Size.Height + 5),
                Size = new Size(14, PickUpFilterTab.Size.Height - 5 - filterPanel.Size.Height),
                VisibleSize = PickUpFilterRow.Length,
                Change = 3,
            };
            PickupFilterSearchScrollBar.ValueChanged += SearchScrollBar_ValueChanged;


            for (int i = 0; i < PickUpFilterRow.Length; i++)
            {
                int index = i;
                PickUpFilterRow[index] = new PickUpFilterRow
                {
                    Parent = PickUpFilterTab,
                    Location = new Point(0, filterPanel.Size.Height + 5 + i * 58),
                };
                //   SearchRows[index].MouseClick += (o, e) => { SelectedRow = SearchRows[index]; };
                PickUpFilterRow[index].MouseWheel += PickupFilterSearchScrollBar.DoMouseWheel;
            }

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

    public sealed class PickUpFilterRow : DXControl
    {

        #region Properties

        #region Selected

        public bool Selected
        {
            get => _Selected;
            set
            {
                if (_Selected == value) return;

                bool oldValue = _Selected;
                _Selected = value;

                OnSelectedChanged(oldValue, value);
            }
        }
        private bool _Selected;
        public event EventHandler<EventArgs> SelectedChanged;
        public void OnSelectedChanged(bool oValue, bool nValue)
        {
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0);
            ItemCell.BorderColour = Selected ? Color.FromArgb(198, 166, 99) : Color.FromArgb(99, 83, 50);

            SelectedChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region ItemInfo

        public ItemInfo ItemInfo
        {
            get { return _ItemInfo; }
            set
            {

                ItemInfo oldValue = _ItemInfo;
                _ItemInfo = value;

                OnItemInfoChanged(oldValue, value);
            }
        }
        private ItemInfo _ItemInfo;
        public event EventHandler<EventArgs> ItemInfoChanged;
        public void OnItemInfoChanged(ItemInfo oValue, ItemInfo nValue)
        {
            ItemInfoChanged?.Invoke(this, EventArgs.Empty);
            Visible = ItemInfo != null;

            if (ItemInfo == null)
            {
                return;
            }

            ItemCell.Item = new ClientUserItem(ItemInfo, 1);
            ItemCell.RefreshItem();

            NameLabel.Text = ItemInfo.ItemName;

            NameLabel.ForeColour = Color.FromArgb(198, 166, 99);

            ItemInfoChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXItemCell ItemCell;
        public DXLabel NameLabel, CountLabelLabel, CountLabel, ProgressLabelLabel, ProgressLabel, DateLabel, TogoLabel, DateLabelLabel;
        public DXButton CheckButton;
        public DXCheckBox AutoPickUpCheckBox, DisplayItemNameCheckBox, HighLightItemNameCheckBox;

        #endregion



        public PickUpFilterRow()
        {
            Size = new Size(335, 55);

            DrawTexture = true;
            BackColour = Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0);

            Visible = false;

            ItemCell = new DXItemCell
            {
                Parent = this,
                Location = new Point((Size.Height - DXItemCell.CellHeight) / 2, (Size.Height - DXItemCell.CellHeight) / 2),
                FixedBorder = true,
                Border = true,
                ReadOnly = true,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                FixedBorderColour = true,
            };

            NameLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(ItemCell.Location.X + ItemCell.Size.Width, 22),
                IsControl = false,
            };

            AutoPickUpCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "自动拾取" },
                Checked = false,
                Location = new Point(220, 5),
                
            };
            AutoPickUpCheckBox.MouseClick += (o, e) =>
            {
                if(!AutoPickUpCheckBox.Checked && !GameScene.Game.AutoPickUpItemList.Contains(ItemInfo.ItemName))
                {
                    GameScene.Game.AutoPickUpItemList.Add(ItemInfo.ItemName);
                }
                else if(AutoPickUpCheckBox.Checked && GameScene.Game.AutoPickUpItemList.Contains(ItemInfo.ItemName))
                {
                    GameScene.Game.AutoPickUpItemList.Remove(ItemInfo.ItemName);
                }
                GameScene.Game.SaveAutoPickUpItemList();
            };

            DisplayItemNameCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "物品显名" },
                Checked = false,
                Location = new Point(220, 20)
            };
            DisplayItemNameCheckBox.MouseClick += (o, e) =>
            {
                if (!DisplayItemNameCheckBox.Checked && !GameScene.Game.DisplayNameItemList.Contains(ItemInfo.ItemName))
                {
                    GameScene.Game.DisplayNameItemList.Add(ItemInfo.ItemName);
                }
                else if (DisplayItemNameCheckBox.Checked && GameScene.Game.DisplayNameItemList.Contains(ItemInfo.ItemName))
                {
                    GameScene.Game.DisplayNameItemList.Remove(ItemInfo.ItemName);
                }
                GameScene.Game.SaveDisplayNameItemList();
            };

            HighLightItemNameCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "物品高亮" },
                Checked = false,
                Location = new Point(220, 35)
            };
            HighLightItemNameCheckBox.MouseClick += (o, e) =>
            {
                if (!HighLightItemNameCheckBox.Checked && !GameScene.Game.HightLightItemList.Contains(ItemInfo.ItemName))
                {
                    GameScene.Game.HightLightItemList.Add(ItemInfo.ItemName);
                }
                else if (HighLightItemNameCheckBox.Checked && GameScene.Game.HightLightItemList.Contains(ItemInfo.ItemName))
                {
                    GameScene.Game.HightLightItemList.Remove(ItemInfo.ItemName);
                }
                GameScene.Game.SaveHightLightItemList();
            };


        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Selected = false;
                SelectedChanged = null;

                _ItemInfo = null;
                ItemInfoChanged = null;


                if (ItemCell != null)
                {
                    if (!ItemCell.IsDisposed)
                        ItemCell.Dispose();

                    ItemCell = null;
                }

                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

                if (AutoPickUpCheckBox != null)
                {
                    if (!AutoPickUpCheckBox.IsDisposed)
                        AutoPickUpCheckBox.Dispose();

                    AutoPickUpCheckBox = null;
                }

                if (DisplayItemNameCheckBox != null)
                {
                    if (!DisplayItemNameCheckBox.IsDisposed)
                        DisplayItemNameCheckBox.Dispose();

                    DisplayItemNameCheckBox = null;
                }

                if (HighLightItemNameCheckBox != null)
                {
                    if (!HighLightItemNameCheckBox.IsDisposed)
                        HighLightItemNameCheckBox.Dispose();

                    HighLightItemNameCheckBox = null;
                }

            }

        }

        #endregion
    }
}
