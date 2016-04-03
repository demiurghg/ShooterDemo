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
	class PlayerPain : SfxInstance {
		
		public PlayerPain ( SfxSystem sfxSystem, FXEvent fxEvent ) : base(sfxSystem, fxEvent)
		{
			ShakeCamera( rand.GaussDistribution(0,140), rand.GaussDistribution(0,140), rand.GaussDistribution(0,140) );

			//AddSoundStage( @"sound\character\jump",	fxEvent.Origin, 4, false );
		}
	}
}
