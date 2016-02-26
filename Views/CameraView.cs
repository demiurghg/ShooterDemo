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
using BEPUphysics;
using BEPUphysics.Character;


namespace ShooterDemo.Views {
	public class CameraView : EntityView<object> {

		/// <summary>
		/// 
		/// </summary>
		/// <param name="game"></param>
		/// <param name="space"></param>
		public CameraView ( World world ) : base(world)
		{
		}


		Vector3 filteredPos = Vector3.Zero;


		/// <summary>
		/// 
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update ( float elapsedTime )
		{
			var rw = Game.RenderSystem.RenderWorld;
			var vp = Game.RenderSystem.DisplayBounds;

			var aspect	=	(vp.Width) / (float)vp.Height;
		
			rw.Camera.SetupCameraFov( new Vector3(10,4,10), new Vector3(0,4,0), Vector3.Up, MathUtil.Rad(90), 0.125f, 1024f, 1, 0, aspect );

			var player	=	World.GetEntityOrNull( e => e.Is("player") && e.UserGuid == World.UserGuid );

			if (player==null) {
				return;
			}


			var uc	=	(World.GameClient as ShooterClient).UserCommand;

			var m	= 	Matrix.RotationYawPitchRoll( uc.Yaw, uc.Pitch, uc.Roll );

			var ppos	=	player.Position;
			//ppos.Y = 1;
			var pos		=	ppos + Vector3.Up * 1.0f + m.Backward * 0;

			/*filteredPos = Vector3.Lerp( filteredPos, pos, 0.3f );
			pos = filteredPos;*/

			var fwd	=	pos + m.Forward;
			var up	=	m.Up;

			rw.Camera.SetupCameraFov( pos, fwd, up, MathUtil.Rad(90), 0.125f, 1024f, 1, 0, aspect );

		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		public override void Kill ( uint id )
		{
			/*object controller;
			
			if ( RemoveObject( id, out controller ) ) {
				space.Remove( controller );
			} */
		}
	}
}
