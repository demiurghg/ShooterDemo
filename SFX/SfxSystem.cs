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
using Fusion.Engine.Audio;


namespace ShooterDemo.SFX {
	public class SfxSystem {

		TextureAtlas spriteSheet;

		readonly Game			game;
		readonly ShooterClient	client;
		public readonly RenderWorld	rw;
		public readonly SoundWorld	sw;

		Dictionary<FXEventType, Type> fxDictionary;
		List<SfxInstance> runningSFXes = new List<SfxInstance>();

		float timeAccumulator = 0;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="game"></param>
		public SfxSystem ( ShooterClient client )
		{
			this.client	=	client;
			this.game	=	client.Game;
			this.rw		=	game.RenderSystem.RenderWorld;
			this.sw		=	game.SoundSystem.SoundWorld;

			fxDictionary	=	Misc.GetAllClassesWithAttribute<SfxAttribute>()
									.ToDictionary( t1 => t1.GetCustomAttribute<SfxAttribute>().FXType );

			Game_Reloading(this, EventArgs.Empty);
			game.Reloading +=	Game_Reloading;
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void Game_Reloading ( object sender, EventArgs e )
		{
			spriteSheet	=  client.Content.Load<TextureAtlas>(@"sprites\particles");

			rw.ParticleSystem.Images	=	spriteSheet;	
		}


		/// <summary>
		/// 
		/// </summary>
		public void StopAllSFX ()
		{
			rw.ParticleSystem.Images	=	null;
			runningSFXes.Clear();
		}



		/// <summary>
		/// Updates visible meshes
		/// </summary>
		/// <param name="gameTime"></param>
		public void Update ( float elapsedTime )
		{
			const float dt = 1/60.0f;
			timeAccumulator	+= elapsedTime;

			while ( timeAccumulator > dt ) {

				runningSFXes.RemoveAll( sfx => sfx.IsExhausted );

				foreach ( var sfx in runningSFXes ) {
					sfx.Update( dt );
				}

				timeAccumulator -= dt;				
			}
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="imageName"></param>
		/// <returns></returns>
		public int GetSpriteIndex ( string spriteName )
		{
			return spriteSheet.IndexOf( spriteName );
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="fxEvent"></param>
		public void RunFX ( FXEvent fxEvent )
		{
			Type type;

			if (fxDictionary.TryGetValue( fxEvent.FXType, out type )) {
				
				var sfx = (SfxInstance)Activator.CreateInstance( type, this, fxEvent );
				runningSFXes.Add( sfx );

			} else {
				Log.Warning("Unhandled FX event: {0}", fxEvent.FXType );
			}
		}
	}
}
