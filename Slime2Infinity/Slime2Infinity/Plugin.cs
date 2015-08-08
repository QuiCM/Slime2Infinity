using System;
using System.IO;
using System.IO.Streams;

using Terraria;
using Terraria.ID;
using TerrariaApi.Server;

namespace Slime2Infinity
{
	[ApiVersion(1, 21)]
	public class Plugin : TerrariaPlugin
	{
		public override string Author
		{
			get
			{
				return "White";
			}
		}

		public override string Description
		{
			get
			{
				return "Reintroduces the bug where hitting the King Slime spawns blue slimes to mob cap";
			}
		}

		public override string Name
		{
			get
			{
				return "Limo Infinitum";
			}
		}

		public Plugin(Main game)
			:base(game)
		{

		}

		public override void Initialize()
		{
			ServerApi.Hooks.NetGetData.Register(this, OnGetData);
		}

		private void OnGetData(GetDataEventArgs e)
		{
			if (e.MsgID != PacketTypes.NpcStrike)
			{
				return;
			}

			using (MemoryStream data = new MemoryStream(e.Msg.readBuffer, e.Index, e.Length - 1))
			{
				var id = data.ReadInt16();
				var dmg = data.ReadInt16();
				data.ReadSingle();
				data.ReadByte();
				data.ReadByte();

				if (id < 0 || id > Main.maxNPCs)
				{
					return;
				}

				NPC npc = Main.npc[id];
				if (npc == null)
				{
					return;
				}

				if (npc.type != NPCID.KingSlime || !npc.active)
				{
					return;
				}
				if (dmg <= 0)
				{
					return;
				}

				if (Main.rand == null)
				{
					Main.rand = new Random((int)DateTime.Now.Ticks);
				}

				for (int i = 0; i < 20; i++)
				{
					int amt = 4 + Main.rand.Next(1, 5);

					for (int j = 0; j < amt; j++)
					{
						int x = (int)npc.position.X + Main.rand.Next(-80, 81);
						int y = (int)npc.position.Y + Main.rand.Next(-80, 81);
						NPC.NewNPC(x, y, NPCID.BlueSlime, 0);
					}
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
			}

			base.Dispose(disposing);
		}
	}
}
