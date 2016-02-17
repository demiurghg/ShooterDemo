using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion.Engine.Common;


namespace ShooterDemo.Core {
	public abstract class EntityView {

		/// <summary>
		/// Updates controller.
		/// </summary>
		/// <param name="gameTime"></param>
		public abstract void Update ( GameTime gameTime );

		/// <summary>
		/// Called on each viewable entity.
		/// </summary>
		/// <param name="entity"></param>
		public abstract void Present ( GameTime gameTime, ref Entity entity );

		/// <summary>
		/// Gets indices of all controlled entities.
		/// </summary>
		/// <returns></returns>
		public abstract uint[] GetIDs();

		/// <summary>
		/// Called when entity has died.
		/// </summary>
		/// <param name="id"></param>
		public abstract void Kill ( uint id );
	}
}
