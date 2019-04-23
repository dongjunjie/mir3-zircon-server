using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Client.Controls;
using Client.UserModels;
using Library;
using Library.SystemModels;

namespace Client.Scenes.Views
{
	
	public sealed class MonsterDropListDialog : DXWindow
	{
		
		public override void OnVisibleChanged(bool oValue, bool nValue)
		{
			base.OnVisibleChanged(oValue, nValue);
			bool flag = !base.Visible && GameScene.Game.ReadMailBox != null;
			if (flag)
			{
				GameScene.Game.ReadMailBox.Visible = false;
			}
		}

		
		public override void OnIsVisibleChanged(bool oValue, bool nValue)
		{
			base.OnIsVisibleChanged(oValue, nValue);
			bool isVisible = base.IsVisible;
			if (isVisible)
			{
				this.RefreshList();
			}
		}

		
		
		public override WindowType Type
		{
			get
			{
				return WindowType.MailBox;
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
				return true;
			}
		}

		
		public MonsterDropListDialog()
		{
			base.TitleLabel.Text = "怪物爆率查询";
			base.HasFooter = false;
			base.SetClientSize(new Size(350, 350));
			DXControl dxcontrol = new DXControl
			{
				Parent = this,
				Size = new Size(base.ClientArea.Width, 26),
				Location = new Point(base.ClientArea.Left, base.ClientArea.Top),
				Border = true,
				BorderColour = Color.FromArgb(198, 166, 99)
			};
			DXLabel dxlabel = new DXLabel
			{
				Parent = dxcontrol,
				Location = new Point(5, 5),
				Text = "名称:"
			};
			this.ItemNameBox = new DXTextBox
			{
				Parent = dxcontrol,
				Size = new Size(180, 20),
				Location = new Point(dxlabel.Location.X + dxlabel.Size.Width + 5, dxlabel.Location.Y)
			};
			this.ItemNameBox.TextBox.KeyPress += delegate(object s, KeyPressEventArgs e)
			{
				bool flag = e.KeyChar != '\r';
				if (!flag)
				{
					e.Handled = true;
					bool enabled = this.SearchButton.Enabled;
					if (enabled)
					{
						this.Search();
					}
				}
			};
			this.SearchButton = new DXButton
			{
				Size = new Size(80, DXControl.SmallButtonHeight),
				Location = new Point(this.ItemNameBox.Location.X + this.ItemNameBox.Size.Width + 15, dxlabel.Location.Y - 1),
				Parent = dxcontrol,
				ButtonType = ButtonType.SmallButton,
				Label = 
				{
					Text = "查找"
				}
			};
			this.SearchButton.MouseClick += delegate(object o, MouseEventArgs e)
			{
				this.Search();
			};
			this.MonsterList.AddRange(Globals.MonsterInfoList.Binding);
			this.Header = new MonsterDropRow
			{
				Parent = this,
				Location = new Point(base.ClientArea.Left, base.ClientArea.Y + dxcontrol.Size.Height + 5),
				IsHeader = true
			};
			this.Rows = new MonsterDropRow[15];
			this.ScrollBar = new DXVScrollBar
			{
				Parent = this,
				Size = new Size(14, base.ClientArea.Height - 2 - 22),
				Location = new Point(base.ClientArea.Right - 14, base.ClientArea.Top + 1 + 27),
				VisibleSize = 15,
				Change = 3
			};
			this.ScrollBar.ValueChanged += this.ScrollBar_ValueChanged;
			base.MouseWheel += this.ScrollBar.DoMouseWheel;
			DXControl parent = new DXControl
			{
				Parent = this,
				Location = new Point(base.ClientArea.Location.X, base.ClientArea.Location.Y + dxcontrol.Size.Height + 5 + this.Header.Size.Height + 2),
				Size = new Size(base.ClientArea.Width - 16, base.ClientArea.Size.Height - 22 - dxcontrol.Size.Height - 5)
			};
			for (int i = 0; i < 15; i++)
			{
				int index = i;
				this.Rows[index] = new MonsterDropRow
				{
					Parent = parent,
					Location = new Point(0, 22 * i),
					Visible = false
				};
				this.Rows[index].MouseClick += delegate(object o, MouseEventArgs e)
				{
					GameScene.Game.MonsterDropItemsBox.Bind(this.Rows[index].Monster);
				};
				this.Rows[index].MouseWheel += this.ScrollBar.DoMouseWheel;
			}
		}

		
		public void Search()
		{
			this.MonsterList = new List<MonsterInfo>();
			this.ScrollBar.MaxValue = 0;
			foreach (MonsterDropRow monsterDropRow in this.Rows)
			{
				monsterDropRow.Visible = true;
			}
			foreach (MonsterInfo monsterInfo in Globals.MonsterInfoList.Binding)
			{
				bool flag = monsterInfo.Drops.Count == 0;
				if (!flag)
				{
					bool flag2 = !string.IsNullOrEmpty(this.ItemNameBox.TextBox.Text) && monsterInfo.MonsterName.IndexOf(this.ItemNameBox.TextBox.Text, StringComparison.OrdinalIgnoreCase) < 0;
					if (!flag2)
					{
						this.MonsterList.Add(monsterInfo);
					}
				}
			}
			this.RefreshList();
		}

		
		private void ScrollBar_ValueChanged(object sender, EventArgs e)
		{
			this.RefreshList();
		}

		
		public void RefreshList()
		{
			bool flag = this.Rows == null;
			if (!flag)
			{
				this.MonsterList.Sort((MonsterInfo x1, MonsterInfo x2) => x1.Index.CompareTo(x2.Index));
				this.ScrollBar.MaxValue = this.MonsterList.Count;
				for (int i = 0; i < this.Rows.Length; i++)
				{
					bool flag2 = i + this.ScrollBar.Value >= this.MonsterList.Count;
					if (flag2)
					{
						this.Rows[i].Monster = null;
					}
					else
					{
						this.Rows[i].Monster = this.MonsterList[i + this.ScrollBar.Value];
					}
				}
			}
		}

		
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				bool flag = this.Header != null;
				if (flag)
				{
					bool flag2 = !this.Header.IsDisposed;
					if (flag2)
					{
						this.Header.Dispose();
					}
					this.Header = null;
				}
				bool flag3 = this.ScrollBar != null;
				if (flag3)
				{
					bool flag4 = !this.ScrollBar.IsDisposed;
					if (flag4)
					{
						this.ScrollBar.Dispose();
					}
					this.ScrollBar = null;
				}
				bool flag5 = this.Rows != null;
				if (flag5)
				{
					for (int i = 0; i < this.Rows.Length; i++)
					{
						bool flag6 = this.Rows[i] != null;
						if (flag6)
						{
							bool flag7 = !this.Rows[i].IsDisposed;
							if (flag7)
							{
								this.Rows[i].Dispose();
							}
							this.Rows[i] = null;
						}
					}
					this.Rows = null;
				}
				this.MonsterList.Clear();
				this.MonsterList = null;
			}
		}

		
		public DXTextBox ItemNameBox;

		
		public DXButton SearchButton;

		
		public MonsterDropRow Header;

		
		public DXVScrollBar ScrollBar;

		
		public MonsterDropRow[] Rows;

		
		public List<MonsterInfo> MonsterList = new List<MonsterInfo>();
	}

    public sealed class MonsterDropRow : DXControl
    {
        
        
        
        public bool IsHeader
        {
            get
            {
                return this._IsHeader;
            }
            set
            {
                bool flag = this._IsHeader == value;
                if (!flag)
                {
                    bool isHeader = this._IsHeader;
                    this._IsHeader = value;
                    this.OnIsHeaderChanged(isHeader, value);
                }
            }
        }

        public event EventHandler<EventArgs> IsHeaderChanged;

        
        public void OnIsHeaderChanged(bool oValue, bool nValue)
        {
            this.NameLabel.Text = "怪物名称";
            this.LevelLabel.Text = "等级";
            this.BossLabel.Text = "BOSS";
            this.NameLabel.ForeColour = Color.FromArgb(198, 166, 99);
            this.LevelLabel.ForeColour = Color.FromArgb(198, 166, 99);
            this.BossLabel.ForeColour = Color.FromArgb(198, 166, 99);
            base.DrawTexture = false;
            base.IsControl = false;
            EventHandler<EventArgs> isHeaderChanged = this.IsHeaderChanged;
            if (isHeaderChanged != null)
            {
                isHeaderChanged(this, EventArgs.Empty);
            }
        }

        
        
        
        public MonsterInfo Monster
        {
            get
            {
                return this._Mail;
            }
            set
            {
                MonsterInfo mail = this._Mail;
                this._Mail = value;
                this.OnMailChanged(mail, value);
            }
        }

        public event EventHandler<EventArgs> MailChanged;

        
        public void OnMailChanged(MonsterInfo oValue, MonsterInfo nValue)
        {
            base.Visible = (nValue != null);
            bool flag = nValue == null;
            if (!flag)
            {
                this.NameLabel.Text = this.Monster.MonsterName;
                this.LevelLabel.Text = this.Monster.Level.ToString();
                this.BossLabel.Text = (this.Monster.IsBoss ? "BOSS" : "");
                this.RefreshIcon();
                EventHandler<EventArgs> mailChanged = this.MailChanged;
                if (mailChanged != null)
                {
                    mailChanged(this, EventArgs.Empty);
                }
            }
        }

        
        public MonsterDropRow()
        {
            this.Size = new Size(333, 20);
            base.DrawTexture = true;
            base.BackColour = Color.FromArgb(25, 20, 0);
            this.NameLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(130, 20),
                Location = new Point(30, 2),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Parent = this,
                IsControl = false
            };
            this.LevelLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(100, 20),
                Location = new Point(this.NameLabel.Location.X + this.NameLabel.Size.Width, 2),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Parent = this,
                IsControl = false
            };
            this.BossLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(70, 20),
                Location = new Point(this.LevelLabel.Location.X + this.LevelLabel.Size.Width, 2),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Parent = this,
                IsControl = false
            };
        }

        
        public override void OnMouseEnter()
        {
            base.OnMouseEnter();
            bool isHeader = this.IsHeader;
            if (!isHeader)
            {
                base.BackColour = Color.FromArgb(80, 80, 125);
            }
        }

        
        public override void OnMouseLeave()
        {
            base.OnMouseLeave();
            bool isHeader = this.IsHeader;
            if (!isHeader)
            {
                base.BackColour = Color.FromArgb(25, 20, 0);
            }
        }

        
        public void RefreshIcon()
        {
        }

        
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this._IsHeader = false;
                this.IsHeaderChanged = null;
                this._Mail = null;
                this.MailChanged = null;
                bool flag = this.NameLabel != null;
                if (flag)
                {
                    bool flag2 = !this.NameLabel.IsDisposed;
                    if (flag2)
                    {
                        this.NameLabel.Dispose();
                    }
                    this.NameLabel = null;
                }
                bool flag3 = this.LevelLabel != null;
                if (flag3)
                {
                    bool flag4 = !this.LevelLabel.IsDisposed;
                    if (flag4)
                    {
                        this.LevelLabel.Dispose();
                    }
                    this.LevelLabel = null;
                }
                bool flag5 = this.BossLabel != null;
                if (flag5)
                {
                    bool flag6 = !this.BossLabel.IsDisposed;
                    if (flag6)
                    {
                        this.BossLabel.Dispose();
                    }
                    this.BossLabel = null;
                }
            }
        }

        
        private bool _IsHeader;

        
        private MonsterInfo _Mail;

        
        public DXLabel NameLabel;

        
        public DXLabel LevelLabel;

        
        public DXLabel BossLabel;
    }

}
