using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion.Engine.Common;


namespace ShooterDemo.Core {
	public interface IEntityController {

		/// <summary>
		/// Updates controller.
		/// </summary>
		/// <param name="gameTime"></param>
		void Update ( GameTime gameTime );

		/// <summary>
		/// Called when entity has died.
		/// </summary>
		/// <param name="id"></param>
		void Kill ( uint id );
	}
}
