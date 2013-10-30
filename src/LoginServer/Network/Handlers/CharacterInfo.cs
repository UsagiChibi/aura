﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see licence file in the main folder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aura.Shared.Network;
using Aura.Login.Database;

namespace Aura.Login.Network.Handlers
{
	public partial class LoginServerHandlers : PacketHandlerManager<LoginClient>
	{
		/// <summary>
		/// Character information request, if client doesn't have it cached.
		/// </summary>
		/// <remarks>
		/// Handles characters, pets, and partners. The results are pretty
		/// much the same, aside from the op.
		/// </remarks>
		/// <example>
		/// 0001 [................] String : mabius1
		/// 0002 [0010000000000000] Long   : ...
		/// </example>
		[PacketHandler(Op.CharacterInfoRequest, Op.PetInfoRequest)]
		public void CharacterInfoRequest(LoginClient client, MabiPacket packet)
		{
			var server = packet.GetString();
			var characterId = packet.GetLong();

			var op = packet.Op++;

			var character = (packet.Op == Op.CharacterInfoRequest ? client.Account.GetCharacter(characterId) : client.Account.GetPet(characterId));
			if (character == null)
			{
				Send.CharacterInfoRequestR_Fail(client, op);
				return;
			}

			var items = LoginDb.Instance.GetEquipment(character.CreatureId);

			Send.CharacterInfoRequestR(client, packet.Op, character, items);
		}
	}
}
