using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MirDB;

namespace Client.UserModels
{
    [UserObject]
    public sealed class KeyBindInfo : DBObject
    {
        public string Category
        {
            get { return _Category; }
            set
            {
                if (_Category == value) return;

                var oldValue = _Category;
                _Category = value;

                OnChanged(oldValue, value, "Category");
            }
        }
        private string _Category;
        
        public KeyBindAction Action
        {
            get { return _Action; }
            set
            {
                if (_Action == value) return;

                var oldValue = _Action;
                _Action = value;

                OnChanged(oldValue, value, "Action");
            }
        }
        private KeyBindAction _Action;

        public bool Control1
        {
            get { return _Control1; }
            set
            {
                if (_Control1 == value) return;

                var oldValue = _Control1;
                _Control1 = value;

                OnChanged(oldValue, value, "Control1");
            }
        }
        private bool _Control1;

        public bool Alt1
        {
            get { return _Alt1; }
            set
            {
                if (_Alt1 == value) return;

                var oldValue = _Alt1;
                _Alt1 = value;

                OnChanged(oldValue, value, "Alt1");
            }
        }
        private bool _Alt1;

        public bool Shift1
        {
            get { return _Shift1; }
            set
            {
                if (_Shift1 == value) return;

                var oldValue = _Shift1;
                _Shift1 = value;

                OnChanged(oldValue, value, "Shift1");
            }
        }
        private bool _Shift1;

        public Keys Key1
        {
            get { return _Key1; }
            set
            {
                if (_Key1 == value) return;

                var oldValue = _Key1;
                _Key1 = value;

                OnChanged(oldValue, value, "Key1");
            }
        }
        private Keys _Key1;
        

        public bool Control2
        {
            get { return _Control2; }
            set
            {
                if (_Control2 == value) return;

                var oldValue = _Control2;
                _Control2 = value;

                OnChanged(oldValue, value, "Control2");
            }
        }
        private bool _Control2;

        public bool Shift2
        {
            get { return _Shift2; }
            set
            {
                if (_Shift2 == value) return;

                var oldValue = _Shift2;
                _Shift2 = value;

                OnChanged(oldValue, value, "Shift2");
            }
        }
        private bool _Shift2;

        public bool Alt2
        {
            get { return _Alt2; }
            set
            {
                if (_Alt2 == value) return;

                var oldValue = _Alt2;
                _Alt2 = value;

                OnChanged(oldValue, value, "Alt2");
            }
        }
        private bool _Alt2;

        public Keys Key2
        {
            get { return _Key2; }
            set
            {
                if (_Key2 == value) return;

                var oldValue = _Key2;
                _Key2 = value;

                OnChanged(oldValue, value, "Key2");
            }
        }
        private Keys _Key2;
    }

    public enum KeyBindAction
    {
        None,

        [Description("设置界面")]
        ConfigWindow,
        [Description("角色界面")]
        CharacterWindow,
        [Description("背包界面")]
        InventoryWindow,
        [Description("技能界面")]
        MagicWindow,
        [Description("技能快捷键")]
        MagicBarWindow,
        [Description("排行榜")]
        RankingWindow,
        [Description("元宝商铺")]
        GameStoreWindow,
        [Description("宠物界面")]
        CompanionWindow,
        [Description("组队界面")]
        GroupWindow,
        [Description("自动喝药界面")]
        AutoPotionWindow,
        [Description("个人仓库界面")]
        StorageWindow,
        [Description("黑名单界面")]
        BlockListWindow,
        [Description("行会界面")]
        GuildWindow,
        [Description("任务界面")]
        QuestLogWindow,
        [Description("任务跟踪界面")]
        QuestTrackerWindow,
        [Description("快捷键界面")]
        BeltWindow,
        [Description("元宝商铺界面")]
        MarketPlaceWindow,
        [Description("小地图界面")]
        MapMiniWindow,
        [Description("大地图界面")]
        MapBigWindow,
        [Description("邮箱界面")]
        MailBoxWindow,
        [Description("发送邮件界面")]
        MailSendWindow,
        [Description("聊天设置界面")]
        ChatOptionsWindow,
        [Description("退出游戏见面")]
        ExitGameWindow,


        [Description("切换攻击模式")]
        ChangeAttackMode,
        [Description("切换宠物模式")]
        ChangePetMode,

        [Description("组队开关")]
        GroupAllowSwitch,
        [Description("添加进组队")]
        GroupTarget,

        [Description("请求交易")]
        TradeRequest,
        [Description("交易开关")]
        TradeAllowSwitch,

        [Description("物品拾取")]
        ItemPickUp,

        [Description("夫妻传送")]
        PartnerTeleport,

        [Description("骑马开关")]
        MountToggle,
        [Description("自动跑步开关")]
        AutoRunToggle,
        [Description("切换聊天模式")]
        ChangeChatMode,


        [Description("使用快捷栏物品 1")]
        UseBelt01,
        [Description("使用快捷栏物品 2")]
        UseBelt02,
        [Description("使用快捷栏物品 3")]
        UseBelt03,
        [Description("使用快捷栏物品 4")]
        UseBelt04,
        [Description("使用快捷栏物品 5")]
        UseBelt05,
        [Description("使用快捷栏物品 6")]
        UseBelt06,
        [Description("使用快捷栏物品 7")]
        UseBelt07,
        [Description("使用快捷栏物品 8")]
        UseBelt08,
        [Description("使用快捷栏物品 9")]
        UseBelt09,
        [Description("使用快捷栏物品 10")]
        UseBelt10,

        [Description("切换技能条 1")]
        SpellSet01,
        [Description("切换技能条 2")]
        SpellSet02,
        [Description("切换技能条 3")]
        SpellSet03,
        [Description("切换技能条 4")]
        SpellSet04,

        [Description("使用技能 1")]
        SpellUse01,
        [Description("使用技能 2")]
        SpellUse02,
        [Description("使用技能 3")]
        SpellUse03,
        [Description("使用技能 4")]
        SpellUse04,
        [Description("使用技能 5")]
        SpellUse05,
        [Description("使用技能 6")]
        SpellUse06,
        [Description("使用技能 7")]
        SpellUse07,
        [Description("使用技能 8")]
        SpellUse08,
        [Description("使用技能 9")]
        SpellUse09,
        [Description("使用技能 10")]
        SpellUse10,
        [Description("使用技能 11")]
        SpellUse11,
        [Description("使用技能 12")]
        SpellUse12,
        [Description("使用技能 13")]
        SpellUse13,
        [Description("使用技能 14")]
        SpellUse14,
        [Description("使用技能 15")]
        SpellUse15,
        [Description("使用技能 16")]
        SpellUse16,
        [Description("使用技能 17")]
        SpellUse17,
        [Description("使用技能 18")]
        SpellUse18,
        [Description("使用技能 19")]
        SpellUse19,
        [Description("使用技能 20")]
        SpellUse20,
        [Description("使用技能 21")]
        SpellUse21,
        [Description("使用技能 22")]
        SpellUse22,
        [Description("使用技能 23")]
        SpellUse23,
        [Description("使用技能 24")]
        SpellUse24,
        [Description("锁定/解锁物品")]
        ToggleItemLock,

        [Description("财富界面")]
        FortuneWindow
    }
}
