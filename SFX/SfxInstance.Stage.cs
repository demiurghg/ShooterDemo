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

		protected class Stage {

			public bool Looped { get; set; }
			public bool Stopped { get; private set; }

			readonly SfxInstance	instance;
			readonly int			spriteIndex;
			readonly float 			delay;
			readonly float			period;
			readonly float			sleep;
			readonly int			count;
			readonly EmitFunction	emit;


			protected float			time		= 0;
			protected int			emitCount	= 0;
			

			/// <summary>
			/// 
			/// </summary>
			/// <param name="fxEvent"></param>
			/// <param name="spriteIndex"></param>
			/// <param name="delay"></param>
			/// <param name="period"></param>
			/// <param name="sleep"></param>
			/// <param name="count"></param>
			/// <param name="emit"></param>
			public Stage ( SfxInstance instance, int spriteIndex, float delay, float period, float sleep, int count, EmitFunction emit )
			{
				this.instance		=	instance	;
				this.spriteIndex	=	spriteIndex	;
				this.delay			=	delay		;
				this.period			=	period		;
				this.sleep			=	sleep		;
				this.count			=	count		;
				this.emit			=	emit		;
			}



			/// <summary>
			/// 
			/// </summary>
			/// <param name="dt"></param>
			public void Update ( float dt, FXEvent fxEvent )
			{	
				float old_time		=	time;
				float new_time		=	time + dt;
				
				var pos	=	fxEvent.Origin;
				var vel =	Vector3.Zero;
				
				if ( !Stopped ) {

					for ( int part=emitCount; true; part++ ) {
	
						float prt_time	= GetParticleEmitTime( part );
						float prt_dt	= prt_time - old_time;
		
						if (prt_time <= new_time) {

							float addTime = new_time - prt_time;

							Vector3 newPos = pos - vel * addTime; 
		
							var p = new Particle();
							p.TimeLag		=	addTime;
							p.ImageIndex	=	spriteIndex;
							p.Position		=	newPos;

							emit( ref p, newPos );

							instance.rw.ParticleSystem.InjectParticle( p );

							emitCount++;
			
						} else {
							break;
						}
					}

					if ( !Looped && ( time >= delay + period ) ) {
						Stopped = true;
					}

					time += dt;
				}
			}


			/// <summary>
			/// 
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			float GetParticleEmitTime( int index )
			{
				float	full_cycle			= delay + period + sleep;
				int		num_cycles			= index / count;
				int		part_ind_in_bunch	= index	% count;
				float	interval			= period / (float)count;
	
				return	full_cycle * num_cycles + 
						delay +
						interval * part_ind_in_bunch;
			}
		}

	}
}
