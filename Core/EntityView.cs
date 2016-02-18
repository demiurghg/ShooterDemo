using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion.Engine.Common;


namespace ShooterDemo.Core {
	public abstract class EntityView {

		public readonly Game Game;
		public readonly World World;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="game"></param>
		public EntityView ( World world )
		{
			World	=	world;
			Game	=	world.Game;
		}

		/// <summary>
		/// Called on each viewable entity.
		/// </summary>
		/// <param name="entity"></param>
		public abstract void Present ( GameTime gameTime );

		/// <summary>
		/// Called when entity has died.
		/// </summary>
		/// <param name="id"></param>
		public abstract void Kill ( uint id );
	}
}
