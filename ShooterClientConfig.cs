using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShooterDemo {
	public class ShooterClientConfig {

		public float Sensitivity { get; set; }
		public bool InvertMouse { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public ShooterClientConfig ()
		{
			Sensitivity	=	5;
			InvertMouse	=	false;
		}
	}
}
