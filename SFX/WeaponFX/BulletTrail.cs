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
	[Sfx(FXEventType.BulletTrail)]
	class BulletTrail : SfxInstance {
		
		public BulletTrail ( SfxSystem sfxSystem, FXEvent fxEvent ) : base(sfxSystem, fxEvent)
		{
			AddParticleStage("bulletSpark", 0, 0f, 0.1f, 30, EmitSpark );

			AddLightStage( fxEvent.Target + fxEvent.Normal * 0.05f	, new Color4(200,150,100,1), 1, 100f, 3f );
			AddLightStage( fxEvent.Origin							, new Color4(200,150,100,1), 3, 100f, 3f );

			AddSoundStage( @"sound\weapon\machineGun",	fxEvent.Origin, 1 );
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
	}
}
