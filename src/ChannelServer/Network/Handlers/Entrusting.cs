﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Aura.Channel.Network.Sending;
using Aura.Channel.Network.Sending.Helpers;
using Aura.Channel.World;
using Aura.Channel.World.Entities;
using Aura.Channel.World.Entities.Props;
using Aura.Data;
using Aura.Mabi.Network;
using Aura.Shared.Network;
using Aura.Shared.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aura.Channel.Network.Handlers
{
	public partial class ChannelServerHandlers : PacketHandlerManager<ChannelClient>
	{
		/// <summary>
		/// Dummy handler for EntrustedEnchant.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="packet"></param>
		[PacketHandler(Op.EntrustedEnchant)]
		public void EntrustedEnchant(ChannelClient client, Packet packet)
		{
			var entityId = packet.GetLong();
			var unkByte = packet.GetByte();
			var unkLong = packet.GetLong();

			var creature = client.GetCreatureSafe(packet.Id);

			// Check feature
			if (!AuraData.FeaturesDb.IsEnabled("EnchantEntrust"))
			{
				Send.Notice(creature, Localization.Get("Requesting enchantments isn't possible yet."));
				Send.EntrustedEnchantR(creature, false);
				return;
			}

			// Check partner
			var partner = creature.Party.GetMember(entityId);
			if (partner == null)
			{
				Send.MsgBox(creature, Localization.Get("Player not found in party."));
				Send.EntrustedEnchantR(creature, false);
				return;
			}

			// No party members: "There is no party member available to ask for an Enchantment."
			// Member doesn't have rF+: "You cannot request the enchantment."

			Send.EntrustedEnchantR(creature, true, partner);
		}

		/// <summary>
		/// Sent when closing the entrustment window.
		/// </summary>
		/// <example>
		/// No parameters.
		/// </example>
		[PacketHandler(Op.EntrustedEnchantCancel)]
		public void EntrustedEnchantCancel(ChannelClient client, Packet packet)
		{
			var creature = client.GetCreatureSafe(packet.Id);

			// check active...

			Send.EntrustedEnchantClose(creature);
		}
	}
}