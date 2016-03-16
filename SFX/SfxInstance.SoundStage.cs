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

		
		protected class SoundStage : Stage {

			AudioEmitter	emitter;

			CurvePoint[]	curve	=	Enumerable.Range(0,5).Select( i => new CurvePoint(i/4.0f, 1.0f-i/4.0f) ).ToArray();

			/// <summary>
			/// 
			/// </summary>
			/// <param name="instance"></param>
			/// <param name="position"></param>
			/// <param name="soundPath"></param>
			public SoundStage ( SfxInstance instance, Vector3 position, float radius, string soundPath ) : base(instance)
			{
				var sound	=	instance.sfxSystem.LoadSound( soundPath );

				if (sound==null) {
					return;
				}

				emitter	=	SfxInstance.sw.AllocEmitter();
				emitter.Position		=	position;
				emitter.DistanceScale	=	radius;
				emitter.DopplerScale	=	1;
				emitter.VolumeCurve		=	null;

				emitter.PlaySound( sound, PlayOptions.None );
			}


			public override void Stop ( bool immediate )
			{
				emitter.StopSound( immediate );
			}

			public override bool IsExhausted ()
			{
				return (emitter.SoundState==SoundState.Stopped);
			}

			public override void Kill ()
			{
				SfxInstance.sw.FreeEmitter( emitter );
			}

			public override void Update ( float dt, FXEvent fxEvent )
			{
				emitter.Position	=	fxEvent.Origin;
				emitter.Velocity	=	fxEvent.Velocity;
			}

		}

	}
}
