using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion;
using Fusion.Core;
using Fusion.Core.Mathematics;
using Fusion.Engine.Graphics;
using Fusion.Engine.Common;
using Fusion.Engine.Server;
using Fusion.Engine.Client;
using BEPUVector3 = BEPUutilities.Vector3;
using BEPUTransform = BEPUutilities.AffineTransform;
using ShooterDemo.Entities;

namespace ShooterDemo {
	public partial class GameWorld {

		/// <summary>
		/// 
		/// </summary>
		/// <param name="guid"></param>
		public void PlayerEntered ( Guid guid )
		{
			var sp = entities
						.Where( e1 => e1 is StartPoint )
						.Select( e2 => e2 as StartPoint )
						.OrderBy( e3 => Guid.NewGuid() )
						.FirstOrDefault();

			if (sp==null) {
				throw new GameException("No start points");
			}
									

			var p = new Player( guid, sp.World );

			entities.Add( p );
		}



		public Player GetPlayer ( Guid guid )
		{
			return entities.FirstOrDefault( e => (e is Player) && (((Player)e).UserGuid==guid) ) as Player;
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="guid"></param>
		public void PlayerLeft ( Guid guid )
		{
			var player = GetPlayer(guid);

			if (player!=null) {
				player.Deactivate(this);
			}

			entities.Remove( player );
		}

		
	}
}
