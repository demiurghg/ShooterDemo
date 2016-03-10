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

		OmniLight	light = null;
		float	lightIntensity;
		float	lightFadeInRate;
		float	lightFadeOutRate;
		float	lightFadeRate;
		Color4	lightColor;
		Vector3	lightOffset;


		/// <summary>
		/// 
		/// </summary>
		/// <param name="color"></param>
		/// <param name="radius"></param>
		/// <param name="fadeIn"></param>
		/// <param name="fadeOut"></param>
		protected void AddLight ( Vector3 position, Color4 color, float radius, float fadeInRate, float fadeOutRate )
		{
			if ( fadeInRate  < 0 ) {
				throw new ArgumentOutOfRangeException("fadeInRate < 0");
			}
			if ( fadeOutRate < 0 ) {
				throw new ArgumentOutOfRangeException("fadeOutRate < 0");
			}

			this.lightOffset	=	lightOffset;

			light				=	new OmniLight();
			light.Position		=	position;
			light.RadiusInner	=	radius * 0.1f;
			light.RadiusOuter	=	radius;
			light.Intensity		=	Color4.Zero;

			lightColor			=	color;

			lightFadeInRate		=	fadeInRate;
			lightFadeOutRate	=	fadeOutRate;

			lightIntensity		=	0.0001f;
			lightFadeRate		=	lightFadeInRate;

			rw.LightSet.OmniLights.Add(light); 
		}



		/// <summary>
		/// 
		/// </summary>
		void KillLight ()
		{
			if (light!=null) {
				rw.LightSet.OmniLights.Remove(light);
			}
		}



		/// <summary>
		/// 
		/// </summary>
		bool IsLightExhausted {
			get {
				return lightIntensity <= 0;
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="dt"></param>
		protected void UpdateLight ( float dt )
		{
			if (light==null) {
				return;
			}

			lightIntensity += dt * lightFadeRate;
			lightIntensity =  MathUtil.Clamp( lightIntensity, 0, 1 );

			if ( lightIntensity >= 1 ) {
				lightIntensity = 1;
				lightFadeRate = -lightFadeOutRate;
			}

			light.Intensity = lightColor * lightIntensity;
		}


	}
}
