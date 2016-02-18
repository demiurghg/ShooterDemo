using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion.Engine.Common;


namespace ShooterDemo.Core {
	public abstract class EntityController {
		
		public readonly Game Game;
		public readonly World World;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="game"></param>
		public EntityController ( World world )
		{
			World	=	world;
			Game	=	world.Game;
		}


		/// <summary>
		/// Updates controller.
		/// </summary>
		/// <param name="gameTime"></param>
		public abstract void Update ( GameTime gameTime );

		/// <summary>
		/// Called when entity has died.
		/// </summary>
		/// <param name="id"></param>
		public abstract void Kill ( uint id );
	}
}
