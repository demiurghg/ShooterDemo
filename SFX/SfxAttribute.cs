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


namespace ShooterDemo.SFX {

	[AttributeUsage(AttributeTargets.Class)]
	public class SfxAttribute : Attribute {

		public readonly FXEventType FXType;

		public SfxAttribute ( FXEventType fxType )
		{
			this.FXType		=	fxType;
		}

		private SfxAttribute()
		{
		}
	}
}
