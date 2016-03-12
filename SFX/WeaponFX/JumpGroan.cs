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
	[Sfx(FXEventType.PlayerJump)]
	class JumpGroan : SfxInstance {
		
		public JumpGroan ( SfxSystem sfxSystem, FXEvent fxEvent ) : base(sfxSystem, fxEvent)
		{
			AddSoundStage( @"sound\character\jump",	fxEvent.Origin, 4 );
		}
	}
}
