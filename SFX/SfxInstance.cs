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

	/// <summary>
	/// 
	/// </summary>
	public partial class SfxInstance {
		
		bool looped;

		static protected Random rand = new Random();

		protected readonly SfxSystem sfxSystem;
		protected readonly RenderWorld rw;
		protected readonly SoundWorld sw;

		public delegate void EmitFunction ( ref Particle p, Vector3 pos );

		readonly List<Stage> stages = new List<Stage>();

		protected FXEvent	fxEvent;


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sfxSystem"></param>
		/// <param name="fxEvent"></param>
		public SfxInstance( SfxSystem sfxSystem, FXEvent fxEvent )
		{
			this.sfxSystem	=	sfxSystem;
			this.rw			=	sfxSystem.rw;
			this.sw			=	sfxSystem.sw;
			this.looped		=	false;
			this.fxEvent	=	fxEvent;
		}



		/// <summary>
		/// Immediatly removes all sfx instance stuff, 
		/// like light sources and sounds.
		/// Check IsExhausted before calling Kill to produce smooth animation.
		/// </summary>
		public void Kill ()
		{
			KillLight();
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="elapsedTime"></param>
		public void Update ( float dt )
		{
			foreach ( var stage in stages ) {
				stage.Update( dt, fxEvent );
			}

			UpdateLight( dt );
		}


		/// <summary>
		/// 
		/// </summary>
		public bool IsExhausted {
			get {
				
				bool isStopped = true;

				//	check stages :
				foreach ( var stage in stages ) {
					isStopped &= stage.Stopped;
				}

				//	check lights :
				isStopped &= IsLightExhausted;

				return isStopped;
			}
		}


		/*-----------------------------------------------------------------------------------------
		 * 
		 *	Particle helper functions
		 * 
		-----------------------------------------------------------------------------------------*/

		/// <summary>
		/// 
		/// </summary>
		/// <param name="spriteName"></param>
		/// <param name="delay"></param>
		/// <param name="period"></param>
		/// <param name="sleep"></param>
		/// <param name="count"></param>
		/// <param name="emit"></param>
		protected void AddStage ( string spriteName, float delay, float period, float sleep, int count, EmitFunction emit )
		{
			var spriteIndex		=	sfxSystem.GetSpriteIndex( spriteName );
			var stage			=	new Stage( this, spriteIndex, delay, period, sleep, count, emit );
			stages.Add( stage );
		}





		protected static void SetupMotion ( ref Particle p, Vector3 origin, Vector3 velocity, Vector3 accel )
		{
			p.Position		=	origin;
			p.Velocity		=	velocity;
			p.Acceleration	=	accel;
		}


		protected void	SetupTiming	( ref Particle p, float totalTime, float fadeIn, float fadeOut ) 
		{
			p.LifeTime		=	totalTime;
			p.FadeIn		=	fadeIn;
			p.FadeOut		=	fadeOut;
		}

		
		protected void	SetupSize ( ref Particle p, float size0, float size1 ) 
		{
			p.Size0	=	size0;
			p.Size1	=	size1;
		}


		protected void	SetupColor ( ref Particle p, Color4 color0, Color4 color1 ) 
		{
			p.Color0		=	color0;
			p.Color1		=	color1;
		}


		protected void	SetupColor ( ref Particle p, float intensity, float alpha0, float alpha1 ) 
		{
			p.Color0		=	new Color4( intensity, intensity, intensity, alpha0 );
			p.Color1		=	new Color4( intensity, intensity, intensity, alpha1 );
		}


		protected void	SetupAngles	( ref Particle p, float angularLength )
		{
			p.Rotation0	=	rand.NextFloat(0,1) * MathUtil.TwoPi;
			p.Rotation1	=	p.Rotation0 + MathUtil.DegreesToRadians(angularLength) * rand.NextFloat(0,1)>0.5 ? 1 : -1;
		} 

		protected void	SetupAnglesAbs	( ref Particle p, float angularLength )
		{
			p.Rotation0	=	rand.NextFloat(0,1) * MathUtil.TwoPi;
			p.Rotation1	=	p.Rotation0 + angularLength;
		} 

	}
}
