using System;
using System.Collections.Generic;
using System.Drawing;
using Client.Controls;
using Client.UserModels;
using Library;
using Library.SystemModels;

namespace Client.Scenes.Views
{
	public sealed class MonsterDropItemsDialog : DXWindow
	{
		public override WindowType Type
		{
			get
			{
				return WindowType.None;
			}
		}

		public override bool CustomSize
		{
			get
			{
				return false;
			}
		}

		public override bool AutomaticVisiblity
		{
			get
			{
				return false;
			}
		}

		public MonsterDropItemsDialog()
		{
			base.TitleLabel.Text = "怪物爆率";
			base.SetClientSize(new Size(485, 551));
			this.SearchRows = new MonsterDropItemRow[9];
			this.SearchScrollBar = new DXVScrollBar
			{
				Parent = this,
				Location = new Point(base.ClientArea.Size.Width - 14 + base.ClientArea.Left, base.ClientArea.Y + 5),
				Size = new Size(14, base.ClientArea.Height - 5),
				VisibleSize = this.SearchRows.Length,
				Change = 3
			};
			this.SearchScrollBar.ValueChanged += this.SearchScrollBar_ValueChanged;
			for (int i = 0; i < this.SearchRows.Length; i++)
			{
				int num = i;
				this.SearchRows[num] = new MonsterDropItemRow
				{
					Parent = this,
					Location = new Point(base.ClientArea.X, base.ClientArea.Y + 5 + i * 58)
				};
				this.SearchRows[num].MouseWheel += this.SearchScrollBar.DoMouseWheel;
			}
		}

		
		public void Bind(MonsterInfo monster)
		{
			this.SearchResults = new List<DropInfo>();
			this.SearchScrollBar.MaxValue = 0;
			foreach (MonsterDropItemRow monsterDropItemRow in this.SearchRows)
			{
				monsterDropItemRow.Visible = true;
			}
			foreach (DropInfo dropInfo in Globals.DropInfoList.Binding)
			{
				bool flag = dropInfo.Monster == monster;
				if (flag)
				{
					this.SearchResults.Add(dropInfo);
				}
			}
			this.RefreshList();
			base.Visible = true;
			base.TitleLabel.Text = "怪物爆率:" + monster.MonsterName;
		}

		
		public void RefreshList()
		{
			bool flag = this.SearchResults == null;
			if (!flag)
			{
				this.SearchScrollBar.MaxValue = this.SearchResults.Count;
				for (int i = 0; i < this.SearchRows.Length; i++)
				{
					bool flag2 = i + this.SearchScrollBar.Value >= this.SearchResults.Count;
					if (flag2)
					{
						this.SearchRows[i].DropInfo = null;
						this.SearchRows[i].Visible = false;
					}
					else
					{
						this.SearchRows[i].DropInfo = this.SearchResults[i + this.SearchScrollBar.Value];
					}
				}
			}
		}

		
		private void SearchScrollBar_ValueChanged(object sender, EventArgs e)
		{
			this.RefreshList();
		}

		
		public DXVScrollBar SearchScrollBar;

		
		public List<DropInfo> SearchResults;

		
		private MonsterDropItemRow[] SearchRows;
	}

    public sealed class MonsterDropItemRow : DXControl
    {
        
        
        
        public bool Selected
        {
            get
            {
                return this._Selected;
            }
            set
            {
                bool flag = this._Selected == value;
                if (!flag)
                {
                    bool selected = this._Selected;
                    this._Selected = value;
                    this.OnSelectedChanged(selected, value);
                }
            }
        }

        public event EventHandler<EventArgs> SelectedChanged;

        
        public void OnSelectedChanged(bool oValue, bool nValue)
        {
            base.BackColour = (this.Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0));
            this.ItemCell.BorderColour = (this.Selected ? Color.FromArgb(198, 166, 99) : Color.FromArgb(99, 83, 50));
            EventHandler<EventArgs> selectedChanged = this.SelectedChanged;
            if (selectedChanged != null)
            {
                selectedChanged(this, EventArgs.Empty);
            }
        }

        
        
        
        public DropInfo DropInfo
        {
            get
            {
                return this._dropInfo;
            }
            set
            {
                DropInfo dropInfo = this._dropInfo;
                this._dropInfo = value;
                this.OnItemInfoChanged(dropInfo, value);
            }
        }

