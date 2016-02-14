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
using BEPUphysics;
using BEPUphysics.Character;
using System.IO;


namespace ShooterDemo.Entities {
	class Player : Entity {

		public Guid UserGuid {
			get; private set;
		}


		public Matrix World { get; set; }


		CharacterController	charCtrl;


		
		/// <summary>
		/// Default constructor
		/// </summary>
		public Player ()
		{
			var world				=	Matrix.Identity;
			world.TranslationVector	=	Vector3.Up * 4;
			World	=	world;
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="origin"></param>
		public Player ( Space physSpace, Guid userGuid, Matrix world )
		{
			UserGuid	=	userGuid;
			World		=	world;

			charCtrl	=	new CharacterController( MathConverter.Convert( world.TranslationVector ) );
			physSpace.Add( charCtrl );
		}


		public override void Show ( RenderWorld rw )
		{
		}


		public override void Hide ( RenderWorld rw )
		{
		}


		public override void Activate ( GameWorld gameWorld )
		{
		}


		public override void Deactivate ( GameWorld gameWorld )
		{
		}




		/// <summary>
		/// 
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update ( GameTime gameTime )
		{
			//throw new NotImplementedException();
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="userCmd"></param>
		public void FeedCommand ( UserCommand userCmd )
		{
			if (userCmd.CtrlFlags.HasFlag( UserCtrlFlags.Forward )) {
				World *= Matrix.Translation( Vector3.ForwardRH * 0.1f );
			}
			if (userCmd.CtrlFlags.HasFlag( UserCtrlFlags.Backward )) {
				World *= Matrix.Translation( Vector3.BackwardRH * 0.1f );
			}
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		public override void Read ( BinaryReader reader )
		{
			UserGuid	=	new Guid(reader.ReadBytes(16));
			World		=	reader.Read<Matrix>();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public override void Write ( BinaryWriter writer )
		{
			writer.Write( UserGuid.ToByteArray() );
			writer.Write<Matrix>( World );
		}

	}
}
