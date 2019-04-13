using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Client.Controls;
using Client.Envir;
using Client.Models;
using Client.UserModels;
using Library;
using Library.SystemModels;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class BuffDialog : DXWindow
    {
        #region Properties
        private Dictionary<ClientBuffInfo, DXImageControl> Icons = new Dictionary<ClientBuffInfo, DXImageControl>();

        public override WindowType Type => WindowType.BuffBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisiblity => true;
        #endregion

        public BuffDialog()
        {
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            CloseButton.Visible = false;
            Opacity = 0.6F;
            
            Size = new Size(30, 30);
        }

        #region Methods
        public void BuffsChanged()
        {
            foreach (DXImageControl control in Icons.Values)
                control.Dispose();

            Icons.Clear();

            List<ClientBuffInfo> buffs = MapObject.User.Buffs.ToList();

            Stats permStats = new Stats();

            for (int i = buffs.Count - 1; i >= 0; i--)
            {
                ClientBuffInfo buff = buffs[i];

                switch (buff.Type)
                {
                    case BuffType.ItemBuff:
                        if (buff.RemainingTime != TimeSpan.MaxValue) continue;

                        permStats.Add(Globals.ItemInfoList.Binding.First(x => x.Index == buff.ItemIndex).Stats);

                        buffs.Remove(buff);
                        break;
                    case BuffType.Ranking:
                    case BuffType.Developer:
                        buffs.Remove(buff);
                        break;
                }
            }
            
            if (permStats.Count > 0)
                buffs.Add(new ClientBuffInfo { Index = 0, Stats = permStats, Type = BuffType.ItemBuffPermanent, RemainingTime = TimeSpan.MaxValue });

            buffs.Sort((x1, x2) => x2.RemainingTime.CompareTo(x1.RemainingTime));




            foreach (ClientBuffInfo buff in buffs)
            {
                DXImageControl icon;
                Icons[buff] = icon = new DXImageControl
                {
                    Parent = this,
                    LibraryFile = LibraryFile.CBIcon,
                };

                switch (buff.Type)
                {
                    case BuffType.Heal:
                        icon.Index = 78;
                        break;
                    case BuffType.Invisibility:
                        icon.Index = 74;
                        break;
                    case BuffType.MagicResistance:
                        icon.Index = 92;
                        break;
                    case BuffType.Resilience:
                        icon.Index = 91;
                        break;
                    case BuffType.PoisonousCloud:
                        icon.Index = 98;
                        break;
                    case BuffType.Castle:
                        icon.Index = 242;
                        break;
                    case BuffType.FullBloom:
                        icon.Index = 162;
                        break;
                    case BuffType.WhiteLotus:
                        icon.Index = 163;
                        break;
                    case BuffType.RedLotus:
                        icon.Index = 164;
                        break;
                    case BuffType.MagicShield:
                        icon.Index = 100;
                        break;
                    case BuffType.FrostBite:
                        icon.Index = 221;
                        break;
                    case BuffType.ElementalSuperiority:
                        icon.Index = 93;
                        break;
                    case BuffType.BloodLust:
                        icon.Index = 90;
                        break;
                    case BuffType.Cloak:
                        icon.Index = 160;
                        break;
                    case BuffType.GhostWalk:
                        icon.Index = 160;
                        break;
                    case BuffType.Observable:
                        icon.Index = 172;
                        break;
                    case BuffType.TheNewBeginning:
                        icon.Index = 166;
                        break;
                    case BuffType.Veteran:
                        icon.Index = 171;
                        break;

                    case BuffType.Brown:
                        icon.Index = 229;
                        break;
                    case BuffType.PKPoint:
                        icon.Index = 266;
                        break;
                    case BuffType.Redemption:
                        icon.Index = 258;
                        break;
                    case BuffType.Renounce:
                        icon.Index = 94;
                        break;
                    case BuffType.Defiance:
                        icon.Index = 97;
                        break;
                    case BuffType.Might:
                        icon.Index = 96;
                        break;
                    case BuffType.ReflectDamage:
                        icon.Index = 98;
                        break;
                    case BuffType.Endurance:
                        icon.Index = 95;
                        break;
                    case BuffType.JudgementOfHeaven:
                        icon.Index = 99;
                        break;
                    case BuffType.StrengthOfFaith:
                        icon.Index = 141;
                        break;
                    case BuffType.CelestialLight:
                        icon.Index = 142;
                        break;
                    case BuffType.Transparency:
                        icon.Index = 160;
                        break;
                    case BuffType.LifeSteal:
                        icon.Index = 98;
                        break;
                    case BuffType.DarkConversion:
                        icon.Index = 166;
                        break;
                    case BuffType.DragonRepulse:
                        icon.Index = 165;
                        break;
                    case BuffType.Evasion:
                        icon.Index = 167;
                        break;
                    case BuffType.RagingWind:
                        icon.Index = 168;
                        break;
                    case BuffType.MagicWeakness:
                        icon.Index = 182;
                        break;
                    case BuffType.ItemBuff:
                        icon.Index = Globals.ItemInfoList.Binding.First(x => x.Index == buff.ItemIndex).BuffIcon;
                        break;
                    case BuffType.PvPCurse:
                        icon.Index = 241;
                        break;

                    case BuffType.ItemBuffPermanent:
                        icon.Index = 81;
                        break;
                    case BuffType.HuntGold:
                        icon.Index = 264;
                        break;
                    case BuffType.Companion:
                        icon.Index = 137;
                        break;
                    case BuffType.MapEffect:
                        icon.Index = 76;
                        break;
                    case BuffType.Guild:
                        icon.Index = 140;
                        break;
                    default:
                        icon.Index = 73;
                        break;
                }

                icon.ProcessAction = () =>
                {
                    if (MouseControl == icon)
                        icon.Hint = GetBuffHint(buff);
                };
            }

            for (int i = 0; i < buffs.Count; i++)
                Icons[buffs[i]].Location = new Point(3 + (i%6)*27, 3 + (i/6)*27);

            Size = new Size(3 + Math.Min(6, Math.Max(1, Icons.Count))*27, 3 + Math.Max(1, 1 +  (Icons.Count - 1)/6) * 27);
            
        }
        private string GetBuffHint(ClientBuffInfo buff)
        {
            string text = string.Empty;

            Stats stats = buff.Stats;

            switch (buff.Type)
            {
                case BuffType.Server:
                    text = $"服务器设置\n";
                    break;
                case BuffType.HuntGold:
                    text = $"赏金\n";
                    break;
                case BuffType.Observable:
                    text = $"观察者模式\n\n" +
                           $"你允许其他玩家观战.\n";
                    break;
                case BuffType.Veteran:
                    text = $"老兵模式\n";
                    break;
                case BuffType.Brown:
                    text = $"灰名\n";
                    break;
                case BuffType.PKPoint:
                    text = $"PK值\n";
                    break;
                case BuffType.Redemption:
                    text = $"Redemption Key Stone\n";
                    break;
                case BuffType.Castle:
                    text = $"沙巴克城主\n";
                    break;
                case BuffType.Guild:
                    text = $"行会模式\n";
                    break;
                case BuffType.MapEffect:
                    text = $"地图效果\n";
                    break;
                case BuffType.ItemBuff:
                    ItemInfo info = Globals.ItemInfoList.Binding.First(x => x.Index == buff.ItemIndex);
                    text = info.ItemName + "\n";
                    stats = info.Stats;
                    break;
                case BuffType.ItemBuffPermanent:
                    text = "永久Buffs\n";
                    break;
                case BuffType.Defiance:
                    text = $"铁布衫\n";
                    break;
                case BuffType.Might:
                    text = $"破血狂杀\n";
                    break;
                case BuffType.Endurance:
                    text = $"金刚之躯\n";
                    break;
                case BuffType.ReflectDamage:
                    text = $"移花接木\n";
                    break;
                case BuffType.Renounce:
                    text = $"凝血离魂\n";
                    break;
                case BuffType.MagicShield:
                    text = $"魔法盾\n";
                    break;
                case BuffType.FrostBite:
                    text = $"护身冰环\n";
                    break;
                case BuffType.JudgementOfHeaven:
                    text = $"天打雷劈\n";
                    break;
                case BuffType.Heal:
                    text = $"治愈术\n";
                    break;
                case BuffType.Invisibility:
                    text = $"隐身术\n";

                    text += $"隐藏在视线之内.\n";
                    break;
                case BuffType.MagicResistance:
                    text = $"幽灵盾\n";
                    break;
                case BuffType.Resilience:
                    text = $"神圣战甲术\n";
                    break;
                case BuffType.ElementalSuperiority:
                    text = $"强魔震法\n";
                    break;
                case BuffType.BloodLust:
                    text = $"猛虎强势\n";
                    break;
                case BuffType.StrengthOfFaith:
                    text = $"移花接木\n";
                    break;
                case BuffType.CelestialLight:
                    text = $"阴阳法环\n";
                    break;
                case BuffType.Transparency:
                    text = $"妙影无踪\n";
                    break;
                case BuffType.LifeSteal:
                    text = $"吸星大法\n";
                    break;
                case BuffType.PoisonousCloud:
                    text = $"鬼雾\n";
                    break;
                case BuffType.FullBloom:
                    text = $"泣血杜鹃\n";
                    break;
                case BuffType.WhiteLotus:
                    text = $"White Lotus\n";
                    break;
                case BuffType.RedLotus:
                    text = $"暗夜百合\n";
                    break;
                case BuffType.Cloak:
                    text = $"潜行\n";
                    break;
                case BuffType.GhostWalk:
                    text = $"鬼灵步\n\n" +
                           $"当你隐身时，你有机会提高100%的移动速度。.";
                    break;
                case BuffType.TheNewBeginning:
                    text = $"心念\n";
                    break;
                case BuffType.DarkConversion:
                    text = $"恶灵契约\n";
                    break;
                case BuffType.DragonRepulse:
                    text = $"星云锁链\n";
                    break;
                case BuffType.Evasion:
                    text = $"风之闪避\n";
                    break;
                case BuffType.RagingWind:
                    text = $"风之守护\n";
                    break;
                case BuffType.MagicWeakness:
                    text = $"魔法衰弱\n\n" +
                           $"你的魔防大幅度下降.\n";
                    break;
                case BuffType.Companion:
                    text = $"宠物\n";
                    break;
            }
            
            if (stats != null && stats.Count > 0)
            {
                foreach (KeyValuePair<Stat, int> pair in stats.Values)
                {
                    if (pair.Key == Stat.Duration) continue;

                    string temp = stats.GetDisplay(pair.Key);

                    if (temp == null) continue;
                    text += $"\n{temp}";
                }

                if (buff.RemainingTime != TimeSpan.MaxValue)
                    text += $"\n";
            }

            if (buff.RemainingTime != TimeSpan.MaxValue)
                text += $"\n持续时间: {Functions.ToString(buff.RemainingTime, true)}";

            if (buff.Pause) text += "\n暂停 (无效).";

            return text;
        }

        public override void Process()
        {
            base.Process();

            foreach (KeyValuePair<ClientBuffInfo, DXImageControl> pair in Icons)
            {
                if (pair.Key.Pause)
                {
                    pair.Value.ForeColour = Color.IndianRed;
                    continue;
                }
                    if (pair.Key.RemainingTime == TimeSpan.MaxValue) continue;

                if (pair.Key.RemainingTime.TotalSeconds >= 10)
                {
                    pair.Value.ForeColour = Color.White;
                    continue;
                }
                
                float rate = pair.Key.RemainingTime.Milliseconds / (float)1000;

                pair.Value.ForeColour = Functions.Lerp(Color.White, Color.CadetBlue, rate);
            }

            Hint = Icons.Count > 0 ? null : "Buff区域";


        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                foreach (KeyValuePair<ClientBuffInfo, DXImageControl> pair in Icons)
                {
                    if (pair.Value == null) continue;

                    if (pair.Value.IsDisposed) continue;

                    pair.Value.Dispose();
                }

                Icons.Clear();
                Icons = null;
            }

        }

        #endregion
    }

}
