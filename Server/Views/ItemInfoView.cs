using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using DevExpress.XtraBars;
using Library;
using Library.SystemModels;
using Server.Envir;

namespace Server.Views
{
    public partial class ItemInfoView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public ItemInfoView()
        {
            InitializeComponent();

            ItemInfoGridControl.DataSource = SMain.Session.GetCollection<ItemInfo>().Binding;
            MonsterLookUpEdit.DataSource = SMain.Session.GetCollection<MonsterInfo>().Binding;
            SetLookUpEdit.DataSource = SMain.Session.GetCollection<SetInfo>().Binding;

            ItemTypeImageComboBox.Items.AddEnum<ItemType>();
            RequiredClassImageComboBox.Items.AddEnum<RequiredClass>();
            RequiredGenderImageComboBox.Items.AddEnum<RequiredGender>();
            StatImageComboBox.Items.AddEnum<Stat>();
            RequiredTypeImageComboBox.Items.AddEnum<RequiredType>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(ItemInfoGridView);
            SMain.SetUpView(ItemStatsGridView);
            SMain.SetUpView(DropsGridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            List<string> itemData = new List<string>();

            itemData.Add(GetLine(null));
            foreach (ItemInfo item in SMain.Session.GetCollection<ItemInfo>().Binding)
                itemData.Add(GetLine(item));

            string path = @"D:\Zircon Server\Data Works\Exports\";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            File.WriteAllLines(path + "Items.csv", itemData);
        }

        public string GetLine(ItemInfo info)
        {
            StringBuilder builder = new StringBuilder();

            
            builder.Append((info?.ItemName ?? "名字") + ", ");
            builder.Append((info?.ItemType.ToString() ?? "类型") + ", ");

            if (info == null)
            {
                builder.Append("需求职业");
            }
            else
            {
                Type type = info.RequiredClass.GetType();

                MemberInfo[] infos = type.GetMember(info.RequiredClass.ToString());

                DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();

                builder.Append(description?.Description.Replace(",", "") ?? info.RequiredClass.ToString());
            }

            builder.Append(", ");

            builder.Append((info?.RequiredType.ToString() ?? "需求类别") + ", ");
            builder.Append((info?.RequiredAmount.ToString() ?? "需求护甲") + ", ");
            builder.Append((info?.Image.ToString("00000") ?? "图像") + ", ");
            builder.Append((info?.Weight.ToString() ?? "重量") + ", ");
            builder.Append((info?.Durability.ToString() ?? "耐久") + ", ");
            builder.Append((info?.Rarity.ToString() ?? "稀有性") + ", ");

            builder.Append((info == null ? "防御" : string.Format("{0}-{1}", info.Stats[Stat.MinAC], info.Stats[Stat.MaxAC])) + ", ");
            builder.Append((info == null ? "魔防" : string.Format("{0}-{1}", info.Stats[Stat.MinMR], info.Stats[Stat.MaxMR])) + ", ");
            builder.Append((info == null ? "攻击" : string.Format("{0}-{1}", info.Stats[Stat.MinDC], info.Stats[Stat.MaxDC])) + ", ");
            builder.Append((info == null ? "魔法" : string.Format("{0}-{1}", info.Stats[Stat.MinMC], info.Stats[Stat.MaxMC])) + ", ");
            builder.Append((info == null ? "道术" : string.Format("{0}-{1}", info.Stats[Stat.MinSC], info.Stats[Stat.MaxSC])) + ", ");
            builder.Append((info == null ? "准确" : string.Format("+{0}", info.Stats[Stat.Accuracy])) + ", ");
            builder.Append((info == null ? "敏捷" : string.Format("+{0}", info.Stats[Stat.Agility])) + ", ");
            builder.Append((info == null ? "攻击速度" : string.Format("+{0}", info.Stats[Stat.AttackSpeed])) + ", ");
            builder.Append((info == null ? "生命值" : string.Format("+{0}", info.Stats[Stat.Health])) + ", ");
            builder.Append((info == null ? "魔法值" : string.Format("+{0}", info.Stats[Stat.Mana])) + ", ");
            builder.Append((info == null ? "幸运" : string.Format("+{0}", info.Stats[Stat.Luck])) + ", ");
            builder.Append((info == null ? "经验倍率" : string.Format("+{0}", info.Stats[Stat.ExperienceRate])) + ", ");
            builder.Append((info == null ? "掉落倍率" : string.Format("+{0}", info.Stats[Stat.DropRate])) + ", ");
            builder.Append((info == null ? "金币倍率" : string.Format("+{0}", info.Stats[Stat.GoldRate])) + ", ");
            builder.Append((info == null ? "技能倍率" : string.Format("+{0}", info.Stats[Stat.SkillRate])) + ", ");
            builder.Append((info == null ? "耐久" : string.Format("+{0}", info.Stats[Stat.Duration])) + ", ");
            builder.Append((info == null ? "背包重量" : string.Format("+{0}", info.Stats[Stat.BagWeight])) + ", ");
            builder.Append((info == null ? "穿着负重" : string.Format("+{0}", info.Stats[Stat.WearWeight])) + ", ");
            builder.Append((info == null ? "手持负重" : string.Format("+{0}", info.Stats[Stat.HandWeight])) + ", ");
            builder.Append((info == null ? "生命窃取" : string.Format("+{0}", info.Stats[Stat.LifeSteal])) + ", ");
            builder.Append((info == null ? "火元素伤害" : string.Format("+{0}", info.Stats[Stat.FireAttack])) + ", ");
            builder.Append((info == null ? "冰元素伤害" : string.Format("+{0}", info.Stats[Stat.IceAttack])) + ", ");
            builder.Append((info == null ? "电元素伤害" : string.Format("+{0}", info.Stats[Stat.LightningAttack])) + ", ");
            builder.Append((info == null ? "风元素伤害" : string.Format("+{0}", info.Stats[Stat.WindAttack])) + ", ");
            builder.Append((info == null ? "神圣元素伤害" : string.Format("+{0}", info.Stats[Stat.HolyAttack])) + ", ");
            builder.Append((info == null ? "黑暗元素伤害" : string.Format("+{0}", info.Stats[Stat.DarkAttack])) + ", ");
            builder.Append((info == null ? "幻影元素伤害" : string.Format("+{0}", info.Stats[Stat.PhantomAttack])) + ", ");


            return builder.ToString();
        }

    }
}