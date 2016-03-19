using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion;
using Fusion.Core;
using Fusion.Core.Content;
using Fusion.Core.Mathematics;
using ShooterDemo;
using ShooterDemo.Core;
using Fusion.Engine.Graphics;

namespace ShooterDemo.SFX.WeaponFX {
	class RailHit : SfxInstance {

		Vector3 sparkDir;
		
		public RailHit ( SfxSystem sfxSystem, FXEvent fxEvent ) : base(sfxSystem, fxEvent)
		{
			sparkDir = Matrix.RotationQuaternion(fxEvent.Rotation).Forward;

			AddParticleStage("railSpark", 0, 0f, 0.1f, 100, false, EmitSpark );
			AddParticleStage("railPuff" , 0, 0f, 0.1f,  30, false, EmitPuff );
																					  
			AddLightStage( fxEvent.Origin + sparkDir * 0.1f	, new Color4(135,135,239,1), 1.0f, 100f, 3f );

			AddSoundStage( @"sound\weapon\railshot",	fxEvent.Origin, 1, false );
		}



		void EmitSpark ( ref Particle p, FXEvent fxEvent )
		{
			var vel	=	sparkDir * rand.GaussDistribution(3,1) + rand.GaussRadialDistribution(0, 0.5f);
			var pos	=	fxEvent.Origin;

			SetupMotion		( ref p, pos, vel, Vector3.Zero, 0, 0.2f );
			SetupAngles		( ref p, 160 );
			SetupColor		( ref p, 5000, 5000, 0, 1 );
			SetupTiming		( ref p, 0.5f, 0.01f, 0.9f );
			SetupSize		( ref p, 0.1f, 0.00f );
		}



		void EmitPuff ( ref Particle p, FXEvent fxEvent )
		{
			var vel	=	rand.GaussRadialDistribution(0, 0.7f);
			var pos	=	fxEvent.Origin;

			SetupMotion		( ref p, pos, vel, Vector3.Zero, 0, 0.0f );
			SetupAngles		( ref p, 10 );
			SetupColor		( ref p, 0, 5000, 0, 1 );
			SetupTiming		( ref p, 0.5f, 0.01f, 0.1f );
			SetupSize		( ref p, 0.2f, 0.7f );
		}
	}



	class RailMuzzle : SfxInstance {

		Vector3 sparkDir;
		
		public RailMuzzle ( SfxSystem sfxSystem, FXEvent fxEvent ) : base(sfxSystem, fxEvent)
		{
			ShakeCamera( rand.GaussDistribution(0,20), rand.GaussDistribution(0,20), rand.GaussDistribution(0,20) );


			sparkDir = Matrix.RotationQuaternion(fxEvent.Rotation).Forward;
			//AddParticleStage("bulletSpark", 0, 0f, 0.1f, 30, EmitSpark );
																					  
			AddLightStage( fxEvent.Origin, new Color4(135,135,239,1), 2.0f, 100f, 3f );

			AddSoundStage( @"sound\weapon\railshot",	fxEvent.Origin, 1, false );
		}
	}



	class RailTrail : SfxInstance {

		Vector3 sparkDir;


		float sin ( float a ) { return (float)Math.Sin(a*6.28f); }
		float cos ( float a ) { return (float)Math.Cos(a*6.28f); }

		
		public RailTrail ( SfxSystem sfxSystem, FXEvent fxEvent ) : base(sfxSystem, fxEvent)
		{
			var p = new Particle();

			p.TimeLag		=	0;
			p.ImageIndex	=	sfxSystem.GetSpriteIndex("railSpark");

			var m = Matrix.RotationQuaternion( fxEvent.Rotation );
			var up = m.Up;
			var rt = m.Right;

			int count = Math.Min((int)(fxEvent.Velocity.Length() * 10), 300);

			for (int i=0; i<count; i++) {

				var t		=	i / 50.0f;
				var pos		=	fxEvent.Origin + fxEvent.Velocity * (i+0)/(float)count;
				var pos1	=	fxEvent.Origin + fxEvent.Velocity * (i+1)/(float)count;
				var vel		=	rand.GaussRadialDistribution(0,0.05f);
			
				SetupMotion	( ref p, pos, vel, Vector3.Zero, 0, 0 );
				SetupColor	( ref p, 0, 1000, 0, 1 );
				SetupTiming	( ref p, 1, 0.5f, 0.5f );
				SetupSize	( ref p, 0.03f, 0.0f );
				SetupAngles	( ref p, 160 );

				p.TailPosition	=	pos1;
 
				//SfxInstance.rw.Debug.Trace( p.Position, 0.2f, Color.Yellow );
				rw.ParticleSystem.InjectParticle( p );
			}



			p.ImageIndex	=	sfxSystem.GetSpriteIndex("railTrace");
			count = 1;//Math.Max(5, Math.Min((int)(fxEvent.Velocity.Length()/4), 16));

			for (int i=0; i<3; i++) {	 

				var pos		=	fxEvent.Origin;
				var pos1	=	fxEvent.Origin + fxEvent.Velocity;
				
				SetupMotion	( ref p, pos, Vector3.Zero, Vector3.Zero, 0, 0 );
				SetupColor	( ref p, 0, 2000, 0, 1 );
				SetupTiming	( ref p, rand.GaussDistribution(0.5f,0.1f), 0.01f, 0.9f );
				SetupSize	( ref p, rand.GaussDistribution(0.3f,0.1f), 0 );
				SetupAngles	( ref p, 160 );

				p.TailPosition	=	pos1;
				p.Effects	|=	ParticleFX.Beam;
 
				rw.ParticleSystem.InjectParticle( p );
			}
		}
	}
}
