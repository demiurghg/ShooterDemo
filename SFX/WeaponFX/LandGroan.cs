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
	[Sfx(FXEventType.PlayerLanding)]
	class LandGroan : SfxInstance {
		
		public LandGroan ( SfxSystem sfxSystem, FXEvent fxEvent ) : base(sfxSystem, fxEvent)
		{
			AddSoundStage( @"sound\character\fall0", fxEvent.Origin, 4 );
		}
	}
}
