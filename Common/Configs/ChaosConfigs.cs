using System.ComponentModel;             // DefaultValueAttribute 在这
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;          // ModConfig 就在这里！




namespace DoChaos.Common.Configs
{
    public class ChaosConfig : ModConfig
    {
        public static ChaosConfig Instance { get; private set; }

        public override ConfigScope Mode => ConfigScope.ServerSide;

        public override void OnLoaded()
        {
            Instance = this;
        }

        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref NetworkText message)
        {
            // 单人模式随便改
            if (Main.netMode == NetmodeID.SinglePlayer)
                return true;

            // 多人时：只允许 0 号玩家（房主）改
            if (whoAmI == 0)
                return true;

            // 其他玩家改的话，拒绝并提示
            message = NetworkText.FromKey("Mods.DoChaos.Configs.ChaosConfig.OnlyHostCanChange");
            return false;
        }

        // ★ 枚举，不需要任何 Converter
        public enum ChaosTargetMode
        {
            [LabelKey("$Mods.DoChaos.Configs.ChaosConfig.ChaosTargetMode.SpecificPlayer")]
            SpecificPlayer,

            [LabelKey("$Mods.DoChaos.Configs.ChaosConfig.ChaosTargetMode.RandomPlayer")]
            RandomPlayer,

            [LabelKey("$Mods.DoChaos.Configs.ChaosConfig.ChaosTargetMode.AllPlayers")]
            AllPlayers
        }

        // =================
        // 总开关
        // =================

        [Header("$Mods.DoChaos.Configs.ChaosConfig.Headers.Main")]
        [DefaultValue(true)]
        public bool EnableChaos { get; set; }

        // =================
        // 功能开关
        // =================

        [Header("$Mods.DoChaos.Configs.ChaosConfig.Headers.Features")]
        [DefaultValue(true)]
        public bool EnableGiveItem { get; set; }

        [DefaultValue(true)]
        public bool EnableSpawnNPC { get; set; }

        // =================
        // 触发频率
        // =================

        [Header("$Mods.DoChaos.Configs.ChaosConfig.Headers.Frequency")]
        [DefaultValue(20)]
        [Range(5, 300)]
        public int ChaosIntervalSeconds { get; set; }

        // =================
        // 目标模式
        // =================

        [Header("$Mods.DoChaos.Configs.ChaosConfig.Headers.TargetMode")]
        [DefaultValue(ChaosTargetMode.RandomPlayer)]
        [LabelKey("$Mods.DoChaos.Configs.ChaosConfig.TargetMode.Label")]
        [TooltipKey("$Mods.DoChaos.Configs.ChaosConfig.TargetMode.Tooltip")]
        public ChaosTargetMode TargetMode { get; set; }

        [DefaultValue("")]
        [LabelKey("$Mods.DoChaos.Configs.ChaosConfig.TargetPlayerName.Label")]
        [TooltipKey("$Mods.DoChaos.Configs.ChaosConfig.TargetPlayerName.Tooltip")]
        public string TargetPlayerName { get; set; }
    }
}
