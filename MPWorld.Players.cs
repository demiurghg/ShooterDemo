using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Fusion;
using Fusion.Core;
using Fusion.Core.Content;
using Fusion.Core.Mathematics;
using Fusion.Engine.Common;
using Fusion.Engine.Input;
using Fusion.Engine.Client;
using Fusion.Engine.Server;
using Fusion.Engine.Graphics;
using ShooterDemo.Core;
using ShooterDemo.Views;
using ShooterDemo.Controllers;

namespace ShooterDemo {
	public partial class MPWorld : World {

		Random rand = new Random();


		public override void PlayerCommand ( Guid guid, byte[] command )
		{
			var userCmd =	UserCommand.FromBytes( command );

			var player	=	GetEntityOrNull( e => e.Is("player") && e.UserGuid == guid );

			if (player==null) {
				return;
			}

			GetController<Characters>().Move( player.ID, userCmd );
		}


		/// <summary>
		/// Called internally when player entered.
		/// </summary>
		/// <param name="guid"></param>
		public override void PlayerEntered ( Guid guid )
		{
			Log.Verbose("player entered: {0}", guid );

			var sp = GetEntities("startPoint").OrderBy( e => rand.Next() ).FirstOrDefault();

			if (sp==null) {
				throw new GameException("No starting points found");
			}

			var ent = Spawn( "player", 0, sp.Position, sp.Angles );

			ent.UserGuid = guid;
		}



		/// <summary>
		/// Called internally when player left.
		/// </summary>
		/// <param name="guid"></param>
		public override void PlayerLeft ( Guid guid )
		{
			Log.Verbose("player left: {0}", guid );

			var ent = GetEntityOrNull( e => e.UserGuid == guid );

			if (ent!=null) {
				Kill( ent.ID );
			}
		}

	}
}
