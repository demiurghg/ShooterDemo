﻿using System;
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
	class RocketTrail : SfxInstance {

		public RocketTrail ( SfxSystem sfxSystem, FXEvent fxEvent ) : base(sfxSystem, fxEvent)
		{
			//AddParticleStage("bulletSpark",		0.00f, 0.1f, 0.0f,  150, true, EmitSpark );
			AddParticleStage("explosionFire",	0.00f, 0.2f, 0.0f,   90, true, EmitFire );
			AddParticleStage("explosionSmoke",	0.00f, 0.2f, 0.0f,   60, true, EmitSmoke );

			AddLightStage( fxEvent.Origin * 0.1f	, new Color4(100, 75, 50,1), 1, 100f, 3f );

			AddSoundStage( @"sound\weapon\explosion",	fxEvent.Origin, 1 );
		}



		//void EmitSpark ( ref Particle p, FXEvent fxEvent )
		//{
		//	var vel	=	rand.GaussRadialDistribution(0, 6.0f);
		//	var pos	=	fxEvent.Origin;

		//	SetupMotion		( ref p, pos, Vector3.Zero, Vector3.Zero );
		//	SetupAngles		( ref p, 160 );
		//	SetupColor		( ref p, 500, 0, 1 );
		//	SetupTiming		( ref p, 0.2f, 0.01f, 0.9f );
		//	SetupSize		( ref p, 0.1f, 0.00f );
		//}


		void EmitFire ( ref Particle p, FXEvent fxEvent )
		{
			var vel	=	rand.GaussRadialDistribution(0, 0.3f) + Matrix.RotationQuaternion(fxEvent.Rotation).Backward * 5;
			var pos	=	fxEvent.Origin;

			SetupMotion		( ref p, pos, vel, Vector3.Zero );
			SetupAngles		( ref p, 0 );
			SetupColor		( ref p, 1000, 0, 1.0f );
			SetupTiming		( ref p, 0.2f, 0.01f, 0.1f );
			SetupSize		( ref p, 0.2f, 0.6f );
		}

		
		
		void EmitSmoke ( ref Particle p, FXEvent fxEvent )
		{
			var dir = 	rand.UniformRadialDistribution(0,1);
			var vel	=	dir * 0.5f;
			var pos	=	fxEvent.Origin;

			SetupMotion		( ref p, pos, vel, -vel, 0, -0.1f );
			SetupAngles		( ref p, 10 );
			SetupColor		( ref p, 5, 0, 1.0f );
			SetupTiming		( ref p, 1.0f, 0.4f, 0.4f );
			SetupSize		( ref p, 0.2f, 0.5f );
		}



	}
}
