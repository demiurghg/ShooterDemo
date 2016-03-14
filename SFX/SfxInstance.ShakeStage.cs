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

		/*protected class ShakeStage : Stage {

		//	public ShakeStage 

		} */



		class Oscillator {

			public float Offset { get { return offset; } }
			public float Target = 0;

			readonly float damping;
			readonly float stiffness;
			float offset	=	0;
			float velocity	=	0;
			const float mass = 1;

			public Oscillator ( float stiffness, float damping )
			{
				this.stiffness	=	stiffness;
				this.damping	=	damping;
			}

			public void Kick ( float velocity )
			{
				this.velocity	=	velocity;
			}


			public void Update ( float elapsed )
			{
				float force =	(Target-offset) * stiffness - velocity * damping;
				velocity	=	velocity + (force/mass) * elapsed;
				offset		=	offset + velocity * elapsed;
			}
		}

	}
}