        public event EventHandler<EventArgs> ItemInfoChanged;

        
        public void OnItemInfoChanged(DropInfo oValue, DropInfo nValue)
        {
            EventHandler<EventArgs> itemInfoChanged = this.ItemInfoChanged;
            if (itemInfoChanged != null)
            {
                itemInfoChanged(this, EventArgs.Empty);
            }
            base.Visible = (nValue != null);
            bool flag = nValue == null;
            if (!flag)
            {
                this.ItemCell.Item = new ClientUserItem(nValue.Item, 1L);
                this.ItemCell.RefreshItem();
                this.NameLabel.Text = nValue.Item.ItemName;
                this.NameLabel.ForeColour = Color.FromArgb(198, 166, 99);
                this.UpdateInfo();
                EventHandler<EventArgs> itemInfoChanged2 = this.ItemInfoChanged;
                if (itemInfoChanged2 != null)
                {
                    itemInfoChanged2(this, EventArgs.Empty);
                }
            }
        }

        
        private void UpdateInfo()
        {
            bool flag = this.DropInfo == null;
            if (flag)
            {
                this.CountLabel.Text = "未知";
                this.ProgressLabel.Text = "未知";
                this.DateLabel.Text = "未知";
            }
            else
            {
                this.CountLabel.Text = this.DropInfo.Amount.ToString("#,##0");
                this.ProgressLabel.Text = string.Format("1/{0}", this.DropInfo.Chance);
                this.DateLabel.Text = (this.DropInfo.PartOnly ? "仅部件" : "正常");
            }
        }

        
        public MonsterDropItemRow()
        {
            this.Size = new Size(465, 55);
            base.DrawTexture = true;
            base.BackColour = (this.Selected ? Color.FromArgb(80, 80, 125) : Color.FromArgb(25, 20, 0));
            base.Visible = false;
            this.ItemCell = new DXItemCell
            {
                Parent = this,
                Location = new Point((this.Size.Height - 36) / 2, (this.Size.Height - 36) / 2),
                FixedBorder = true,
                Border = true,
                ReadOnly = true,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                FixedBorderColour = true
            };
            this.NameLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(this.ItemCell.Location.X + this.ItemCell.Size.Width, 22),
                IsControl = false
            };
            this.CountLabelLabel = new DXLabel
            {
                Parent = this,
                Text = "掉落数量:",
                ForeColour = Color.White,
                IsControl = false
            };
            this.CountLabelLabel.Location = new Point(320 - this.CountLabelLabel.Size.Width, 5);
            this.CountLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(320, 5),
                IsControl = false
            };
            this.ProgressLabelLabel = new DXLabel
            {
                Parent = this,
                Text = "掉落几率:",
                ForeColour = Color.White,
                IsControl = false
            };
            this.ProgressLabelLabel.Location = new Point(320 - this.ProgressLabelLabel.Size.Width, 20);
            this.ProgressLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(320, 20),
                IsControl = false
            };
            this.DateLabelLabel = new DXLabel
            {
                Parent = this,
                Text = "掉落方式:",
                ForeColour = Color.White,
                IsControl = false
            };
            this.DateLabelLabel.Location = new Point(320 - this.DateLabelLabel.Size.Width, 35);
            this.DateLabel = new DXLabel
            {
                Parent = this,
                Location = new Point(320, 35),
                IsControl = false
            };
        }

        
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this._Selected = false;
                this.SelectedChanged = null;
                this._dropInfo = null;
                this.ItemInfoChanged = null;
                bool flag = this.ItemCell != null;
                if (flag)
                {
                    bool flag2 = !this.ItemCell.IsDisposed;
                    if (flag2)
                    {
                        this.ItemCell.Dispose();
                    }
                    this.ItemCell = null;
                }
                bool flag3 = this.NameLabel != null;
                if (flag3)
                {
                    bool flag4 = !this.NameLabel.IsDisposed;
                    if (flag4)
                    {
                        this.NameLabel.Dispose();
                    }
                    this.NameLabel = null;
                }
                bool flag5 = this.CountLabelLabel != null;
                if (flag5)
                {
                    bool flag6 = !this.CountLabelLabel.IsDisposed;
                    if (flag6)
                    {
                        this.CountLabelLabel.Dispose();
                    }
                    this.CountLabelLabel = null;
                }
                bool flag7 = this.CountLabel != null;
                if (flag7)
                {
                    bool flag8 = !this.CountLabel.IsDisposed;
                    if (flag8)
                    {
                        this.CountLabel.Dispose();
                    }
                    this.CountLabel = null;
                }
                bool flag9 = this.ProgressLabelLabel != null;
                if (flag9)
                {
                    bool flag10 = !this.ProgressLabelLabel.IsDisposed;
                    if (flag10)
                    {
                        this.ProgressLabelLabel.Dispose();
                    }
                    this.ProgressLabelLabel = null;
                }
                bool flag11 = this.ProgressLabel != null;
                if (flag11)
                {
                    bool flag12 = !this.ProgressLabel.IsDisposed;
                    if (flag12)
                    {
                        this.ProgressLabel.Dispose();
                    }
                    this.ProgressLabel = null;
                }
            }
        }

        
        private bool _Selected;

        
        private DropInfo _dropInfo;

        
        public DXItemCell ItemCell;

        
        public DXLabel NameLabel;

        
        public DXLabel CountLabelLabel;

        
        public DXLabel CountLabel;

        
        public DXLabel ProgressLabelLabel;

        
        public DXLabel ProgressLabel;

        
        public DXLabel DateLabel;

        
        public DXLabel TogoLabel;

        
        public DXLabel DateLabelLabel;
    }
}
