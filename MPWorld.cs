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

		
		readonly string mapName;

		
		public MPWorld( GameServer server, string map ) : base(server)
		{
			InitPhysSpace(9.8f);
			this.mapName	=	map;

			AddController( new Characters(this, PhysSpace) );

			var scene = Content.Load<Scene>(@"scenes\" + map );

			ReadMapFromScene( Content, scene, IsClient );
		}



		public MPWorld( GameClient client, string serverInfo ) : base(client)
		{
			InitPhysSpace(9.8f);

			AddView( new ModelView(client, this) );

			var scene = Content.Load<Scene>(@"scenes\" + serverInfo );

			ReadMapFromScene( Content, scene, IsClient );
		}



		public override void FinalizeLoad ()
		{
			AddMeshInstances();
		}



		public override void Cleanup ()
		{
			Game.RenderSystem.RenderWorld.ClearWorld();
		}


		public override string ServerInfo ()
		{
			return mapName;
		}



		public override void PlayerEntered ( Guid guid )
		{
			throw new NotImplementedException();
		}



		public override void PlayerLeft ( Guid guid )
		{
			throw new NotImplementedException();
		}



		public override void Write ( BinaryWriter writer )
		{
			base.Write( writer );
		}



		public override void Read ( BinaryReader reader )
		{
			base.Read( reader );
		}
	}
}
