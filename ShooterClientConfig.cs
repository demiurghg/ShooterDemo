using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShooterDemo {
	public class ShooterClientConfig {

		public float Sensitivity { get; set; }
		public bool InvertMouse { get; set; }
		public float PullFactor { get; set; }
		public bool ThirdPerson { get; set; }

		public float ZoomFov { get; set; }
		public float Fov { get; set; }
		public float BobHeave	{ get; set; }
		public float BobPitch	{ get; set; }
		public float BobRoll	{ get; set; }
		public float BobStrafe  { get; set; }
		public float BobJump	{ get; set; }
		public float BobLand	{ get; set; }


		/// <summary>
		/// 
		/// </summary>
		public ShooterClientConfig ()
		{
			Sensitivity	=	5;
			InvertMouse	=	false;
			PullFactor	=	1;

			Fov			=	90.0f;
			ZoomFov		=	30.0f;

			BobHeave	=	0.05f;
			BobPitch	=	1.0f;
			BobRoll		=	2.0f;
			BobStrafe  	=	5.0f;
			BobJump		=	5.0f;
			BobLand		=	5.0f;
		}
	}
}
