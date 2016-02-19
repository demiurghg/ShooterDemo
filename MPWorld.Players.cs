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


		/// <summary>
		/// Called internally when player entered.
		/// </summary>
		/// <param name="guid"></param>
		void PlayerEnteredInternal ( Guid guid )
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
		void PlayerLeftInternal ( Guid guid )
		{
			Log.Verbose("player left: {0}", guid );

			var ent = GetEntityOrNull( e => e.UserGuid == guid );

			if (ent!=null) {
				Kill( ent.ID );
			}
		}

	}
}
