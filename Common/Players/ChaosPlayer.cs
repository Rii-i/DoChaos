using System;                      // 为了 string.IsNullOrWhiteSpace
using System.Collections.Generic;  // 为了 List<T>

using DoChaos.Common.Configs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
 

namespace DoChaos.Common.Players
{
    public class ChaosPlayer : ModPlayer
    {
        private int chaosTimer;

        public override void PostUpdate()
        {
            if (!Player.active || Main.gamePaused)
                return;

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            var cfg = ChaosConfig.Instance;
            if (cfg == null || !cfg.EnableChaos)
                return;

            chaosTimer++;

            int intervalTicks = 60 * cfg.ChaosIntervalSeconds;
            if (intervalTicks <= 0)
                intervalTicks = 60 * 20; // 防止被设成 0 出问题

            if (chaosTimer >= intervalTicks)
            {
                chaosTimer = 0;
                DoChaos();
            }
        }


        private void DoChaos()
        {
            var cfg = ChaosConfig.Instance;
            if (cfg == null)
                return;

            // 根据配置选择目标玩家
            switch (cfg.TargetMode)
            {
                case ChaosConfig.ChaosTargetMode.SpecificPlayer:
                    Player targetSpecific = FindPlayerByName(cfg.TargetPlayerName);
                    if (targetSpecific != null)
                        ApplyChaosToPlayer(targetSpecific);
                    break;

                case ChaosConfig.ChaosTargetMode.RandomPlayer:
                    Player targetRandom = GetRandomActivePlayer();
                    if (targetRandom != null)
                        ApplyChaosToPlayer(targetRandom);
                    break;

                case ChaosConfig.ChaosTargetMode.AllPlayers:
                    ApplyToAllPlayers();
                    break;
            }
        }

        private void ApplyChaosToPlayer(Player target)
        {
            var cfg = ChaosConfig.Instance;
            if (cfg == null)
                return;

            IEntitySource source = target.GetSource_Misc("ChaosEffect");

            if (cfg.EnableGiveItem)
                GiveRandomItem(source, target);

            if (cfg.EnableSpawnNPC)
                SpawnRandomNPC(source, target);
        }

        private void ApplyToAllPlayers()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player p = Main.player[i];
                if (p != null && p.active)
                    ApplyChaosToPlayer(p);
            }
        }

        private Player FindPlayerByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player p = Main.player[i];
                if (p != null && p.active && p.name == name)
                    return p;
            }
            return null;
        }

        private Player GetRandomActivePlayer()
        {
            // 建立一个 active 玩家列表
            List<Player> activePlayers = new();

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player p = Main.player[i];
                if (p != null && p.active)
                    activePlayers.Add(p);
            }

            if (activePlayers.Count == 0)
                return null;

            return activePlayers[Main.rand.Next(activePlayers.Count)];
        }


        private void GiveRandomItem(IEntitySource source, Player target)
        {
            if (ItemLoader.ItemCount <= 1)
                return;

            int itemType = Main.rand.Next(1, ItemLoader.ItemCount);
            target.QuickSpawnItem(source, itemType);

            string itemName = Lang.GetItemNameValue(itemType);

            NetworkText text = NetworkText.FromKey(
                "Mods.DoChaos.Status.GetItem",
                target.name,
                itemName
            );

            ChatHelper.BroadcastChatMessage(
                text,
                new Color(255, 215, 0)
            );
        }


        private void SpawnRandomNPC(IEntitySource source, Player target)
        {
            if (NPCLoader.NPCCount <= 1)
                return;

            int npcType = Main.rand.Next(1, NPCLoader.NPCCount);

            Vector2 spawnPos = target.Center + new Vector2(
                Main.rand.Next(-400, 401),
                -200
            );

            int index = NPC.NewNPC(source, (int)spawnPos.X, (int)spawnPos.Y, npcType);
            if (index < 0 || index >= Main.npc.Length)
                return;

            NPC npc = Main.npc[index];
            npc.target = target.whoAmI;

            string npcName = Lang.GetNPCNameValue(npcType);

            NetworkText text = NetworkText.FromKey(
                "Mods.DoChaos.Status.NpcSpawn",
                target.name,
                npcName
            );

            ChatHelper.BroadcastChatMessage(
                text,
                new Color(150, 255, 150)
            );
        }
    }
}
