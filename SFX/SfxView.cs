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


namespace ShooterDemo.SFX {
	public class SfxView : EntityView<BaseSfx> {



		/// <summary>
		/// 
		/// </summary>
		/// <param name="game"></param>
		public SfxView ( World world ) : base(world)
		{
			Game.Reloading += Game_Reloading;
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void Game_Reloading ( object sender, EventArgs e )
		{
			//IterateObjects( (ent,m) => m.Reload(Game.RenderSystem, World.Content) );
		}



		/// <summary>
		/// Updates visible meshes
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update ( float elapsedTime, float lerpFactor )
		{
			IterateObjects( (e,sfx) => {
				//m.Instance.World	=	m.PreTransform * e.GetWorldMatrix(lerpFactor) * m.PostTransform;
				//m.Instance.Visible	=	e.UserGuid != World.GameClient.Guid;
			});
		}



		/// <summary>
		/// Removes entity
		/// </summary>
		/// <param name="id"></param>
		public override void Kill ( uint id )
		{	
			BaseSfx sfx;

			if ( RemoveObject( id, out sfx ) ) {
				//Game.RenderSystem.RenderWorld.Instances.Remove( model.Instance );
			}
		}

	}
}
