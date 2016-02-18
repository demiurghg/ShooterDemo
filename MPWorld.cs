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

namespace ShooterDemo {
	public partial class MPWorld : World {

		
		readonly string mapName;

		
		/// <summary>
		/// 
		/// </summary>
		public MPWorld ( Game game, ContentManager content, bool server, string map ) : base(game)
		{
			this.mapName	=	map;

			var scene	=	content.Load<Scene>(@"scenes\" + mapName);

			InitPhysSpace(9.8f);

			ReadMapFromScene( content, scene, !server );
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
