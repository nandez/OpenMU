﻿// <copyright file="WarpGateHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.Pathfinding;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for warp gate packets.
    /// This one is called when a player has entered a gate area, and sends a gate enter request.
    /// </summary>
    [PlugIn("WarpGateHandlerPlugIn", "Handler for warp gate packets.")]
    [Guid("d8f56da4-774b-42af-96ac-12a10ea0187b")]
    internal class WarpGateHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly WarpGateAction warpAction = new WarpGateAction();

        private readonly WizardTeleportAction teleportAction = new WizardTeleportAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => EnterGateRequest.Code;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            if (packet.Length < EnterGateRequest.Length)
            {
                return;
            }

            EnterGateRequest request = packet;
            var gateNumber = request.GateNumber;

            if (gateNumber == 0)
            {
                this.teleportAction.TryTeleportWithSkill(player, new Point(request.TeleportTargetX, request.TeleportTargetY));
                return;
            }

            var gate = player.SelectedCharacter?.CurrentMap?.EnterGates.FirstOrDefault(g => g.Number == gateNumber);
            if (gate is null)
            {
                player.Logger.LogWarning("Gate {0} not found in current map {1}", gateNumber,  player.SelectedCharacter?.CurrentMap);
                return;
            }

            this.warpAction.EnterGate(player, gate);
        }
    }
}
