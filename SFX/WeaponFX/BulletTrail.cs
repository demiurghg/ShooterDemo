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
			AddStage("bulletSpark", 0, 0f, 0.1f, 30, EmitSpark );
		}



		void EmitSpark ( ref Particle p, Vector3 _pos )
		{
			var vel	=	fxEvent.Normal * rand.GaussDistribution(4,3) + rand.GaussRadialDistribution(0, 0.7f);
			var pos	=	fxEvent.Target;

			SetupMotion		( ref p, pos, vel, Vector3.Down * 7 );
			SetupAngles		( ref p, 160 );
			SetupColor		( ref p, 1000, 0, 1 );
			SetupTiming		( ref p, 0.5f, 0.01f, 0.01f );
			SetupSize		( ref p, 0.15f, 0.00f );
		}
	}
}
