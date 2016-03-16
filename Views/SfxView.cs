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
using ShooterDemo.SFX;


namespace ShooterDemo.Views {
	public class SfxView : EntityView<SfxInstance> {


		/// <summary>
		/// 
		/// </summary>
		/// <param name="game"></param>
		public SfxView ( World world ) : base(world)
		{
		}



		/// <summary>
		/// Hot reload?
		/// </summary>
		/// <param name="scenePath"></param>
		/// <param name="nodeName"></param>
		/// <param name="preTransform"></param>
		public void AttachSFX ( Entity entity, string sfxName )
		{
			var fxID	=	World.GameClient.Atoms[ sfxName ];
			var	fxEvent	=	new FXEvent(fxID, entity.ID, entity.Position, entity.LinearVelocity, entity.Rotation);
			
			AddObject( entity.ID, World.RunFX( fxEvent ) ); 
		} 



		/// <summary>
		/// Updates visible meshes
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update ( float elapsedTime, float lerpFactor )
		{
			IterateObjects( (e,sfx) => {
				var p	=	e.LerpPosition( lerpFactor );
				var q   =	e.LerpRotation( lerpFactor );
				var v	=	e.LinearVelocity;
				sfx.Move( p,v,q );
				//Game.RenderSystem.RenderWorld.Debug.Trace( e.Position, 0.5f, Color.Yellow );
			});
		}



		/// <summary>
		/// Removes entity
		/// </summary>
		/// <param name="id"></param>
		public override void Kill ( uint id )
		{	
			SfxInstance sfx;

			if ( RemoveObject( id, out sfx ) ) {
				sfx.Kill();
			}
		}

	}
}
