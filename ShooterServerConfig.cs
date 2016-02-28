using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fusion;
using Fusion.Engine.Client;
using Fusion.Engine.Common;
using Fusion.Engine.Server;
using Fusion.Core.Content;
using Fusion.Engine.Graphics;
using ShooterDemo.Core;

namespace ShooterDemo {
	partial class ShooterServerConfig {

		public int TargetFrameRate { get; set; }

		public float ServerTimeDriftRate { get; set; }

		public int SimulateDelay { get; set; }



		public ShooterServerConfig()
		{
			TargetFrameRate		=	60;
			SimulateDelay		=	0;
			ServerTimeDriftRate	=	0.001f;
		}
	}
}
