using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion;
using Fusion.Core.Mathematics;
using Fusion.Core.Configuration;
using Fusion.Core.Shell;
using Fusion.Core.Utils;
using Fusion.Engine.Common;
using Fusion.Engine.Client;
using Fusion.Engine.Server;
using Fusion.Engine.Graphics;
using Fusion.Engine.Audio;
using Fusion.Engine.Input;
using System.IO;


namespace ShooterDemo.Entities {
	class StartPoint : Entity {

		public Matrix World { get; set; }


		
		/// <summary>
		/// Default constructor
		/// </summary>
		public StartPoint ()
		{
			var world				=	Matrix.Identity;
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="origin"></param>
		public StartPoint ( Node node, Matrix world )
		{
			World	=	world;
		}


		public override void Activate ( GameWorld gameWorld )
		{
		}


		public override void Deactivate ( GameWorld gameWorld )
		{
		}


		public override void Show ( RenderWorld rw )
		{
		}


		public override void Hide ( RenderWorld rw )
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update ( GameTime gameTime )
		{
		}




		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		public override void Read ( BinaryReader reader )
		{
			World		=	reader.Read<Matrix>();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public override void Write ( BinaryWriter writer )
		{
			writer.Write<Matrix>( World );
		}

	}
}
