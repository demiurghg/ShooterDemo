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
		/// Gets entities.
		/// </summary>
		public EntityCollection Entities {
			get { return entities; }
		}
		EntityCollection entities;


		Random	rand = new Random();


		/// <summary>
		/// Gets world's info.
		/// </summary>
		public string Info { get; private set; }


		/// <summary>
		/// Creates world
		/// </summary>
		/// <param name="map"></param>
		public GameWorld ( GameServer server, string map )
		{
			entities	=	new EntityCollection(this);
			Info		=	@"scenes\" + map;

			InitPhysSpace( 9.8f );

			var scene	=	server.Content.Load<Scene>( Info );

			ReadMapFromScene( server.Content, scene, false );
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="client"></param>
		/// <param name="serverInfo"></param>
		public GameWorld( GameClient client, string serverInfo )
		{
			entities	=	new EntityCollection(this);

			InitPhysSpace( 9.8f );

			var scene	=	client.Content.Load<Scene>( serverInfo );

			ReadMapFromScene( client.Content, scene, true );
		}



		/// <summary>
		/// Kills all entities.
		/// </summary>
		public void KillAll ()
		{
			foreach ( var ent in entities ) {
				ent.Deactivate(this);
			}
		}



		/// <summary>
		/// Update physics
		/// </summary>
		/// <param name="gameTime"></param>
		public void Simulate ( GameTime gameTime )
		{
			physSpace.Update( gameTime.ElapsedSec );
		} 



		/// <summary>
		/// Update entities' logic
		/// </summary>
		/// <param name="gameTime"></param>
		public void Think ( GameTime gameTime )
		{
			//	get entity array :
			var ents = new Entity[ entities.Count ];
			entities.CopyTo( ents, 0 );

			//	update entities :
			foreach ( var ent in entities ) {
				ent.Update( gameTime );
			}
		}



		/// <summary>
		/// Draw
		/// </summary>
		/// <param name="gameTime"></param>
		public void Present	( GameTime gameTime )
		{
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="userGuid"></param>
		/// <param name="command"></param>
		public void Command ( Guid userGuid, byte[] command )
		{
		}
		
	}
}
