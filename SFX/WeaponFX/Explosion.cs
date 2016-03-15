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
	class Explosion : SfxInstance {
		
		public Explosion ( SfxSystem sfxSystem, FXEvent fxEvent ) : base(sfxSystem, fxEvent)
		{
			AddParticleStage("bulletSpark",		0.00f, 0.0f, 0.1f, 1000, EmitSpark );
			AddParticleStage("explosionSmoke",	0.10f, 0.1f, 1.0f,   30, EmitSmoke );
			AddParticleStage("explosionFire",	0.00f, 0.1f, 1.0f,   30, EmitFire );

			AddLightStage( fxEvent.Origin + fxEvent.Normal * 0.1f	, new Color4(100, 75, 50,1), 1, 100f, 3f );

			AddSoundStage( @"sound\weapon\explosion",	fxEvent.Origin, 1 );
		}



		void EmitSpark ( ref Particle p, Vector3 _pos )
		{
			var vel	=	rand.GaussRadialDistribution(0, 8.0f);
			var pos	=	fxEvent.Origin;

			SetupMotion		( ref p, pos, vel, -vel );
			SetupAngles		( ref p, 160 );
			SetupColor		( ref p, 500, 0, 1 );
			SetupTiming		( ref p, 0.25f, 0.01f, 0.9f );
			SetupSize		( ref p, 0.2f, 0.00f );
		}


		void EmitSmoke ( ref Particle p, Vector3 _pos )
		{
			var dir = 	rand.UniformRadialDistribution(0,1);
			var vel	=	fxEvent.Normal * dir;
			var pos	=	fxEvent.Origin + dir;

			SetupMotion		( ref p, pos, vel, Vector3.Zero );
			SetupAngles		( ref p, 10 );
			SetupColor		( ref p, 5, 0, 1.0f );
			SetupTiming		( ref p, 1.5f, 0.1f, 0.1f );
			SetupSize		( ref p, 1.2f, 2 );
		}


		void EmitFire ( ref Particle p, Vector3 _pos )
		{
			var vel	=	rand.UniformRadialDistribution(0, 0.5f);
			var pos	=	fxEvent.Origin + rand.UniformRadialDistribution(1,1) * 0.25f;

			SetupMotion		( ref p, pos, vel, Vector3.Zero );
			SetupAngles		( ref p, 0 );
			SetupColor		( ref p, 2000, 0, 1.0f );
			SetupTiming		( ref p, 0.4f, 0.01f, 0.8f );
			SetupSize		( ref p, 2.0f, 2.0f );
		}
	}
}
