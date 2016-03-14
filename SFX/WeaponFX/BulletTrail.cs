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
	class BulletTrail : SfxInstance {
		
		public BulletTrail ( SfxSystem sfxSystem, FXEvent fxEvent ) : base(sfxSystem, fxEvent)
		{
			ShakeCamera( rand.GaussDistribution(0,10), rand.GaussDistribution(0,10), rand.GaussDistribution(0,10) );

			AddParticleStage("bulletSpark", 0, 0f, 0.1f, 30, EmitSpark );

			//AddParticleStage("explosionSmoke", 0, 0.1f, 1f, 15, EmitSmoke );

			AddLightStage( fxEvent.Target + fxEvent.Normal * 0.1f	, new Color4(100, 75, 50,1), 1, 100f, 3f );
			AddLightStage( fxEvent.Origin							, new Color4(200,150,100,1), 3, 100f, 3f );

			AddSoundStage( @"sound\weapon\machineGun2",	fxEvent.Origin, 1 );
			AddSoundStage( @"sound\weapon\bulletHit",	fxEvent.Target, 1 );
		}



		void EmitSpark ( ref Particle p, Vector3 _pos )
		{
			var vel	=	fxEvent.Normal * rand.GaussDistribution(4,3) + rand.GaussRadialDistribution(0, 0.7f);
			var pos	=	fxEvent.Target;

			SetupMotion		( ref p, pos, vel, Vector3.Down * 7 );
			SetupAngles		( ref p, 160 );
			SetupColor		( ref p, 500, 0, 1 );
			SetupTiming		( ref p, 0.5f, 0.01f, 0.9f );
			SetupSize		( ref p, 0.1f, 0.00f );
		}


		void EmitSmoke ( ref Particle p, Vector3 _pos )
		{
			var vel	=	fxEvent.Normal * rand.NextFloat(0.0f,1.5f) + rand.GaussRadialDistribution(0, 0.2f);
			var pos	=	fxEvent.Target;

			SetupMotion		( ref p, pos, vel, Vector3.Down * 1 );
			SetupAngles		( ref p, 15 );
			SetupColor		( ref p, 1, 0, 0.5f );
			SetupTiming		( ref p, 1.5f, 0.01f, 0.1f );
			SetupSize		( ref p, 0.2f, rand.NextFloat(0.5f, 1.2f) );
		}
	}
}
